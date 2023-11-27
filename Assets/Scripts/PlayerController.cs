using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // inputs
    private float horizontalInput;
    private float forwardInput;
    private float backwardInput;

    // speed control
    private float sidewardSpeed = 7.0f;
    private float forwardSpeed = 9.0f;
    private float backwardSpeed = 6.0f;

    // boundary control
    private int xBoundary = 7;
    private int zBoundary = 15;

    // bark control
    public bool hasBarked = false;
    public bool hasBarkedJump = false;
    public float barkCooldownTime;
    public float barkJumpCooldownTime;
    public ParticleSystem barkEffect;

    // sound effects
    // REVISIT: Test once sound effects sourced
    private AudioSource sheepdogAudio;
    public AudioClip barkSound;
    public AudioClip hurtSound;

    public GameObject[] herd;
    private Rigidbody sheepdogRb;

    public float jumpForce;
    public bool isGrounded = false;
    public float jumpSpeed = 0.4f;
    public float backJumpSpeed = 0.2f;

    public float thrownSpeed = 2.0f;
    public float heightTrigger = 0.5f;

    // health
    public int health = 5;

    private bool isGameActive;

    // Start is called before the first frame update
    void Start()
    {
        sheepdogAudio = GetComponent<AudioSource>();
        sheepdogRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        isGameActive = GameObject.Find("UIManager").GetComponent<UIManager>().isGameActive;

        if (isGameActive)
        {
            // horizontal player movement
            horizontalInput = Input.GetAxis("Horizontal");
            if (isGrounded)
            {
                transform.Translate(Vector3.right * horizontalInput * Time.deltaTime * sidewardSpeed);
            }
            else
            {
                transform.Translate(Vector3.right * horizontalInput * Time.deltaTime * sidewardSpeed * jumpSpeed);
            }

            // slower movement when going backwards, faster movement when going forwards
            forwardInput = Input.GetAxis("Forward");
            if (isGrounded)
            {
                transform.Translate(Vector3.forward * forwardInput * Time.deltaTime * forwardSpeed);
            }
            else
            {
                transform.Translate(Vector3.forward * forwardInput * Time.deltaTime * forwardSpeed * jumpSpeed);
            }

            backwardInput = Input.GetAxis("Backward");
            if (isGrounded)
            {
                transform.Translate(Vector3.forward * backwardInput * Time.deltaTime * backwardSpeed);
            }
            else
            {
                transform.Translate(Vector3.forward * backwardInput * Time.deltaTime * backwardSpeed * backJumpSpeed);
            }

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


            // keep track of herd
            herd = GameObject.FindGameObjectsWithTag("Sheep");

            // player bark
            if (Input.GetKeyDown(KeyCode.Space) && !hasBarked)
            {
                barkEffect.Play();
                // REVISIT: Test once sound effects sourced
                // sheepdogAudio.PlayOneShot(barkSound, 1.0f);
                hasBarked = true;
                StartCoroutine(BarkCooldown());
            }

            if (Input.GetKeyDown(KeyCode.Tab) && !hasBarkedJump && CheckSheepGrounded(herd))
            {
                hasBarkedJump = true;
                StartCoroutine(BarkJumpCooldown());
            }

            if (Input.GetKeyDown(KeyCode.J) && isGrounded)
            {
                Jump();
            }

            if (health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void Jump()
    {
        isGrounded = false;
        sheepdogRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    // Coroutine to wait for bark to cool down
    IEnumerator BarkCooldown()
    {
        yield return new WaitForSeconds(barkCooldownTime);
        hasBarked = false;
    }

    // Coroutine to wait for bark to cool down
    IEnumerator BarkJumpCooldown()
    {
        yield return new WaitForSeconds(barkJumpCooldownTime);
        hasBarkedJump = false;
    }

    private bool CheckSheepGrounded(GameObject[] herd)
    {
        bool allGrounded = true;
        foreach (GameObject sheep in herd)
        {
            allGrounded &= sheep.GetComponent<SheepController2>().isGrounded;
        }
        return allGrounded;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Wolf" && !collision.gameObject.GetComponent<WolfController>().hasBitten)
        {
            Debug.Log("Bitten!");
            collision.gameObject.GetComponent<WolfController>().hasBitten = true;
            health -= 1;
            // REVISIT: Test once sound effects sourced
            // sheepdogAudio.PlayOneShot(hurtSound, 1.0f);
        }

        if (collision.gameObject.tag == "Obstacle" && !collision.gameObject.GetComponent<MoveBackwards>().hasHitPlayer)
        {
            Debug.Log("Collided!");
            collision.gameObject.GetComponent<MoveBackwards>().hasHitPlayer = true;
            health -= 1;
            // REVISIT: Test once sound effects sourced
            // sheepdogAudio.PlayOneShot(hurtSound, 1.0f);
        }

        if (collision.gameObject.tag == "Trail Lane")
        {
            isGrounded = true;
        }

        if (collision.gameObject.tag == "Sheep" && (transform.position.y - collision.gameObject.transform.position.y) > heightTrigger)
        {
            sheepdogRb.AddForce(Vector3.up * thrownSpeed, ForceMode.Impulse);
        }
    }
}
