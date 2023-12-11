using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SheepController : MonoBehaviour
{
    // bark interaction
    private GameObject sheepDog;
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
    private GameObject[] herd;
    public bool isGrounded = false;
    private float jumpDelayModifier = 7.5f;

    // player proximity
    private Vector3 sheepDogProximity;
    private float sheepDogProximityX;
    private float sheepDogProximityZ;

    // UI
    private bool isGameActive;
    private GameObject uiManager;

    // audio
    private GameObject audioManager;

    // spawn manager
    private GameObject spawnManager;

    // stray sheep
    public bool isHerdSheep;

    // wolf 
    public bool hasEnteredWolfSpace;
    public bool hasAvoidedWolf;

    private float xDistanceFromBoundary = 1.5f;

    // animation
    private GameObject animationManager;

    // Start is called before the first frame update
    void Start()
    {
        sheepDog = GameObject.Find("Sheepdog");
        sheepRb = GetComponent<Rigidbody>();
        pastBarkJumpState = isBarkedJumpAt;
        uiManager = GameObject.Find("UIManager");
        audioManager = GameObject.Find("AudioManager");
        spawnManager = GameObject.Find("SpawnManager");
        animationManager = GameObject.Find("AnimationManager");
    }

    // Update is called once per frame
    void Update()
    {
        isGameActive = uiManager.GetComponent<UIManager>().isGameActive;

        if (isGameActive)
        {
            CheckPlayerActivity();

            // keep track of herd
            herd = GameObject.FindGameObjectsWithTag("Sheep");

            // sheep behaviour based on tag
            DetermineSheepBehaviour();
        }
    }

    private void CheckPlayerActivity()
    {
        // keep track of barks
        isBarkedAt = sheepDog.GetComponent<PlayerController>().hasBarkedMove;
        isBarkedJumpAt = sheepDog.GetComponent<PlayerController>().hasBarkedJump;

        // player proximity
        sheepDogProximityX = transform.position.x - sheepDog.transform.position.x;
        sheepDogProximityZ = transform.position.z - sheepDog.transform.position.z;
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
            float jumpDelay = CalculateDelay(herd);
            StartCoroutine(StaggeredJump(jumpDelay));
        }

        // track change of state of bark low to high
        pastBarkJumpState = isBarkedJumpAt;


        // sheep try to keep a small distance from each other
        foreach (GameObject sheep in herd)
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

            if (!hasAvoidedWolf && Mathf.Abs(wolfProximityX) < 5.0f && Mathf.Abs(wolfProximityZ) < 7.0f)
            {
                if (isGrounded)
                {
                    Jump(jumpDirection, jumpForce);
                }
                Avoid(wolfProximity, avoidSpeed);
                hasEnteredWolfSpace = true;
            }

            if (hasEnteredWolfSpace && Mathf.Abs(wolfProximityZ) > 7.0f)
            {
                hasAvoidedWolf = true;
            }
        }

        MovementBoundaries();

        // sheep is lost if allowed to drift back too far
        if (transform.position.z < zBackwardBoundary)
        {
            audioManager.GetComponent<AudioManager>().hasDetectedLostSheep = true;
            spawnManager.GetComponent<SpawnManager>().timeSinceLostSheep = 0;
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
            if (isGrounded)
            {
                Jump(jumpDirection, jumpForce);
            }
        }
        else
        {
            sheepRb.isKinematic = true;
        }

        // movement across trail
        Vector3 spawnPosition = spawnManager.GetComponent<SpawnManager>().straySheepSpawnPosition;
        Vector3 targetPosition = spawnManager.GetComponent<SpawnManager>().straySheepTargetPosition;
        Vector3 targetDirection = targetPosition - spawnPosition;
        MoveAcrossTrail(targetDirection);

        // stray sheep avoids herd sheep
        foreach (GameObject sheep in herd)
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
            Destroy(gameObject);
        }
        if (transform.position.z > zBoundForward || transform.position.z < zBoundBack)
        {
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
        animationManager.GetComponent<AnimationManager>().playSheepJumpAnimation = true;
        animationManager.GetComponent<AnimationManager>().sheepId = name;
        sheepRb.AddForce(new Vector3(hopDirectionX, hopDirectionY, hopDirectionZ) * force, ForceMode.Impulse);
        isGrounded = false;
    }

    IEnumerator StaggeredJump(float delay)
    {
        yield return new WaitForSeconds(delay);
        animationManager.GetComponent<AnimationManager>().playSheepJumpAnimation = true;
        animationManager.GetComponent<AnimationManager>().sheepId = name;
        sheepRb.AddForce(jumpDirection * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    void Jump(Vector3 direction, float force)
    {
        animationManager.GetComponent<AnimationManager>().playSheepJumpAnimation = true;
        animationManager.GetComponent<AnimationManager>().sheepId = name;
        sheepRb.AddForce(direction * force, ForceMode.Impulse);
        isGrounded = false;
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

    private void OnCollisionEnter(Collision collision)
    {
        if (tag == "Sheep" && collision.gameObject.tag == "Obstacle")
        {
            audioManager.GetComponent<AudioManager>().hasDetectedCollision = true;
            audioManager.GetComponent<AudioManager>().hasDetectedLostSheep = true;
            spawnManager.GetComponent<SpawnManager>().timeSinceLostSheep = 0;

            Debug.Log("Sheep Lost!");
            Destroy(gameObject);
        }

        if (collision.gameObject.tag == "Trail Lane" || collision.gameObject.tag == "Boundary Lane")
        {
            isGrounded = true;
        }

        // hop away if landed on top of another sheep or player
        if ((collision.gameObject.tag == "Sheep" || collision.gameObject.tag == "Player") && (transform.position.y - collision.gameObject.transform.position.y) > heightTrigger )
        {
            Hop(jumpForce);
        }

        // hop away if player lands ontop of sheep
        if (collision.gameObject.tag == "Player" && (collision.gameObject.transform.position.y - transform.position.y) > heightTrigger && isGrounded)
        {
            Hop(throwForce);
        }
    }
}
