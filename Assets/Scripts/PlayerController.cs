using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayerController
{
    public Transform PlayerTransform { get; set; }
    public int Health { get; set; }

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
    //public bool hasBarkedMove = false;
    //public bool hasBarkedJump = false;
    public bool HasBarkedMove { get; set; }
    public bool HasBarkedJump { get; set; }

    // bark particle 
    public ParticleSystem barkEffect;

    // herd
    private GameObject[] herd;

    // collision with top of sheep
    [SerializeField] private float thrownSpeed = 4.0f;
    [SerializeField] private float heightTrigger = 2.2f;

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

    // dependancies
    public void SetDependencies(IAudioManager audioManager, IUIManager uiManager)
    {
        _audioManager = audioManager;
        _uiManager = uiManager;
    }

    private void Awake()
    {
        PlayerTransform = this.transform;
        Health = 5;
    }

    // Start is called before the first frame update
    void Start()
    {
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
        }

        PlayerTransform = transform;
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
            sheepdogHeadAnim.Play("dog head bark move");
            barkEffect.Play();
            _audioManager.HasDetectedBarkMove = true;
            StartCoroutine(BarkMoveCooldown(1.0f));
        }

        // player bark jump command
        barkJumpInput = Input.GetKeyDown(KeyCode.Tab);
        // keep track of herd to check they're grounded to trigger jump
        herd = GameObject.FindGameObjectsWithTag("Sheep");
        if (barkJumpInput && !HasBarkedJump && CheckSheepGrounded(herd))
        {
            sheepdogHeadAnim.Play("dog head bark jump");
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
        sheepdogBodyAnim.Play("dog jump");
        sheepdogHeadAnim.Play("dog head jump");
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

    private bool CheckSheepGrounded(GameObject[] herd)
    {
        bool allGrounded = true;
        foreach (GameObject sheep in herd)
        {
            allGrounded &= sheep.GetComponent<SheepController>().isGrounded; // dependency
        }
        return allGrounded;
    }

    void PlaySheepdogCollisionEffect()
    {
        GameObject sheepCollision = Instantiate(sheepdogCollisionEffect, transform.position, transform.rotation);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // bitten by wolf
        if(collision.gameObject.tag == "Wolf" && !collision.gameObject.GetComponent<WolfController>().hasBitten) // dependency
        {
            _audioManager.HasDetectedCollision = true;
            collision.gameObject.GetComponent<WolfController>().hasBitten = true; // dependency
            PlaySheepdogCollisionEffect();
            Health -= 1;
        }

        // collided with obstacle
        if (collision.gameObject.tag == "Obstacle" && !collision.gameObject.GetComponent<ObstacleController>().hasHitPlayer) // dependency
        {
            _audioManager.HasDetectedCollision = true;
            collision.gameObject.GetComponent<ObstacleController>().hasHitPlayer = true; // dependency
            PlaySheepdogCollisionEffect();
            Health -= 1;
        }

        // is grounded
        if (collision.gameObject.tag == "Trail Lane")
        {
            isGrounded = true;
        }

        // player thrown off by sheep if they land on top of them
        if (collision.gameObject.tag == "Sheep" && (transform.position.y - collision.gameObject.transform.position.y) > heightTrigger)
        {
            sheepdogRb.AddForce(Vector3.up * thrownSpeed, ForceMode.Impulse);
        }
    }
}
