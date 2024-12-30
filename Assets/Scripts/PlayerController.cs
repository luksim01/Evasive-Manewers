using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IPlayerController, ICollidable
{
    // self
    public Transform PlayerTransform { get; set; }
    public int Health { get; set; }

    // inputs
    public PlayerInputActions playerControls;
    private InputAction move;
    private InputAction jump;
    private InputAction barkMove;
    private InputAction barkJump;
    private InputAction pause;
    Vector2 moveDirection = Vector2.zero;

    // boundary control
    private readonly float xBoundary = 7.0f;
    private readonly float zBoundary = 15.0f;

    // movement settings : ground
    private readonly float forwardSpeed = 9.0f;
    private readonly float backwardSpeed = 6.0f;
    public readonly float sidewardSpeed = 7.0f;

    // movement settings : jump
    private Rigidbody sheepdogRb;
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
    private Ray ray;
    [SerializeField] private Vector3 playerCenter;
    [SerializeField] private float rayHeightOffset = 1.5f;
    [SerializeField] private float rayRange = 3.5f;
    [SerializeField] private float rayRangeDiag = 2.5f;
    [SerializeField] private float interactionRange = 5f;
    private float previousInteractionRange;
    private GameObject playerInteractivityIndicator;
    public GameObject interactivityIndicator;
    private GameObject interactivitySphereIndicator;
    private GameObject interactivityRingIndicator;
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
        sheepdogRb = GetComponent<Rigidbody>();
        sheepdogRb.useGravity = false;

        sheepdogBodyAnim = PlayerTransform.Find("sheepdog_body").GetComponent<Animator>();
        sheepdogHeadAnim = PlayerTransform.Find("sheepdog_head").GetComponent<Animator>();

        // creating a pool of collision effects
        collisionEffectPool = ObjectPoolUtility.Create("DogCollisionPool", PlayerTransform, sheepdogCollisionEffect, collisionEffectAmountToPool);

        // interactivity
        CreateInteractivityIndicator(interactionRange);
        trackedCollidedList = new List<Collider>();
        removeCollidedList = new List<Collider>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_uiManager.IsGameActive)
        {
            MovementBoundaries(xBoundary, zBoundary);

            MovementControl(forwardSpeed, backwardSpeed, sidewardSpeed);

            CheckPlayerDeath();

            MovementUtility.Fall(sheepdogRb, riseGravityScale, fallGravityScale);

            if(interactionRange != previousInteractionRange)
            {
                UpdateInteractivityIndicator(playerInteractivityIndicator, interactionRange);
            }
            previousInteractionRange = interactionRange;

            CastRadius(interactionRange);

        }
    }

    private void CreateInteractivityIndicator(float range)
    {
        playerInteractivityIndicator = Object.Instantiate(interactivityIndicator, interactivityIndicator.transform.position, interactivityIndicator.transform.rotation);
        playerInteractivityIndicator.transform.localScale = Vector3.one * range * 2;
        playerInteractivityIndicator.gameObject.transform.parent = PlayerTransform;

        interactivitySphereIndicator = playerInteractivityIndicator.transform.Find("Sphere").gameObject;
        interactivityRingIndicator = playerInteractivityIndicator.transform.Find("Ring").gameObject;
    }

    private void UpdateInteractivityIndicator(GameObject interactivityIndicator, float range)
    {
        interactivityIndicator.transform.localScale = Vector3.one * range * 2;
    }

    public void CastRadius(float range)
    {
        int includeLayer = LayerMask.NameToLayer("Interactive");
        int mask = (1 << includeLayer);

        Collider[] collidedCharacterArray = Physics.OverlapSphere(playerInteractivityIndicator.transform.position, range, mask);

        foreach (Collider collidedCharacter in collidedCharacterArray)
        {
            // check that the interactive character isn't self and isn't already in tracked list
            if (!trackedCollidedList.Contains(collidedCharacter) && collidedCharacter.transform != PlayerTransform)
            {
                //Debug.Log($"{collidedCharacter.gameObject.name} is added to tracked list.");
                trackedCollidedList.Add(collidedCharacter);
            }
        }

        foreach (Collider collidedCharacter in trackedCollidedList)
        {
            // check that the interactive character hasn't disappeared from character list
            bool isPresent = false;

            for (int i = 0; i < collidedCharacterArray.Length; i++)
            {
                if (collidedCharacter == collidedCharacterArray[i])
                {
                    isPresent = true;
                    break;
                }
            }

            if (!isPresent)
            {
                removeCollidedList.Add(collidedCharacter);
            }
        }

        if(removeCollidedList.Count > 0)
        {
            foreach (Collider collidedCharacter in removeCollidedList)
            {
                //Debug.Log($"{collidedCharacter.gameObject.name} is removed from tracked list.");
                trackedCollidedList.Remove(collidedCharacter);
            }

            removeCollidedList.Clear();
        }
    }

    private void CheckSurroundings()
    {
        playerCenter = PlayerTransform.position + new Vector3(0f, rayHeightOffset, 0f);

        // forward, backward
        Debug.DrawRay(playerCenter, new Vector3(0f, 0f, rayRange), Color.red);
        Debug.DrawRay(playerCenter, new Vector3(0f, 0f, -rayRange), Color.red);

        // right, left
        Debug.DrawRay(playerCenter, new Vector3(rayRange, 0f, 0f), Color.red);
        Debug.DrawRay(playerCenter, new Vector3(-rayRange, 0f, 0f), Color.red);

        // diagonal forward
        Debug.DrawRay(playerCenter, new Vector3(rayRangeDiag, 0f, rayRangeDiag), Color.red);
        Debug.DrawRay(playerCenter, new Vector3(-rayRangeDiag, 0f, rayRangeDiag), Color.red);

        // diagonal backward
        Debug.DrawRay(playerCenter, new Vector3(rayRangeDiag, 0f, -rayRangeDiag), Color.red);
        Debug.DrawRay(playerCenter, new Vector3(-rayRangeDiag, 0f, -rayRangeDiag), Color.red);

        //ray = new Ray(playerCenter, new Vector3(0f, 0f, rayRange));

        //if (Physics.Raycast(ray, out RaycastHit hit, rayRange))
        //{
        //    Debug.Log($"{hit.collider.name} was hit by ray.");
        //}
    }

    private void MovementControl(float forwardSpeed, float backwardSpeed, float sidewardSpeed)
    {
        moveDirection = move.ReadValue<Vector2>();

        // horizontal movement, slower movement while jumping
        Move(Vector3.right, moveDirection.x, sidewardSpeed);
        
        if(moveDirection.y > 0)
        {
            // forwards movement, slower movement while jumping
            Move(Vector3.forward, moveDirection.y, forwardSpeed);
        }
        else if (moveDirection.y < 0)
        {
            // backwards movement, slower movement while jumping
            Move(Vector3.forward, moveDirection.y, backwardSpeed);
        }
    }

    public void Move(Vector3 direction, float input, float speed)
    {
        PlayerTransform.Translate(direction * input * Time.deltaTime * speed * (isGrounded ? 1 : jumpMovementSpeed));
    }

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
            MovementUtility.Jump(sheepdogRb, Vector3.up, riseGravityScale, jumpHeight);
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

        if (collidingObject.CompareTag("Sheep"))
        {
            SheepController sheepController = collidingObject.gameObject.GetComponent<SheepController>();
            dependancyManager.InjectSheepControllerDependancyIntoPlayerController(sheepController);
            if (PlayerTransform.position.y - _sheepController.SheepTransform.position.y > heightTrigger)
            {
                sheepdogRb.AddForce(Vector3.up * thrownSpeed, ForceMode.Impulse);
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

    private void BarkMove(InputAction.CallbackContext context)
    {
        if (!HasBarkedMove)
        {
            HasBarkedMove = true;
            sheepdogHeadAnim.SetTrigger("isBarkingMove");
            barkEffect.Play();
            _audioManager.HasDetectedBarkMove = true;
            InteractInZone();
            StartCoroutine(BarkMoveCooldown(1.0f));
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
        if (!HasBarkedJump && _spawnManager.CheckSheepGrounded())
        {
            HasBarkedJump = true;
            sheepdogHeadAnim.SetTrigger("isBarkingJump");
            barkEffect.Play();
            _audioManager.HasDetectedBarkJump = true;
            StartCoroutine(BarkJumpCooldown(1.0f));
        }
    }

    private void Pause(InputAction.CallbackContext context)
    {
        _uiManager.PauseResume();
    }
}
