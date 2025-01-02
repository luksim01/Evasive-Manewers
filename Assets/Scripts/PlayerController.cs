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
    private InputAction move;
    private InputAction jump;
    private InputAction barkMove;
    private InputAction barkJump;
    private InputAction pause;
    Vector2 moveInput = Vector2.zero;
    Vector3 moveDirection;
    float verticalMoveSpeed;
    float horizontalMoveSpeed;

    // boundary control
    private readonly float xBoundary = 7.0f;
    private readonly float zBoundary = 15.0f;

    // movement settings : ground
    private readonly float forwardSpeed = 13.0f;
    private readonly float backwardSpeed = 9.0f;
    public readonly float sidewardSpeed = 10.0f;

    // movement settings : jump
    //private Rigidbody sheepdogRb;
    private readonly float jumpMovementSpeed = 0.4f;
    [SerializeField] private bool isGrounded = false;
    [SerializeField] private float jumpHeight = 6f;
    [SerializeField] private float riseGravityScale = 1f;
    [SerializeField] private float fallGravityScale = 2.5f;

    // bark control
    public bool HasBarkedMove { get; set; }
    public bool HasBarkedJump { get; set; }

    // bark particle 
    public ParticleSystem barkEffect;

    // collision with top of sheep
    [SerializeField] private float thrownSpeed = 6.0f;
    [SerializeField] private float heightTrigger = 2f;

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


    private void Awake()
    {
        PlayerTransform = this.transform;
        Health = 5;
        playerControls = new PlayerInputActions();
    }

    void OnEnable()
    {
        move = playerControls.Player.Move;
        move.Enable();

        jump = playerControls.Player.Jump;
        jump.Enable();
        jump.performed += Jump;

        barkMove = playerControls.Player.BarkMove;
        barkMove.Enable();
        barkMove.performed += BarkMove;

        barkJump = playerControls.Player.BarkJump;
        barkJump.Enable();
        barkJump.performed += BarkJump;

        pause = playerControls.Player.Pause;
        pause.Enable();
        pause.performed += Pause;
    }

    void OnDisable()
    {
        move.Disable();
        jump.Disable();
        barkMove.Disable();
        barkJump.Disable();
        pause.Disable();
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
        trackedCollidedList = new List<Collider>();
        removeCollidedList = new List<Collider>();
    }

    //void Update()
    //{
    //    MovementMonitor();
    //}

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_uiManager.IsGameActive)
        {
            MovementBoundaries(xBoundary, zBoundary);

            MovementControl(forwardSpeed, backwardSpeed, sidewardSpeed);

            CheckPlayerDeath();

            MovementUtility.Fall(PlayerRigidbody, riseGravityScale, fallGravityScale);

            if(interactionRange != previousInteractionRange)
            {
                InteractivityUtility.UpdateInteractivityIndicator(playerInteractivityIndicator, interactionRange);
            }
            previousInteractionRange = interactionRange;

            trackedCollidedList = InteractivityUtility.CastRadius(PlayerTransform, playerInteractivityIndicator.transform.position, trackedCollidedList, removeCollidedList, interactionRange);
        }
    }

    private void MovementMonitor()
    {
        moveInput = move.ReadValue<Vector2>();
    }

    private void MovementControl(float forwardSpeed, float backwardSpeed, float sidewardSpeed)
    {
        moveInput = move.ReadValue<Vector2>();

        // horizontal movement, slower movement while jumping
        if (moveInput.x != 0)
        {
            moveDirection = Vector3.right * moveInput.x;
            horizontalMoveSpeed = sidewardSpeed * (isGrounded ? 1 : jumpMovementSpeed);
            MovementUtility.Move(PlayerRigidbody, moveDirection, horizontalMoveSpeed);
        }

        if (moveInput.y > 0)
        {
            // forwards movement, slower movement while jumping
            moveDirection = Vector3.forward * moveInput.y;
            verticalMoveSpeed = forwardSpeed * (isGrounded ? 1 : jumpMovementSpeed);
            MovementUtility.Move(PlayerRigidbody, moveDirection, verticalMoveSpeed);
        }
        else if (moveInput.y < 0)
        {
            // backwards movement, slower movement while jumping
            moveDirection = Vector3.forward * moveInput.y;
            verticalMoveSpeed = backwardSpeed * (isGrounded ? 1 : jumpMovementSpeed);
            MovementUtility.Move(PlayerRigidbody, moveDirection, verticalMoveSpeed);
        }
    }

    //public void Move(Vector3 direction, float input, float speed)
    //{
    //    PlayerTransform.Translate(direction * input * Time.deltaTime * speed * (isGrounded ? 1 : jumpMovementSpeed));
    //}

    private void MovementBoundaries(float xBoundary, float zBoundary)
    {
        // player movement boundaries - left/right
        if (PlayerTransform.position.x < -xBoundary)
        {
            PlayerTransform.position = new Vector3(-xBoundary, PlayerTransform.position.y, PlayerTransform.position.z);
        }
        if (PlayerTransform.position.x > xBoundary)
        {
            PlayerTransform.position = new Vector3(xBoundary, PlayerTransform.position.y, PlayerTransform.position.z);
        }
        // player movement boundaries - forwards/backwards
        if (PlayerTransform.position.z < -zBoundary)
        {
            PlayerTransform.position = new Vector3(PlayerTransform.position.x, PlayerTransform.position.y, -zBoundary);
        }
        if (PlayerTransform.position.z > zBoundary)
        {
            PlayerTransform.position = new Vector3(PlayerTransform.position.x, PlayerTransform.position.y, zBoundary);
        }
    }

    private void CheckPlayerDeath()
    {
        // player death
        if (Health <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            isGrounded = false;
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
        if (collidingObject.CompareTag("Wolf") || collidingObject.CompareTag("Obstacle"))
        {
            _audioManager.HasDetectedCollision = true;
            PlaySheepdogCollisionEffect();
            Health -= 1;
            HasCollided = true;
            StartCoroutine(DamageCooldown(1f));
        }

        if (collidingObject.CompareTag("Trail Lane"))
        {
            isGrounded = true;
        }

        // player bounce on sheep - revisit make this trigger if excluding interactive layer collisions
        if (collidingObject.CompareTag("Sheep"))
        {
            SheepController sheepController = collidingObject.gameObject.GetComponent<SheepController>();
            dependancyManager.InjectSheepControllerDependancyIntoPlayerController(sheepController);
            if (PlayerTransform.position.y - _sheepController.SheepTransform.position.y > heightTrigger)
            {
                PlayerRigidbody.AddForce(Vector3.up * thrownSpeed, ForceMode.Impulse);
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

    private void BarkMove(InputAction.CallbackContext context)
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

    private void BarkJump(InputAction.CallbackContext context)
    {
        // keep track of herd to check they're grounded to trigger jump
        if (_spawnManager.CheckSheepGrounded())
        {
            sheepdogHeadAnim.SetTrigger("isBarkingJump");
            barkEffect.Play();
            _audioManager.HasDetectedBarkJump = true;
            StaggeredJump();
            StartCoroutine(BarkJumpCooldown(0.5f));
        }
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
}
