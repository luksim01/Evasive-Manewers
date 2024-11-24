using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayerController
{
    // self
    public Transform PlayerTransform { get; set; }
    public int Health { get; set; }
    public float BarkInteractionRadius { get; set; }
    [SerializeField] private GameObject barkInteractionIndicator;

    // inputs
    private float horizontalInput;
    private float forwardInput;
    private float backwardInput;
    private bool jumpInput;
    private bool barkMoveInput;
    private bool barkJumpInput;

    // boundary control
    private readonly float xBoundary = 7.0f;
    private readonly float zBoundary = 15.0f;

    // movement settings : ground
    private readonly float forwardSpeed = 9.0f;
    private readonly float backwardSpeed = 6.0f;
    public readonly float sidewardSpeed = 7.0f;
    // movement settings : jump
    private Rigidbody sheepdogRb;
    private readonly float jumpForce = 9.0f;
    private readonly float jumpMovementSpeed = 0.4f;
    [SerializeField] private bool isGrounded = false;

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

    // audio
    private IAudioManager _audioManager;

    // ui
    private bool isGameActive;
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

    private void Awake()
    {
        PlayerTransform = this.transform;
        Health = 5;
    }

    // Start is called before the first frame update
    void Start()
    {
        BarkInteractionRadius = 3f;
        barkInteractionIndicator = CreateBarkInteractionIndicator();

        sheepdogRb = GetComponent<Rigidbody>();

        sheepdogBodyAnim = this.transform.Find("sheepdog_body").GetComponent<Animator>();
        sheepdogHeadAnim = this.transform.Find("sheepdog_head").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        isGameActive = _uiManager.IsGameActive;

        if (isGameActive)
        {
            MovementBoundaries(xBoundary, zBoundary);

            MovementControl(forwardSpeed, backwardSpeed, sidewardSpeed, jumpForce, jumpMovementSpeed);

            CheckBarkCommand();

            CheckPlayerDeath();

            ManageInteractorVision();
        }

        PlayerTransform = transform;
    }

    private void ManageInteractorVision()
    {
        CastRadius(barkInteractionIndicator);
    }

    public GameObject CreateBarkInteractionIndicator()
    {
        GameObject barkInteractionIndicator = new GameObject();
        GameObject barkInteractionIndicatorPrefab = Resources.Load<GameObject>("Prefabs/TDD/InteractiveRadius");
        if (barkInteractionIndicatorPrefab == null)
        {
            Debug.LogError("barkInteractionIndicatorPrefab couldn't be located and assigned");
        }
        else
        {
            barkInteractionIndicator = Object.Instantiate(barkInteractionIndicatorPrefab, barkInteractionIndicatorPrefab.transform.position, barkInteractionIndicatorPrefab.transform.rotation);
            barkInteractionIndicator.name = "InteractionIndicator";
        }

        return barkInteractionIndicator;
    }

    public void CastRadius(GameObject barkInteractionIndicator)
    {
        barkInteractionIndicator.transform.position = new Vector3(this.transform.position.x, barkInteractionIndicator.transform.position.y, this.transform.position.z);

        int excludeLayer = LayerMask.NameToLayer("Ignore Raycast");
        int mask = ~(1 << excludeLayer);

        Collider[] colliderList = Physics.OverlapSphere(transform.position, BarkInteractionRadius, mask);

        foreach (Collider collider in colliderList)
        {
            BaseController interactiveCharacterController = collider.GetComponent<BaseController>();
            GameObject interactiveCharacter = collider.gameObject;

            List<GameObject> interactiveCharacterChildrenList = GetInteractiveCharacterChildrenList(interactiveCharacter);

            foreach (GameObject interactiveCharacterChild in interactiveCharacterChildrenList)
            {
                Debug.Log($"({interactiveCharacter.name}): {interactiveCharacterChild.name}");
            }

            if (Input.GetKeyDown(KeyCode.I)) // placeholder
            {
                interactiveCharacterController.Interact();
            }
        }
    }

    List<GameObject> GetInteractiveCharacterChildrenList(GameObject interactiveCharacter)
    {
        List<GameObject> childrenList = new List<GameObject>();

        foreach (Transform child in interactiveCharacter.transform)
        {
            Renderer renderer = child.GetComponent<Renderer>();
            if (renderer != null && renderer.material != null)
            {
                childrenList.Add(child.gameObject);
            }

            childrenList.AddRange(GetInteractiveCharacterChildrenList(child.gameObject));
        }

        return childrenList;
    }

    private void MovementControl(float forwardSpeed, float backwardSpeed, float sidewardSpeed, float jumpForce, float jumpMovementSpeed)
    {
        // horizontal movement, slower movement while jumping
        horizontalInput = Input.GetAxis("Horizontal");
        Move(Vector3.right, horizontalInput, sidewardSpeed);

        // forwards movement, slower movement while jumping
        forwardInput = Input.GetAxis("Forward");
        Move(Vector3.forward, forwardInput, forwardSpeed);

        // backwards movement, slower movement while jumping
        backwardInput = Input.GetAxis("Backward");
        Move(Vector3.forward, backwardInput, backwardSpeed);

        // jump
        jumpInput = Input.GetKeyDown(KeyCode.LeftShift);
        if (jumpInput && isGrounded)
        {
            Jump(jumpForce);
        }
    }

    public void Move(Vector3 direction, float input, float speed)
    {
        transform.Translate(direction * input * Time.deltaTime * speed * (isGrounded ? 1 : jumpMovementSpeed));
    }

    private void MovementBoundaries(float xBoundary, float zBoundary)
    {
        // player movement boundaries - left/right
        if (transform.position.x < -xBoundary)
        {
            transform.position = new Vector3(-xBoundary, transform.position.y, transform.position.z);
        }
        if (transform.position.x > xBoundary)
        {
            transform.position = new Vector3(xBoundary, transform.position.y, transform.position.z);
        }
        // player movement boundaries - forwards/backwards
        if (transform.position.z < -zBoundary)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -zBoundary);
        }
        if (transform.position.z > zBoundary)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, zBoundary);
        }
    }

    private void CheckBarkCommand()
    {
        // player bark move command
        barkMoveInput = Input.GetKeyDown(KeyCode.Space);

        if (barkMoveInput && !HasBarkedMove)
        {
            HasBarkedMove = true;
            sheepdogHeadAnim.SetTrigger("isBarkingMove");
            barkEffect.Play();
            _audioManager.HasDetectedBarkMove = true;
            StartCoroutine(BarkMoveCooldown(1.0f));
        }

        // player bark jump command
        barkJumpInput = Input.GetKeyDown(KeyCode.Tab);

        // keep track of herd to check they're grounded to trigger jump
        if (barkJumpInput && !HasBarkedJump && _spawnManager.CheckSheepGrounded())
        {
            sheepdogHeadAnim.SetTrigger("isBarkingJump");
            HasBarkedJump = true;
            barkEffect.Play();
            _audioManager.HasDetectedBarkJump = true;
            StartCoroutine(BarkJumpCooldown(1.0f));
        }
    }

    private void CheckPlayerDeath()
    {
        // player death
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void Jump(float jumpForce)
    {
        isGrounded = false;
        sheepdogBodyAnim.SetTrigger("isJumping");
        sheepdogHeadAnim.SetTrigger("isJumping");
        sheepdogRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
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

    void PlaySheepdogCollisionEffect()
    {
        Instantiate(sheepdogCollisionEffect, transform.position, transform.rotation);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // bitten by wolf
        if(collision.gameObject.CompareTag("WolfHuntingDog") || collision.gameObject.CompareTag("WolfHuntingSheep"))
        {
            WolfController wolfController = collision.gameObject.GetComponent<WolfController>();
            dependancyManager.InjectWolfControllerDependancyIntoPlayerController(wolfController);
            if (!_wolfController.HasBitten)
            {
                _audioManager.HasDetectedCollision = true;
                _wolfController.HasBitten = true;
                PlaySheepdogCollisionEffect();
                Health -= 1;
            }
            
        }

        // collided with obstacle
        if (collision.gameObject.CompareTag("Obstacle") && !collision.gameObject.GetComponent<ObstacleController>().hasHitPlayer) // dependency
        {
            _audioManager.HasDetectedCollision = true;
            collision.gameObject.GetComponent<ObstacleController>().hasHitPlayer = true; // dependency
            PlaySheepdogCollisionEffect();
            Health -= 1;
        }

        // is grounded
        if (collision.gameObject.CompareTag("Trail Lane"))
        {
            isGrounded = true;
        }

        // player thrown off by sheep if they land on top of them
        if (collision.gameObject.CompareTag("Sheep"))
        {
            SheepController sheepController = collision.gameObject.GetComponent<SheepController>();
            dependancyManager.InjectSheepControllerDependancyIntoPlayerController(sheepController);
            if (PlayerTransform.position.y - _sheepController.SheepTransform.position.y > heightTrigger)
            {
                sheepdogRb.AddForce(Vector3.up * thrownSpeed, ForceMode.Impulse);
            }
        }
    }
}
