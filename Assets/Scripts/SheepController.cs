using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepController : BaseCharacterController, ISheepController, ICollidable
{
    public Transform SheepTransform { get; set; }

    // bark interaction
    private bool isBarkedAt;
    private bool isBarkedJumpAt;
    private bool pastBarkJumpState;
    private Rigidbody sheepRb;

    public bool isSlowingDown = false;

    // interaction boundaries
    private float xBoundary = 7.3f;
    private float xAvoidDistance = 1.2f;
    private int zForwardBoundary = 15;
    private int zAvoidDistance = 5;
    private int zBackwardBoundary = -32;

    // escape controls
    //private float jumpForce = 8.0f;
    private float throwForce = 8.0f;

    //private float sheepSlowdownSpeed = 0.5f;
    private float boundaryAvoidSpeed = 0.3f;
    [SerializeField] private int avoidSpeed;

    private float heightBoundary = 2.0f;

    private float hopDirectionX;
    private float hopDirectionY = 0.5f;
    private float hopDirectionZ = -0.2f;
    private Vector3 jumpDirection = new Vector3(0, 1, 0.1f);
    private float heightTrigger = 2.2f;

    // staggered jump
    public bool IsGrounded { get; set; }
    public bool isJumping;
    private float jumpDelayModifier = 7.5f;

    // player proximity
    [SerializeField] private Vector3 sheepDogProximity;
    
    // stray sheep
    public bool isHerdSheep;
    //private float xDistanceFromBoundary = 1.5f;

    [SerializeField] private float jumpHeight = 6f;
    [SerializeField] private float riseGravityScale = 2.8f;
    [SerializeField] private float fallGravityScale = 0.9f;

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
    public override void SetDependencies(IAudioManager audioManager, IUIManager uiManager, ISpawnManager spawnManager, IPlayerController playerController)
    {
        _audioManager = audioManager;
        _uiManager = uiManager;
        _spawnManager = spawnManager;
        _sheepdog = playerController;
    }

    // collision
    public bool HasCollided { get; set; }

    // interactivity
    [SerializeField] private float targetFleeTime;
    [SerializeField] private float targetFleeDistance;
    [SerializeField] private float elapsedFleeTime;
    [SerializeField] private bool isFleeing;
    [SerializeField] private Vector3 startFleePosition;
    [SerializeField] private Vector3 targetFleePosition;

    [SerializeField] private float smoothTime;

    // interactivity
    [SerializeField] private float interactionRange;
    private float previousInteractionRange;
    private GameObject sheepInteractivityIndicator;
    public GameObject interactivityIndicator;
    public Material indicatorMaterial;
    [SerializeField] private Vector3 indicatorPositionOffset;
    private Ray ray;
    private List<Collider> trackedCollidedList;
    private List<Collider> removeCollidedList;

    Vector3 targetDirection;

    void Start()
    {
        sheepRb = GetComponent<Rigidbody>();

        // interactivity
        sheepInteractivityIndicator = InteractivityUtility.CreateInteractivityIndicator(SheepTransform, interactivityIndicator, indicatorMaterial, indicatorPositionOffset, interactionRange);
        trackedCollidedList = new List<Collider>();
        removeCollidedList = new List<Collider>();
    }

    void OnEnable()
    {
        InitialiseSheep();
    }

    void FixedUpdate()
    {
        if (_uiManager.IsGameActive && hasInitialisedSheep)
        {
            // sheep behaviour based on tag
            DetermineSheepBehaviour();

            MovementUtility.Fall(sheepRb, riseGravityScale, fallGravityScale);

            if (sheepRb.velocity.y < 0)
            {
                isJumping = false;
            }

            if (interactionRange != previousInteractionRange)
            {
                InteractivityUtility.UpdateInteractivityIndicator(sheepInteractivityIndicator, interactionRange);
            }
            previousInteractionRange = interactionRange;

            trackedCollidedList = InteractivityUtility.CastRadius(SheepTransform, sheepInteractivityIndicator.transform.position, trackedCollidedList, removeCollidedList, interactionRange);
        }
    }

    void InitialiseSheep()
    {
        SheepTransform = this.transform;
        sheepBodyAnim = SheepTransform.Find("sheep_body").GetComponent<Animator>();
        sheepHeadAnim = SheepTransform.Find("sheep_head").GetComponent<Animator>();

        targetFleeTime = 0.2f;
        targetFleeDistance = 2f;
        elapsedFleeTime = 0f;
        isFleeing = false;
        isJumping = false;

        hasInitialisedSheep = true;
    }

    void ReturnToPoolAndReset(GameObject gameObject)
    {
        gameObject.tag = "Stray";
        _spawnManager.RemoveSheepFromHerd(gameObject);
        ObjectPoolUtility.Return(gameObject);
    }

    public override void Interact()
    {
        isBarkedAt = true;
    }

    public void InteractJump()
    {
        isBarkedJumpAt = true;
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
        // avoid player, other sheep, wolves
        AvoidOthers();

        //else if (isSlowingDown)
        //{
        //    // sheep gradually falls behind
        //    SheepTransform.Translate(Vector3.back * sheepSlowdownSpeed * Time.deltaTime);
        //}

        // sheep flee bark if within certain distance of it
        if (isBarkedAt)
        {
            isBarkedAt = false;
            isFleeing = true;
            startFleePosition = SheepTransform.position;
            targetDirection = InteractivityUtility.GetAwayDirection(sheepRb.position, _sheepdog.PlayerRigidbody.position);
            targetFleePosition = startFleePosition + targetDirection.normalized * targetFleeDistance;
        }

        if (isFleeing)
        {
            targetDirection = InteractivityUtility.GetAwayDirection(sheepRb.position, _sheepdog.PlayerRigidbody.position);
            elapsedFleeTime = MovementUtility.Flee(sheepRb, startFleePosition, targetDirection, targetFleeDistance, targetFleeTime, targetFleePosition, elapsedFleeTime);

            if(SheepTransform.position == targetFleePosition || elapsedFleeTime >= targetFleeTime)
            {
                isFleeing = false;
                elapsedFleeTime = 0f;
            }
        }

        // stagger jump of herd
        if (isBarkedJumpAt)
        {
            isBarkedJumpAt = false;
            float jumpDelay = CalculateDelay(_spawnManager.Herd);
            StartCoroutine(StaggeredJump(jumpDelay));
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
        // avoid player, other sheep
        AvoidOthers();

        //unaffected by physics while outside of trail
        if (SheepTransform.position.x < 10.8f && SheepTransform.position.x > -10.8f)
        {
            if (sheepRb.isKinematic)
            {
                sheepRb.isKinematic = false;
            }
            Jump(Vector3.up);
        }
        else
        {
            if (!sheepRb.isKinematic)
            {
                sheepRb.isKinematic = true;
            }
        }

        // movement across trail - revisit tidy up StraySheepTargetDirection OnEnable
        Vector3 spawnPosition = _spawnManager.StraySheepSpawnPosition;
        Vector3 targetPosition = _spawnManager.StraySheepTargetPosition;

        Vector3 targetDirection = targetPosition - spawnPosition;

        MovementUtility.Move(sheepRb, targetDirection, 3f);

        // stray sheep added to herd with close proximity bark within trail
        if (isBarkedAt && SheepTransform.position.x < 6.2 && SheepTransform.position.x > -6.2)
        {
            isBarkedAt = false;
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

    void HuntedSheepBehaviour()
    {
        //unaffected by physics while outside of trail
        if (SheepTransform.position.x < 10.8f && SheepTransform.position.x > -10.8f)
        {
            if (sheepRb.isKinematic)
            {
                sheepRb.isKinematic = false;
            }
        }
        else
        {
            if (!sheepRb.isKinematic)
            {
                sheepRb.isKinematic = true;
            }
        }

        // keep track of wolves
        foreach (Collider collidedCharacter in trackedCollidedList)
        {
            if (_spawnManager.Pack.Contains(collidedCharacter.gameObject))
            {
                targetDirection = InteractivityUtility.GetAwayDirection(sheepRb.position, collidedCharacter.attachedRigidbody.position);
                MovementUtility.MoveSmooth(sheepRb, targetDirection * 1.2f, 5f * 1.2f, 0.8f);
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
        //Debug.Log($"(Hop) is grounded? : {IsGrounded}");
    }

    IEnumerator StaggeredJump(float delay)
    {
        yield return new WaitForSeconds(delay);
        Jump(jumpDirection);
    }

    void Jump(Vector3 direction)
    {
        if (IsGrounded)
        {
            IsGrounded = false;
            isJumping = true;
            //Debug.Log($"(Jump) is grounded? : {IsGrounded}");
            sheepBodyAnim.SetTrigger("isJumping");
            sheepHeadAnim.SetTrigger("isJumping");
            MovementUtility.Jump(sheepRb, direction, riseGravityScale, jumpHeight);
        }
    }

    void AvoidOthers()
    {
        // avoid sheep dog, avoidance takes priority
        if (trackedCollidedList.Contains(_sheepdog.PlayerCollider))
        {
            Vector3 selfPosition = new Vector3(sheepRb.position.x, 0, sheepRb.position.z);
            Vector3 playerPosition = _sheepdog.PlayerRigidbody.position;
            Vector3 direction = selfPosition - new Vector3(playerPosition.x, 0, playerPosition.z);

            MovementUtility.MoveSmooth(sheepRb, direction, avoidSpeed, 1f);
            //Debug.DrawRay(selfPosition, direction, Color.red, 1f);
        }
        // if fleeing, won't be avoiding others
        else if (!isFleeing)
        {
            // all characters in interactive area
            foreach (Collider collidedCharacter in trackedCollidedList)
            {
                // if wolf, jumping and won't be avoiding others
                if (_spawnManager.Pack.Contains(collidedCharacter.gameObject))
                {
                    Jump(Vector3.up);
                }
                else
                {
                    targetDirection = InteractivityUtility.GetAwayDirection(sheepRb.position, collidedCharacter.attachedRigidbody.position);
                    MovementUtility.MoveSmooth(sheepRb, targetDirection * 1.2f, 5f * 1.2f, 0.8f);

                    // check for interactive characters in direction of movement
                    ray = new Ray(sheepRb.position, targetDirection);
                    if (Physics.Raycast(ray, interactionRange * 1.5f, InteractivityUtility.mask))
                    {
                        // if interactive characters ahead, move faster and further than usual
                        MovementUtility.MoveSmooth(sheepRb, targetDirection * 1.1f, avoidSpeed * 1.1f, 0.8f);
                    }
                    else
                    {
                        // if no interactive characters ahead, move even fast and further than usual
                        MovementUtility.MoveSmooth(sheepRb, targetDirection * 1.2f, avoidSpeed * 1.2f, 0.5f);
                    }

                    //Debug.DrawRay(selfPosition, direction, Color.red, 1f);
                }
            }
        }
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

        if (!isJumping)
        {
            if (collidingObject.CompareTag("Trail Lane") || collidingObject.CompareTag("Boundary Lane"))
            {
                IsGrounded = true;
                //Debug.Log($"(OnCollision) is grounded? : {IsGrounded}");
            }
        }

        // hop away if player lands ontop of sheep - revisit make this trigger if excluding interactive layer collisions
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

    private void OnTriggerEnter(Collider other)
    {
        ICollidable collidable = other.gameObject.GetComponent<ICollidable>();
        if (collidable != null && !collidable.HasCollided)
        {
            collidable.OnCollision(this.gameObject);
        }
    }
}
