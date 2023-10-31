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
    public float barkCooldownTime = 0.1f;
    public ParticleSystem barkEffect;

    // sound effects
    // REVISIT: Test once sound effects sourced
    private AudioSource sheepdogAudio;
    public AudioClip barkSound;
    public AudioClip hurtSound;

    // Start is called before the first frame update
    void Start()
    {
        sheepdogAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // player movement
        horizontalInput = Input.GetAxis("Horizontal");
        transform.Translate(Vector3.right * horizontalInput * Time.deltaTime * sidewardSpeed);

        // slower movement when going backwards, faster movement when going forwards
        forwardInput = Input.GetAxis("Forward");
        transform.Translate(Vector3.forward * forwardInput * Time.deltaTime * forwardSpeed);

        backwardInput = Input.GetAxis("Backward");
        transform.Translate(Vector3.forward * backwardInput * Time.deltaTime * backwardSpeed);

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

        // player bark
        if (Input.GetKeyDown(KeyCode.Space) && !hasBarked)
        {
            barkEffect.Play();
            // REVISIT: Test once sound effects sourced
            // sheepdogAudio.PlayOneShot(barkSound, 1.0f);
            hasBarked = true;
            StartCoroutine(BarkCooldown());
        }


    }

    // Coroutine to wait for bark to cool down
    IEnumerator BarkCooldown()
    {
        yield return new WaitForSeconds(barkCooldownTime);
        hasBarked = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Wolf")
        {
            Debug.Log("Bitten!");
            // REVISIT: Test once sound effects sourced
            // sheepdogAudio.PlayOneShot(hurtSound, 1.0f);
        }
        if (collision.gameObject.tag == "Obstacle")
        {
            Debug.Log("Collided!");
            // REVISIT: Test once sound effects sourced
            // sheepdogAudio.PlayOneShot(hurtSound, 1.0f);
        }
    }
}
