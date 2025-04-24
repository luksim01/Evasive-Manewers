using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IPlayerController, ICollidable
{
    // self
    public Transform PlayerTransform { get; set; }
    public Rigidbody PlayerRigidbody { get; set; }
    public Collider PlayerCollider { get; set; }
    public int Health { get; set; }

    // inputs
    public PlayerInputActions playerControls;
    public InputAction Move { get; set; }
    public InputAction Jump { get; set; }
    public InputAction BarkMove { get; set; }
    public InputAction BarkJump { get; set; }
    private InputAction pause { get; set; }
    private InputAction exit { get; set; }
    [SerializeField] private Vector2 moveInput = Vector2.zero;
    Vector3 moveDirection;
    float verticalMoveSpeed;
    float horizontalMoveSpeed;

    // boundary control
    private readonly float xBoundary = 7.0f;
    private readonly float zBoundary = 15.0f;

    // movement settings : ground
    private readonly float speed = 10f;

    // movement settings : jump
    //private Rigidbody sheepdogRb;
    private readonly float jumpMovementSpeed = 0.4f;
    public bool IsGrounded { get; set; }
    [SerializeField] private float jumpHeight = 6f;
    [SerializeField] private float riseGravityScale = 1f;
    [SerializeField] private float fallGravityScale = 2.5f;

    // bark control
    public bool HasBarkedMove { get; set; }
    public bool HasBarkedJump { get; set; }

    // bark particle 
    public ParticleSystem barkEffect;
    public ParticleSystem barkJumpEffect;

    // animation
    private Animator sheepdogBodyAnim;
    private Animator sheepdogHeadAnim;

    // particle
    public GameObject sheepdogCollisionEffect;
    private List<GameObject> collisionEffectPool;
    private int collisionEffectAmountToPool = 2;

    // audio
    private IAudioManager _audioManager;

    // ui
    private IUIManager _uiManager;

    // spawn manager
    private ISpawnManager _spawnManager;

    // wolf
    private WolfController _wolfController;

    // sheep
    private SheepController _sheepController;

    // dependancies
    public void SetDependencies(IAudioManager audioManager, IUIManager uiManager, ISpawnManager spawnManager)
    {
        _audioManager = audioManager;
        _uiManager = uiManager;
        _spawnManager = spawnManager;
    }

    public void SetWolfDependancy(WolfController wolfController)
    {
        _wolfController = wolfController;
    }

    public void SetSheepDependancy(SheepController sheepController)
    {
        _sheepController = sheepController;
    }

    // dependancy manager
    [SerializeField] private DependancyManager dependancyManager;

    // collision
    public bool HasCollided { get; set; }

    // interactivity
    [SerializeField] private float interactionRange;
    private float previousInteractionRange;
    private GameObject playerInteractivityIndicator;
    public GameObject interactivityIndicator;
    public Material indicatorMaterial;
    [SerializeField] private Vector3 indicatorPositionOffset;
    private List<Collider> trackedCollidedList;
    private List<Collider> removeCollidedList;
    private List<Collider> historicalTrackedCollidedList;
    private List<Collider> removeOutlineList;

    [SerializeField] private Vector3 targetDirection;
    [SerializeField] private float repositionSpeed;

    private Vector3 playerInteractivityIndicatorPosition;
    private Vector3 castPosition;

    private bool disableForwardMovement = false;
    private bool disableBackwardMovement = false;
    private bool disableRightMovement = false;
    private bool disableLeftMovement = false;
    private float moveTolerance = 0.3f;

    private void Awake()
    {
        PlayerTransform = this.transform;
        Health = 5;
        playerControls = new PlayerInputActions();
    }

    void OnEnable()
    {
        Move = playerControls.Player.Move;
        Move.Enable();

        Jump = playerControls.Player.Jump;
        Jump.Enable();
        Jump.performed += DoJump;

        BarkMove = playerControls.Player.BarkMove;
        BarkMove.Enable();
        BarkMove.performed += DoBarkMove;

        BarkJump = playerControls.Player.BarkJump;
        BarkJump.Enable();
        BarkJump.performed += DoBarkJump;

        pause = playerControls.Player.Pause;
        pause.Enable();
        pause.performed += Pause;

        exit = playerControls.Player.Exit;
        exit.Enable();
        exit.performed += Exit;
    }

    void OnDisable()
    {
        Move.Disable();
        Jump.Disable();
        BarkMove.Disable();
        BarkJump.Disable();
        pause.Disable();
        exit.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerRigidbody = GetComponent<Rigidbody>();
        PlayerRigidbody.useGravity = false;

        PlayerCollider = GetComponent<Collider>();

        sheepdogBodyAnim = PlayerTransform.Find("sheepdog_body").GetComponent<Animator>();
        sheepdogHeadAnim = PlayerTransform.Find("sheepdog_head").GetComponent<Animator>();

        // creating a pool of collision effects
        collisionEffectPool = ObjectPoolUtility.Create("DogCollisionPool", PlayerTransform, sheepdogCollisionEffect, collisionEffectAmountToPool);

        // interactivity
        playerInteractivityIndicator = InteractivityUtility.CreateInteractivityIndicator(PlayerTransform, interactivityIndicator, indicatorMaterial, indicatorPositionOffset, interactionRange);
        playerInteractivityIndicatorPosition = playerInteractivityIndicator.transform.localPosition;
        playerInteractivityIndicator.SetActive(false);
        trackedCollidedList = new List<Collider>();
        removeCollidedList = new List<Collider>();
        historicalTrackedCollidedList = new List<Collider>();
        removeOutlineList = new List<Collider>();
    }

    void Update()
    {
        MovementMonitor();
        //if (PlayerTransform.position.z > zBoundary || PlayerTransform.position.z < -zBoundary ||
        //    PlayerTransform.position.x > xBoundary || PlayerTransform.position.x < -xBoundary)
        //{
        //    PlayerTransform.position = MovementUtility.MovementConstraints(PlayerTransform.position, zBoundary, -zBoundary, xBoundary, -xBoundary);
        //}

        if(PlayerTransform.position.z > zBoundary)
        {
            disableForwardMovement = true;
        }
        else
        {
            if (disableForwardMovement)
            {
                disableForwardMovement = false;
            }
        }

        if (PlayerTransform.position.z < -zBoundary)
        {
            disableBackwardMovement = true;
        }
        else
        {
            if (disableBackwardMovement)
            {
                disableBackwardMovement = false;
            }
        }

        if (PlayerTransform.position.x > xBoundary)
        {
            disableRightMovement = true;
        }
        else
        {
            if (disableRightMovement)
            {
                disableRightMovement = false;
            }
        }

        if (PlayerTransform.position.x < -xBoundary)
        {
            disableLeftMovement = true;
        }
        else
        {
            if (disableLeftMovement)
            {
                disableLeftMovement = false;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_uiManager.IsGameActive)
        {
            //CheckWithinBounds(zBoundary, -zBoundary, xBoundary, -xBoundary);
            MovementControl(speed);

            CheckPlayerDeath();

            MovementUtility.Fall(PlayerRigidbody, riseGravityScale, fallGravityScale);

            castPosition = this.transform.position + indicatorPositionOffset + playerInteractivityIndicatorPosition;
            trackedCollidedList = InteractivityUtility.CastRadius(PlayerTransform, castPosition, trackedCollidedList, removeCollidedList, interactionRange);

            OutlineInteractiveCharacters();
        }
    }

    private void MovementMonitor()
    {
        moveInput = Move.ReadValue<Vector2>();
        
        if (disableBackwardMovement && moveInput.y < 0)
        {
            moveInput.y = 0f;
            if (moveInput.x < moveTolerance && moveInput.x > -moveTolerance)
            {
                moveInput.x = 0f;
            }
        }

        if (disableForwardMovement && moveInput.y > 0)
        {
            moveInput.y = 0f;
            if (moveInput.x < moveTolerance && moveInput.x > -moveTolerance)
            {
                moveInput.x = 0f;
            }
        }

        if (disableLeftMovement && moveInput.x < 0)
        {
            moveInput.x = 0f;
            if (moveInput.y < moveTolerance && moveInput.y > -moveTolerance)
            {
                moveInput.y = 0f;
            }
        }

        if (disableRightMovement && moveInput.x > 0)
        {
            moveInput.x = 0f;
            if (moveInput.y < moveTolerance && moveInput.y > -moveTolerance)
            {
                moveInput.y = 0f;
            }
        }
    }

    private void MovementControl(float speed)
    {
        MovementUtility.Move(PlayerRigidbody, new Vector3(moveInput.x, 0, moveInput.y), speed * (IsGrounded ? 1 : jumpMovementSpeed));
    }

    private void CheckPlayerDeath()
    {
        // player death
        if (Health <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void DoJump(InputAction.CallbackContext context)
    {
        if (IsGrounded)
        {
            IsGrounded = false;
            sheepdogBodyAnim.SetTrigger("isJumping");
            sheepdogHeadAnim.SetTrigger("isJumping");
            MovementUtility.Jump(PlayerRigidbody, Vector3.up, riseGravityScale, jumpHeight);
        }
    }

    // Coroutine to wait for bark to cool down
    IEnumerator BarkMoveCooldown(float barkCooldownTime)
    {
        yield return new WaitForSeconds(barkCooldownTime);
        HasBarkedMove = false;
    }

    // Coroutine to wait for bark to cool down
    IEnumerator BarkJumpCooldown(float barkJumpCooldownTime)
    {
        yield return new WaitForSeconds(barkJumpCooldownTime);
        HasBarkedJump = false;
    }

    // Coroutine to wait for grace period to cool down
    IEnumerator DamageCooldown(float damageCooldownTime)
    {
        yield return new WaitForSeconds(damageCooldownTime);
        HasCollided = false;
    }

    IEnumerator CollisionEffectDuration(GameObject gameObject, float durationTime)
    {
        yield return new WaitForSeconds(durationTime);
        ObjectPoolUtility.Return(gameObject);
    }

    void PlaySheepdogCollisionEffect()
    {
        GameObject sheepdogCollisionEffect = ObjectPoolUtility.Get(collisionEffectAmountToPool, collisionEffectPool);
        if (sheepdogCollisionEffect != null)
        {
            sheepdogCollisionEffect.transform.SetPositionAndRotation(PlayerTransform.position, PlayerTransform.rotation);
            sheepdogCollisionEffect.SetActive(true);
            StartCoroutine(CollisionEffectDuration(sheepdogCollisionEffect, 2f));
        }
    }

    public void OnCollision(GameObject collidingObject)
    {
        if (collidingObject.CompareTag("WolfHuntingSheep") || collidingObject.CompareTag("WolfHuntingDog") || collidingObject.CompareTag("Obstacle"))
        {
            _audioManager.HasDetectedCollision = true;
            PlaySheepdogCollisionEffect();
            Health -= 1;
            _spawnManager.DecreaseGameSpeed();
            //UserTestManager.instance.SaveUserEventData("Health Lost");
            HasCollided = true;
            StartCoroutine(DamageCooldown(1f));
        }

        if (collidingObject.CompareTag("Trail Lane"))
        {
            IsGrounded = true;
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

    private void DoBarkMove(InputAction.CallbackContext context)
    {
        if (!HasBarkedMove)
        {
            HasBarkedMove = true;
            sheepdogHeadAnim.SetTrigger("isBarkingMove");
            barkEffect.Play();
            _audioManager.HasDetectedBarkMove = true;
            InteractInZone();
            StartCoroutine(BarkMoveCooldown(0.5f));
        }
    }

    private void InteractInZone()
    {
        foreach (Collider collidedCharacter in trackedCollidedList)
        {
            BaseCharacterController interactiveCharacterController = collidedCharacter.GetComponent<BaseCharacterController>();
            interactiveCharacterController.Interact();
        }
    }

    private void OutlineInteractiveCharacters()
    {
        foreach (Collider collidedCharacter in trackedCollidedList)
        {
            BaseCharacterController interactiveCharacterController = collidedCharacter.GetComponent<BaseCharacterController>();
            interactiveCharacterController.AddOutline();
        }

        foreach (Collider historicalCollidedCharacter in historicalTrackedCollidedList)
        {
            if (!trackedCollidedList.Contains(historicalCollidedCharacter))
            {
                BaseCharacterController interactiveCharacterController = historicalCollidedCharacter.GetComponent<BaseCharacterController>();
                interactiveCharacterController.RemoveOutline();
            }
        }

        if(historicalTrackedCollidedList.Count != trackedCollidedList.Count)
        {
            historicalTrackedCollidedList.Clear();
            historicalTrackedCollidedList.AddRange(trackedCollidedList);
        }
    }

    private void DoBarkJump(InputAction.CallbackContext context)
    {
        // keep track of herd to check they're grounded to trigger jump
        sheepdogHeadAnim.SetTrigger("isBarkingJump");
        barkJumpEffect.Play();
        _audioManager.HasDetectedBarkJump = true;
        StaggeredJump();
        StartCoroutine(BarkJumpCooldown(0.5f));
    }

    private void StaggeredJump()
    {
        foreach (GameObject sheep in _spawnManager.Herd)
        {
            SheepController sheepController = sheep.GetComponent<SheepController>();
            sheepController.InteractJump();
        }
    }

    private void Pause(InputAction.CallbackContext context)
    {
        _uiManager.PauseResume();
    }

    private void Exit(InputAction.CallbackContext context)
    {
        _uiManager.Exit();
    }
}
