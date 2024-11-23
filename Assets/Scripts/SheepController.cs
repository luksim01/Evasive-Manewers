using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SheepController : MonoBehaviour, ISheepController
{
    public Transform SheepTransform { get; set; }

    // bark interaction
    private bool isBarkedAt;
    private bool isBarkedJumpAt;
    private bool pastBarkJumpState;
    private Rigidbody sheepRb;

    public bool isSlowingDown = true;

    // interaction boundaries
    private float xBoundary = 7.3f;
    private float xAvoidDistance = 1.2f;
    private int zForwardBoundary = 15;
    private int zAvoidDistance = 5;
    private int zBackwardBoundary = -32;

    // escape controls
    private Vector3 fleeDirection;
    private float speedBurst = 4.0f;
    private float jumpForce = 8.0f;
    private float throwForce = 8.0f;

    private float sheepSlowdownSpeed = 0.5f;
    private float boundaryAvoidSpeed = 0.3f;
    private int avoidSpeed = 2;

    private float heightBoundary = 2.0f;

    private float hopDirectionX;
    private float hopDirectionY = 0.5f;
    private float hopDirectionZ = -0.2f;
    private Vector3 jumpDirection = new Vector3(0, 1.2f, 0.1f);
    private float heightTrigger = 2.2f;

    // staggered jump
    public bool IsGrounded { get; set; }
    private float jumpDelayModifier = 7.5f;

    // player proximity
    private Vector3 sheepDogProximity;
    private float sheepDogProximityX;
    private float sheepDogProximityZ;
    
    // stray sheep
    public bool isHerdSheep;

    // wolf 
    public bool HasEnteredWolfSpace { get; set; }
    public bool HasAvoidedWolf { get; set; }

    private float xDistanceFromBoundary = 1.5f;

    // animation
    private Animator sheepBodyAnim;
    private Animator sheepHeadAnim;

    // particle
    public GameObject sheepCollisionEffect;

    // audio
    private IAudioManager _audioManager;

    // ui
    private bool isGameActive;
    private IUIManager _uiManager;

    // spawn manager
    private ISpawnManager _spawnManager;

    // player
    private IPlayerController _sheepdog;

    // dependancies
    public void SetDependencies(IAudioManager audioManager, IUIManager uiManager, ISpawnManager spawnManager, IPlayerController playerController)
    {
        _audioManager = audioManager;
        _uiManager = uiManager;
        _spawnManager = spawnManager;
        _sheepdog = playerController;
    }

    // Start is called before the first frame update
    void Start()
    {
        sheepRb = GetComponent<Rigidbody>();
        pastBarkJumpState = isBarkedJumpAt;

        sheepBodyAnim = this.transform.Find("sheep_body").GetComponent<Animator>();
        sheepHeadAnim = this.transform.Find("sheep_head").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //isGameActive = uiManager.GetComponent<UIManager>().isGameActive;
        isGameActive = _uiManager.IsGameActive;

        if (isGameActive)
        {
            CheckPlayerActivity();

            // sheep behaviour based on tag
            DetermineSheepBehaviour();
        }

        SheepTransform = this.transform;
    }

    private void CheckPlayerActivity()
    {
        // keep track of barks
        isBarkedAt = _sheepdog.HasBarkedMove;
        isBarkedJumpAt = _sheepdog.HasBarkedJump;

        // player proximity
        sheepDogProximityX = transform.position.x - _sheepdog.PlayerTransform.position.x;
        sheepDogProximityZ = transform.position.z - _sheepdog.PlayerTransform.position.z;
        sheepDogProximity = new Vector3(sheepDogProximityX, 0, sheepDogProximityZ);
    }

    private void DetermineSheepBehaviour()
    {
        switch (tag)
        {
            case "Sheep":
                HerdSheepBehaviour();
                break;
            case "Stray":
                StraySheepBehaviour();
                break;
            case "Hunted":
                HuntedSheepBehaviour();
                break;
            default:
                break;
        }
    }

    void HerdSheepBehaviour()
    {
        // keep track of wolf
        GameObject wolf = GameObject.FindGameObjectWithTag("Wolf");
        
        // avoid player
        if (Mathf.Abs(sheepDogProximityX) < 2.5f && Mathf.Abs(sheepDogProximityZ) < 7.0f)
        {
            Avoid(sheepDogProximity, avoidSpeed);
        }
        else if (isSlowingDown)
        {
            // sheep gradually falls behind
            transform.Translate(Vector3.back * sheepSlowdownSpeed * Time.deltaTime);
        }

        // sheep flee bark if within certain distance of it
        if (isBarkedAt && Mathf.Abs(sheepDogProximityX) < 6.0f && Mathf.Abs(sheepDogProximityZ) < 10.0f)
        {
            FleeBark(sheepDogProximity);
        }

        // stagger jump of herd
        if (isBarkedJumpAt != pastBarkJumpState && !pastBarkJumpState)
        {
            float jumpDelay = CalculateDelay(_spawnManager.Herd);
            StartCoroutine(StaggeredJump(jumpDelay));
        }

        // track change of state of bark low to high
        pastBarkJumpState = isBarkedJumpAt;


        // sheep try to keep a small distance from each other
        foreach (GameObject sheep in _spawnManager.Herd)
        {
            float sheepProximityX = transform.position.x - sheep.transform.position.x;
            float sheepProximityZ = transform.position.z - sheep.transform.position.z;

            Vector3 sheepProximity = new Vector3(sheepProximityX, 0, sheepProximityZ);

            if (Mathf.Abs(sheepProximityX) < 2.0f && Mathf.Abs(sheepProximityZ) < 3.0f)
            {
                Avoid(sheepProximity, avoidSpeed);
            }
        }

        // sheep act frantic when there's a nearby wolf
        if (wolf)
        {
            float wolfProximityX = transform.position.x - wolf.transform.position.x;
            float wolfProximityZ = transform.position.z - wolf.transform.position.z;

            Vector3 wolfProximity = new Vector3(wolfProximityX, 0, 0);

            if (!HasAvoidedWolf && Mathf.Abs(wolfProximityX) < 5.0f && Mathf.Abs(wolfProximityZ) < 7.0f)
            {
                if (IsGrounded)
                {
                    Jump(jumpDirection, jumpForce);
                }
                Avoid(wolfProximity, avoidSpeed);
                HasEnteredWolfSpace = true;
            }

            if (HasEnteredWolfSpace && Mathf.Abs(wolfProximityZ) > 7.0f)
            {
                HasAvoidedWolf = true;
            }
        }

        MovementBoundaries();

        // sheep is lost if allowed to drift back too far
        if (transform.position.z < zBackwardBoundary)
        {
            _audioManager.HasDetectedLostSheep = true;
            _spawnManager.TimeSinceLostSheep = 0;
            Destroy(gameObject);
        }
    }

    private void MovementBoundaries()
    {
        // sheep remains within forest trail, and tries to move away from the forest trail boundary
        if (transform.position.x > xBoundary - xAvoidDistance && transform.position.y < heightBoundary)
        {
            sheepRb.AddForce(Vector3.left * boundaryAvoidSpeed, ForceMode.Impulse);
        }
        if (transform.position.x > xBoundary)
        {
            transform.position = new Vector3(xBoundary, transform.position.y, transform.position.z);
        }

        if (transform.position.x < -xBoundary + xAvoidDistance && transform.position.y < heightBoundary)
        {
            sheepRb.AddForce(Vector3.right * boundaryAvoidSpeed, ForceMode.Impulse);
        }
        if (transform.position.x < -xBoundary)
        {
            transform.position = new Vector3(-xBoundary, transform.position.y, transform.position.z);
        }

        // sheep doesn't move too far forward on the trail and flees backwards if forced to
        if (transform.position.z > zForwardBoundary - zAvoidDistance && transform.position.y < heightBoundary)
        {
            sheepRb.AddForce(Vector3.back * boundaryAvoidSpeed, ForceMode.Impulse);
        }
        if (transform.position.z > zForwardBoundary)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, zForwardBoundary);
        }
    }

    void StraySheepBehaviour()
    {
        //unaffected by physics while outside of trail
        if (transform.position.x < xBoundary + xDistanceFromBoundary && transform.position.x > -xBoundary - xDistanceFromBoundary)
        {
            sheepRb.isKinematic = false;
            if (IsGrounded)
            {
                Jump(jumpDirection, jumpForce);
            }
        }
        else
        {
            sheepRb.isKinematic = true;
        }

        // movement across trail
        Vector3 spawnPosition = _spawnManager.StraySheepSpawnPosition;
        Vector3 targetPosition = _spawnManager.StraySheepTargetPosition;

        Vector3 targetDirection = targetPosition - spawnPosition;
        MoveAcrossTrail(targetDirection);

        // stray sheep avoids herd sheep
        foreach (GameObject sheep in _spawnManager.Herd)
        {
            float sheepProximityX = transform.position.x - sheep.transform.position.x;
            float sheepProximityZ = transform.position.z - sheep.transform.position.z;

            Vector3 sheepProximity = new Vector3(sheepProximityX, 0, sheepProximityZ);

            if (Mathf.Abs(sheepProximityX) < 2.7f && Mathf.Abs(sheepProximityZ) < 3.7f)
            {
                Avoid(sheepProximity, avoidSpeed);
            }
        }

        // stray sheep avoid player
        if (Mathf.Abs(sheepDogProximityX) < 2.7f && Mathf.Abs(sheepDogProximityZ) < 3.7f)
        {
            Avoid(sheepDogProximity, avoidSpeed);
        }

        // stray sheep added to herd with close proximity bark within trail
        if (isBarkedAt &&
            Mathf.Abs(sheepDogProximityX) < 3.0f &&
            Mathf.Abs(sheepDogProximityZ) < 5.0f &&
            transform.position.x < xBoundary - xAvoidDistance &&
            transform.position.x > -xBoundary + xAvoidDistance)
        {
            gameObject.tag = "Sheep";
        }

        // stray sheep is gone once beyond the forest boundary
        if (Mathf.Abs(targetPosition.x - transform.position.x) <= 0.2)
        {
            Destroy(gameObject);
        }
    }

    void MoveAcrossTrail(Vector3 direction)
    {
        Vector3 alignDirection = (direction).normalized;
        transform.Translate(alignDirection * 3 * Time.deltaTime);
    }


    void HuntedSheepBehaviour()
    {
        //unaffected by physics while outside of trail
        if (transform.position.x < xBoundary + xDistanceFromBoundary && transform.position.x > -xBoundary - xDistanceFromBoundary)
        {
            sheepRb.isKinematic = false;
        }
        else
        {
            sheepRb.isKinematic = true;
        }

        // keep track of wolf
        GameObject wolf = GameObject.FindGameObjectWithTag("Wolf");

        if (wolf)
        {
            float wolfProximityX = transform.position.x - wolf.transform.position.x;
            float wolfProximityZ = transform.position.z - wolf.transform.position.z;

            Vector3 wolfProximity = new Vector3(wolfProximityX, 0, wolfProximityZ);

            // flee wolf
            if (Mathf.Abs(wolfProximityX) < 4.5f && Mathf.Abs(wolfProximityZ) < 9.0f)
            {
                Avoid(wolfProximity, avoidSpeed*2);
            }
        }
        else
        {
            DestroyBoundaries(12, -13, 30, -30);
        }
    }

    void DestroyBoundaries(float xBoundRight, float xBoundLeft, float zBoundForward, float zBoundBack)
    {
        // destroy if out of bounds
        if (transform.position.x > xBoundRight || transform.position.x < xBoundLeft)
        {
            PlaySheepCollisionEffect();
            _audioManager.HasDetectedLostSheep = true;
            Destroy(gameObject);
        }
        if (transform.position.z > zBoundForward || transform.position.z < zBoundBack)
        {
            PlaySheepCollisionEffect();
            _audioManager.HasDetectedLostSheep = true;
            Destroy(gameObject);
        }
    }

    private float CalculateDelay(GameObject[] herd)
    {
        float originZ = zBackwardBoundary;
        float frontSheepPosZ = transform.position.z - originZ;

        // largest distance to generate delays for staggered jump
        foreach (GameObject sheep in herd)
        {
            if ((sheep.transform.position.z - originZ) > frontSheepPosZ)
            {
                frontSheepPosZ = sheep.transform.position.z - originZ;
            }
        }

        float delay = jumpDelayModifier * (frontSheepPosZ - (transform.position.z - originZ)) / (frontSheepPosZ);
        return delay;
    }

    void Hop(float force)
    {
        hopDirectionX = Random.Range(-0.2f, 0.2f);
        sheepBodyAnim.SetTrigger("isJumping");
        sheepHeadAnim.SetTrigger("isJumping");
        sheepRb.AddForce(new Vector3(hopDirectionX, hopDirectionY, hopDirectionZ) * force, ForceMode.Impulse);
        IsGrounded = false;
    }

    IEnumerator StaggeredJump(float delay)
    {
        yield return new WaitForSeconds(delay);
        sheepBodyAnim.SetTrigger("isJumping");
        sheepHeadAnim.SetTrigger("isJumping");
        sheepRb.AddForce(jumpDirection * jumpForce, ForceMode.Impulse);
        IsGrounded = false;
    }

    void Jump(Vector3 direction, float force)
    {
        sheepBodyAnim.SetTrigger("isJumping");
        sheepHeadAnim.SetTrigger("isJumping");
        sheepRb.AddForce(direction * force, ForceMode.Impulse);
        IsGrounded = false;
    }

    void Avoid(Vector3 direction, float speed)
    {
        Vector3 alignDirection = (direction).normalized;
        transform.Translate(alignDirection * speed * Time.deltaTime);
    }

    void FleeBark(Vector3 direction)
    {
        fleeDirection = (direction).normalized;
        float fleeDirectionX = fleeDirection.x;
        float fleeDirectionZ = fleeDirection.z;
        transform.Translate(new Vector3(fleeDirectionX, 0, fleeDirectionZ) * speedBurst * Time.deltaTime);
    }

    void PlaySheepCollisionEffect()
    {
        Instantiate(sheepCollisionEffect, transform.position, transform.rotation);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (CompareTag("Sheep") && collision.gameObject.CompareTag("Obstacle"))
        {
            _audioManager.HasDetectedCollision = true;
            _audioManager.HasDetectedLostSheep = true;
            _spawnManager.TimeSinceLostSheep = 0;
            PlaySheepCollisionEffect();
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Trail Lane") || collision.gameObject.CompareTag("Boundary Lane"))
        {
            IsGrounded = true;
        }

        // hop away if landed on top of another sheep or player
        if ((collision.gameObject.CompareTag("Sheep") || collision.gameObject.CompareTag("Player")))
        {
            if ((this.transform.position.y - collision.gameObject.transform.position.y) > heightTrigger)
            {
                Hop(jumpForce);
            }
        }

        // hop away if player lands ontop of sheep
        if (collision.gameObject.CompareTag("Player"))
        {
            if ((_sheepdog.PlayerTransform.position.y - this.transform.position.y) > heightTrigger && IsGrounded)
            {
                Hop(throwForce);
            }
        }
    }
}
