using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepController : BaseCharacterController, ISheepController, ICollidable
{
    public Transform SheepTransform { get; set; }

    // bark interaction
    private bool isBarkedAt;
    private bool isBarkedJumpAt;
    private Rigidbody sheepRb;

    public bool isSlowingDown = true;

    // interaction boundaries
    private float xBoundary = 7.3f;
    private float xAvoidDistance = 1.2f;
    private int zForwardBoundary = 15;
    private int zBackwardBoundary = -13;

    [SerializeField] private int avoidSpeed;

    private float hopDirectionX;
    private float hopDirectionY = 0.5f;
    private float hopDirectionZ = -0.2f;
    private Vector3 jumpDirection = Vector3.up;
    private float heightTrigger;

    // staggered jump
    public bool IsGrounded { get; set; }
    public bool isJumping;
    private float jumpDelayModifier = 3.2f;

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
    [SerializeField] private bool hasInitialisedSheep = false;

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
    [SerializeField] private Vector3 indicatorPositionOffset = new Vector3(0, 0, 0.5f);
    private Ray ray;
    private List<Collider> trackedCollidedList;
    private List<Collider> removeCollidedList;
    private bool isOutlined = false;
    private Vector3 sheepInteractivityIndicatorPosition;
    private Vector3 castPosition;

    Vector3 targetDirection;
    bool disableAvoidance;
    [SerializeField] Vector3 straySheepTargetDirection;
    bool hasInitialisedStraySheep;

    void Start()
    {
        sheepRb = GetComponent<Rigidbody>();

        // interactivity
        sheepInteractivityIndicator = InteractivityUtility.CreateInteractivityIndicator(SheepTransform, interactivityIndicator, indicatorMaterial, indicatorPositionOffset, interactionRange);
        sheepInteractivityIndicatorPosition = sheepInteractivityIndicator.transform.localPosition;
        sheepInteractivityIndicator.SetActive(false);

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

            castPosition = this.transform.position + indicatorPositionOffset + sheepInteractivityIndicatorPosition;
            trackedCollidedList = InteractivityUtility.CastRadius(SheepTransform, castPosition, trackedCollidedList, removeCollidedList, interactionRange);
        }
    }

    void InitialiseSheep()
    {
        SheepTransform = this.transform;
        sheepBodyAnim = SheepTransform.Find("sheep_body").GetComponent<Animator>();
        sheepHeadAnim = SheepTransform.Find("sheep_head").GetComponent<Animator>();

        targetFleeTime = 0.2f;
        targetFleeDistance = 3f;
        elapsedFleeTime = 0f;
        isFleeing = false;
        isJumping = false;
        isOutlined = false;
        isBarkedAt = false;
        isBarkedJumpAt = false;
        RemoveOutline();

        if (_spawnManager != null && CompareTag("Stray"))
        {
            straySheepTargetDirection = _spawnManager.StraySheepTargetPosition;
        }

        hasInitialisedSheep = true;
    }

    void InitialiseStraySheep()
    {
        InitialiseSheep();
        hasInitialisedStraySheep = true;
    }

    void ReturnToPoolAndReset(GameObject gameObject)
    {
        gameObject.tag = "Stray";
        hasInitialisedSheep = false;
        hasInitialisedStraySheep = false;
        trackedCollidedList.Clear();
        removeCollidedList.Clear();
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

    public override void AddOutline()
    {
        if (!isOutlined)
        {
            if(CompareTag("Sheep") || CompareTag("Stray"))
            {
                sheepInteractivityIndicator.SetActive(true);
                isOutlined = true;
            }
        }
    }

    public override void RemoveOutline()
    {
        if (isOutlined)
        {
            if (CompareTag("Sheep") || CompareTag("Stray") || CompareTag("Hunted"))
            {
                sheepInteractivityIndicator.SetActive(false);
                isOutlined = false;
            }
        }
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

    private void Update()
    {
        if (CompareTag("Sheep"))
        {
            if (SheepTransform.position.z > zForwardBoundary - xAvoidDistance || SheepTransform.position.z < zBackwardBoundary ||
                SheepTransform.position.x > xBoundary - xAvoidDistance || SheepTransform.position.x < -xBoundary + xAvoidDistance)
            {
                disableAvoidance = true;
            }
            else
            {
                if (disableAvoidance)
                {
                    disableAvoidance = false;
                }
            }
        }
    }

    void HerdSheepBehaviour()
    {
        // avoid player, other sheep, wolves
        AvoidOthers();

        // avoid being near boundary
        AvoidBoundary();

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

        // sheep is lost if allowed to drift back too far
        if (SheepTransform.position.z < zBackwardBoundary)
        {
            PlayCollisionEffect();
            _audioManager.HasDetectedLostSheep = true;
            _spawnManager.TimeSinceLostSheep = 0;
            _spawnManager.RemoveSheepFromHerd(gameObject);
            ReturnToPoolAndReset(gameObject);
        }
    }

    void StraySheepBehaviour()
    {
        //unaffected by physics while outside of trail
        if (SheepTransform.position.x < 10.8f && SheepTransform.position.x > -10.8f)
        {
            if (sheepRb.isKinematic)
            {
                sheepRb.isKinematic = false;
            }
            else
            {
                // avoid player, other sheep, wolves
                AvoidOthers();
                Jump(Vector3.up);
            }
        }
        else
        {
            if (!sheepRb.isKinematic)
            {
                sheepRb.isKinematic = true;
            }

            if (!hasInitialisedStraySheep)
            {
                InitialiseStraySheep();
            }
        }

        MovementUtility.Move(sheepRb, straySheepTargetDirection, 3f);

        // stray sheep added to herd with close proximity bark within trail
        if (isBarkedAt && SheepTransform.position.x < 6.2 && SheepTransform.position.x > -6.2)
        {
            isBarkedAt = false;
            _spawnManager.AddSheepToHerd(gameObject);
            _spawnManager.RemoveSheepFromStrays(gameObject);
            gameObject.tag = "Sheep";
        }

        // stray sheep is gone once beyond the forest boundary
        if (Mathf.Abs(straySheepTargetDirection.x - SheepTransform.position.x) <= 0.2)
        {
            _spawnManager.RemoveSheepFromStrays(gameObject);
            ReturnToPoolAndReset(gameObject);
        }
    }

    void HuntedSheepBehaviour()
    {
        //unaffected by physics while outside of trail
        if (SheepTransform.position.x < 10.8f && SheepTransform.position.x > -10.8f &&
            SheepTransform.position.z < 35f && SheepTransform.position.z > -13f)
        {
            RemoveOutline();
            if (sheepRb.isKinematic)
            {
                StopAllCoroutines();
                sheepRb.isKinematic = false;
            }
        }
        else
        {
            if (!sheepRb.isKinematic)
            {
                _spawnManager.RemoveSheepFromHunted(gameObject);
                sheepRb.isKinematic = true;
            }
        }

        // keep track of wolves
        foreach (Collider collidedCharacter in trackedCollidedList)
        {
            if (_spawnManager.Pack.Contains(collidedCharacter.gameObject))
            {
                targetDirection = InteractivityUtility.GetAwayDirection(sheepRb.position, collidedCharacter.attachedRigidbody.position);
                MovementUtility.MoveSmooth(sheepRb, targetDirection * 2f, 5f * 1.2f, 0.4f);
            }
        }

        AvoidObstacles();

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
        Jump(jumpDirection);
    }

    void Jump(Vector3 direction)
    {
        if (IsGrounded)
        {
            IsGrounded = false;
            isJumping = true;
            sheepBodyAnim.SetTrigger("isJumping");
            sheepHeadAnim.SetTrigger("isJumping");
            MovementUtility.Jump(sheepRb, direction, riseGravityScale, jumpHeight);
        }
    }

    void AvoidBoundary()
    {
        if (SheepTransform.position.z > zForwardBoundary - xAvoidDistance)
        {
            Jump(new Vector3(0, 1, -0.1f));
        }
        if (SheepTransform.position.x > xBoundary - xAvoidDistance)
        {
            Jump(new Vector3(-0.1f, 1, 0));
        }
        if (SheepTransform.position.x < -xBoundary + xAvoidDistance)
        {
            Jump(new Vector3(0.1f, 1, 0));
        }
    }

    void AvoidObstacles()
    {

        Vector3 raySource = new Vector3(sheepRb.position.x, 1f, sheepRb.position.z + 1f);
        Vector3 rayTarget = new Vector3(0, -0.2f, 1f);

        // check for obstacles in direction of movement
        ray = new Ray(raySource, rayTarget);
        float rangeMultiplier = 5f;

        if (Physics.Raycast(ray, rangeMultiplier, InteractivityUtility.obstacleMask))
        {
            // if obstacle ahead, jump
            Jump(Vector3.up);
        }

        //Debug.DrawRay(raySource, rayTarget * rangeMultiplier, Color.red, 1f);
    }

    void AvoidOthers()
    {
        // avoid sheep dog, avoidance takes priority
        if (trackedCollidedList.Contains(_sheepdog.PlayerCollider))
        {
            isSlowingDown = false;

            targetDirection = InteractivityUtility.GetAwayDirection(sheepRb.position, _sheepdog.PlayerRigidbody.position);
            MovementUtility.MoveSmooth(sheepRb, targetDirection, avoidSpeed, 1f);

            // bounce 
            if(_sheepdog.PlayerTransform.position.y > heightTrigger)
            {
                Jump(new Vector3(0, 0.5f, 0));
            }

            //Debug.DrawRay(sheepRb.position, targetDirection, Color.red, 1f);
        }
        // if fleeing, won't be avoiding others
        else if (!isFleeing && !disableAvoidance)
        {
            isSlowingDown = true;

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
                    if (collidedCharacter)
                    {
                        targetDirection = InteractivityUtility.GetAwayDirection(sheepRb.position, collidedCharacter.attachedRigidbody.position);
                        MovementUtility.MoveSmooth(sheepRb, targetDirection * 1.2f, 5f * 1.2f, 0.8f);

                        // check for interactive characters in direction of movement
                        ray = new Ray(sheepRb.position, targetDirection);
                        if (Physics.Raycast(ray, interactionRange * 1.5f, InteractivityUtility.interactiveMask))
                        {
                            // if interactive characters ahead, move faster and further than usual
                            MovementUtility.MoveSmooth(sheepRb, targetDirection * 1.1f, avoidSpeed * 1.1f, 0.8f);
                        }
                        else
                        {
                            // if no interactive characters ahead, move even fast and further than usual
                            MovementUtility.MoveSmooth(sheepRb, targetDirection * 1.2f, avoidSpeed * 1.2f, 0.5f);
                        }
                        //Debug.DrawRay(sheepRb.position, targetDirection, Color.red, 1f);
                    }
                }
            }
        }

        // sheep gradually falls behind
        if (isSlowingDown)
        {
            targetDirection = InteractivityUtility.GetTowardDirection(sheepRb.position, sheepRb.position + Vector3.back);
            MovementUtility.MoveSmooth(sheepRb, targetDirection, 2f, 0.1f);
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
            _spawnManager.RemoveSheepFromHerd(gameObject);
            ReturnToPoolAndReset(gameObject);
        }

        if (!isJumping)
        {
            if (collidingObject.CompareTag("Trail Lane") || collidingObject.CompareTag("Boundary Lane"))
            {
                IsGrounded = true;
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
