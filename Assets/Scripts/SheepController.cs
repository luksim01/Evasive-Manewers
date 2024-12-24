using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepController : MonoBehaviour, ISheepController, ICollidable
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
    private float xDistanceFromBoundary = 1.5f;

    // animation
    private Animator sheepBodyAnim;
    private Animator sheepHeadAnim;

    // audio
    private IAudioManager _audioManager;

    // ui
    private IUIManager _uiManager;

    // spawn manager
    private ISpawnManager _spawnManager;
    private bool hasInitialisedSheep = false;

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

    // collision
    public bool HasCollided { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        sheepRb = GetComponent<Rigidbody>();
        pastBarkJumpState = isBarkedJumpAt;
    }

    // Update is called once per frame
    //void Update()
    //{

    //    if (_uiManager.IsGameActive && hasInitialisedSheep)
    //    {
    //        CheckPlayerActivity();

    //        // sheep behaviour based on tag
    //        DetermineSheepBehaviour();
    //    }
    //}

    void OnEnable()
    {
        InitialiseSheep();
    }

    void FixedUpdate()
    {
        if (_uiManager.IsGameActive && hasInitialisedSheep)
        {
            CheckPlayerActivity();

            // sheep behaviour based on tag
            DetermineSheepBehaviour();
        }
    }

    void InitialiseSheep()
    {
        SheepTransform = this.transform;
        sheepBodyAnim = SheepTransform.Find("sheep_body").GetComponent<Animator>();
        sheepHeadAnim = SheepTransform.Find("sheep_head").GetComponent<Animator>();
        hasInitialisedSheep = true;
    }

    void ReturnToPoolAndReset(GameObject gameObject)
    {
        gameObject.tag = "Stray";
        _spawnManager.RemoveSheepFromHerd(gameObject);
        ObjectPoolUtility.Return(gameObject);
    }

    private void CheckPlayerActivity()
    {
        // keep track of barks
        isBarkedAt = _sheepdog.HasBarkedMove;
        isBarkedJumpAt = _sheepdog.HasBarkedJump;

        // player proximity
        sheepDogProximityX = SheepTransform.position.x - _sheepdog.PlayerTransform.position.x;
        sheepDogProximityZ = SheepTransform.position.z - _sheepdog.PlayerTransform.position.z;
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
        // avoid player
        if (Mathf.Abs(sheepDogProximityX) < 2.5f && Mathf.Abs(sheepDogProximityZ) < 7.0f)
        {
            Avoid(sheepDogProximity, avoidSpeed);
        }
        else if (isSlowingDown)
        {
            // sheep gradually falls behind
            SheepTransform.Translate(Vector3.back * sheepSlowdownSpeed * Time.deltaTime);
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
            float sheepProximityX = SheepTransform.position.x - sheep.transform.position.x;
            float sheepProximityZ = SheepTransform.position.z - sheep.transform.position.z;

            Vector3 sheepProximity = new Vector3(sheepProximityX, 0, sheepProximityZ);

            if (Mathf.Abs(sheepProximityX) < 2.0f && Mathf.Abs(sheepProximityZ) < 3.0f)
            {
                Avoid(sheepProximity, avoidSpeed);
            }
        }

        // sheep act frantic when there's a nearby wolf
        foreach (GameObject wolf in _spawnManager.Pack)
        {
            float wolfProximityX = SheepTransform.position.x - wolf.transform.position.x;
            float wolfProximityZ = SheepTransform.position.z - wolf.transform.position.z;

            Vector3 wolfProximity = new Vector3(wolfProximityX, 0, 0);

            if (Mathf.Abs(wolfProximityX) < 5.0f && Mathf.Abs(wolfProximityZ) < 7.0f)
            {
                if (IsGrounded)
                {
                    Jump(jumpDirection, jumpForce);
                }
                Avoid(wolfProximity, avoidSpeed);
            }
        }

        MovementBoundaries();

        // sheep is lost if allowed to drift back too far
        if (SheepTransform.position.z < zBackwardBoundary)
        {
            _audioManager.HasDetectedLostSheep = true;
            _spawnManager.TimeSinceLostSheep = 0;
            ReturnToPoolAndReset(gameObject);
        }
    }

    private void MovementBoundaries()
    {
        // sheep remains within forest trail, and tries to move away from the forest trail boundary
        if (SheepTransform.position.x > xBoundary - xAvoidDistance && SheepTransform.position.y < heightBoundary)
        {
            sheepRb.AddForce(Vector3.left * boundaryAvoidSpeed, ForceMode.Impulse);
        }
        if (SheepTransform.position.x > xBoundary)
        {
            SheepTransform.position = new Vector3(xBoundary, SheepTransform.position.y, SheepTransform.position.z);
        }

        if (SheepTransform.position.x < -xBoundary + xAvoidDistance && SheepTransform.position.y < heightBoundary)
        {
            sheepRb.AddForce(Vector3.right * boundaryAvoidSpeed, ForceMode.Impulse);
        }
        if (SheepTransform.position.x < -xBoundary)
        {
            SheepTransform.position = new Vector3(-xBoundary, SheepTransform.position.y, SheepTransform.position.z);
        }

        // sheep doesn't move too far forward on the trail and flees backwards if forced to
        if (SheepTransform.position.z > zForwardBoundary - zAvoidDistance && SheepTransform.position.y < heightBoundary)
        {
            sheepRb.AddForce(Vector3.back * boundaryAvoidSpeed, ForceMode.Impulse);
        }
        if (SheepTransform.position.z > zForwardBoundary)
        {
            SheepTransform.position = new Vector3(SheepTransform.position.x, SheepTransform.position.y, zForwardBoundary);
        }
    }

    void StraySheepBehaviour()
    {
        //unaffected by physics while outside of trail
        if (SheepTransform.position.x < xBoundary + xDistanceFromBoundary && SheepTransform.position.x > -xBoundary - xDistanceFromBoundary)
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
            float sheepProximityX = SheepTransform.position.x - sheep.transform.position.x;
            float sheepProximityZ = SheepTransform.position.z - sheep.transform.position.z;

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
            SheepTransform.position.x < xBoundary - xAvoidDistance &&
            SheepTransform.position.x > -xBoundary + xAvoidDistance)
        {
            _spawnManager.AddSheepToHerd(gameObject);
            _spawnManager.RemoveSheepFromStrays(gameObject);
            gameObject.tag = "Sheep";
        }

        // stray sheep is gone once beyond the forest boundary
        if (Mathf.Abs(targetPosition.x - SheepTransform.position.x) <= 0.2)
        {
            _spawnManager.RemoveSheepFromStrays(gameObject);
            ReturnToPoolAndReset(gameObject);
        }
    }

    void MoveAcrossTrail(Vector3 direction)
    {
        Vector3 alignDirection = (direction).normalized;
        SheepTransform.Translate(alignDirection * 3 * Time.deltaTime);
    }


    void HuntedSheepBehaviour()
    {
        //unaffected by physics while outside of trail
        if (SheepTransform.position.x < xBoundary + xDistanceFromBoundary && SheepTransform.position.x > -xBoundary - xDistanceFromBoundary)
        {
            sheepRb.isKinematic = false;
        }
        else
        {
            sheepRb.isKinematic = true;
        }

        // keep track of wolf
        foreach (GameObject wolf in _spawnManager.Pack)
        {
            float wolfProximityX = SheepTransform.position.x - wolf.transform.position.x;
            float wolfProximityZ = SheepTransform.position.z - wolf.transform.position.z;

            Vector3 wolfProximity = new Vector3(wolfProximityX, 0, wolfProximityZ);

            // flee wolf
            if (Mathf.Abs(wolfProximityX) < 4.5f && Mathf.Abs(wolfProximityZ) < 9.0f)
            {
                Avoid(wolfProximity, avoidSpeed*2);
            }
        }

        DestroyBoundaries(12, -13, 40, -40);
    }

    void DestroyBoundaries(float xBoundRight, float xBoundLeft, float zBoundForward, float zBoundBack)
    {
        // destroy if out of bounds
        if (SheepTransform.position.x > xBoundRight || SheepTransform.position.x < xBoundLeft)
        {
            PlayCollisionEffect();
            _audioManager.HasDetectedLostSheep = true;
            ReturnToPoolAndReset(gameObject);
        }
        if (SheepTransform.position.z > zBoundForward || SheepTransform.position.z < zBoundBack)
        {
            PlayCollisionEffect();
            _audioManager.HasDetectedLostSheep = true;
            ReturnToPoolAndReset(gameObject);
        }
    }

    private float CalculateDelay(List<GameObject> herd)
    {
        float originZ = zBackwardBoundary;
        float frontSheepPosZ = SheepTransform.position.z - originZ;

        // largest distance to generate delays for staggered jump
        foreach (GameObject sheep in herd)
        {
            if ((sheep.transform.position.z - originZ) > frontSheepPosZ)
            {
                frontSheepPosZ = sheep.transform.position.z - originZ;
            }
        }

        float delay = jumpDelayModifier * (frontSheepPosZ - (SheepTransform.position.z - originZ)) / (frontSheepPosZ);
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
        SheepTransform.Translate(alignDirection * speed * Time.deltaTime);
    }

    void FleeBark(Vector3 direction)
    {
        fleeDirection = (direction).normalized;
        float fleeDirectionX = fleeDirection.x;
        float fleeDirectionZ = fleeDirection.z;
        SheepTransform.Translate(new Vector3(fleeDirectionX, 0, fleeDirectionZ) * speedBurst * Time.deltaTime);
    }

    void PlayCollisionEffect()
    {
        GameObject sheepCollisionEffect = ObjectPoolUtility.Get(_spawnManager.SheepCollisionEffectAmountToPool, _spawnManager.SheepCollisionEffectPool);
        if (sheepCollisionEffect != null)
        {
            sheepCollisionEffect.transform.SetPositionAndRotation(SheepTransform.position, SheepTransform.rotation);
            sheepCollisionEffect.SetActive(true);
            _spawnManager.ActivateSheepCollisionEffect(sheepCollisionEffect);
        }
    }

    // collision
    public void OnCollision(GameObject collidingObject)
    {
        if (CompareTag("Sheep") && collidingObject.CompareTag("Obstacle"))
        {
            _audioManager.HasDetectedCollision = true;
            _audioManager.HasDetectedLostSheep = true;
            _spawnManager.TimeSinceLostSheep = 0;
            PlayCollisionEffect();
            ReturnToPoolAndReset(gameObject);
        }

        if (collidingObject.CompareTag("Trail Lane") || collidingObject.CompareTag("Boundary Lane"))
        {
            IsGrounded = true;
        }

        // hop away if landed on top of another sheep or player
        if (collidingObject.CompareTag("Sheep") || collidingObject.CompareTag("Player"))
        {
            if ((SheepTransform.position.y - collidingObject.transform.position.y) > heightTrigger)
            {
                Hop(jumpForce);
            }
        }

        // hop away if player lands ontop of sheep
        if (collidingObject.CompareTag("Player"))
        {
            if ((_sheepdog.PlayerTransform.position.y - SheepTransform.position.y) > heightTrigger && IsGrounded)
            {
                Hop(throwForce);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        ICollidable collidable = collision.gameObject.GetComponent<ICollidable>();
        if (collidable != null && !collidable.HasCollided)
        {
            collidable.OnCollision(this.gameObject);
        }
    }
}
