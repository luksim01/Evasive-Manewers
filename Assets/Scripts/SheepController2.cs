using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepController2 : MonoBehaviour
{
    // bark interaction
    private GameObject sheepDog;
    public bool isBarkedAt;
    public bool isBarkedJumpAt;
    public bool pastBarkJumpState;
    private Rigidbody sheepRb;

    public bool isSlowingDown = false;
    public int slowdownCooldownTime = 5;

    // trail lane alignment
    public GameObject[] trailLaneTargets;
    public float alignSpeed = 8.0f;
    private float xFleeBoundary = 4.6f;
    private float xBoundary = 6.2f;
    private int zForwardBoundary = 10;
    private int zBackwardBoundary = -28;

    // sound effects
    // REVISIT: Test once sound effects sourced
    private AudioSource sheepAudio;
    public AudioClip collisionSound;


    public Vector3 sheepDogProximity;
    public float sheepDogProximityX;
    public float sheepDogProximityZ;

    public Vector3 fleeDirection;
    public float speedBurst = 4.0f;
    public float jumpForce = 8.0f;
    public float jumpDelay;

    public GameObject[] herd;

    public bool isGrounded = true;
    public float jumpDelayModifier = 7.5f;

    // Start is called before the first frame update
    void Start()
    {
        sheepDog = GameObject.Find("Sheepdog");
        sheepRb = GetComponent<Rigidbody>();
        sheepAudio = GetComponent<AudioSource>();

        pastBarkJumpState = isBarkedJumpAt;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // keep track of herd
        herd = GameObject.FindGameObjectsWithTag("Sheep");

        // keep track of barks
        isBarkedAt = sheepDog.GetComponent<PlayerController>().hasBarked;
        isBarkedJumpAt = sheepDog.GetComponent<PlayerController>().hasBarkedJump;

        // move away from the player
        sheepDogProximityX = transform.position.x - sheepDog.transform.position.x;
        sheepDogProximityZ = transform.position.z - sheepDog.transform.position.z;
        sheepDogProximity = new Vector3(sheepDogProximityX, 0, sheepDogProximityZ);

        if( Mathf.Abs(sheepDogProximityX) < 2.5f && Mathf.Abs(sheepDogProximityZ) < 7.0f)
        {
            //Avoid(sheepDogProximity);
        }
        else if(isSlowingDown)
        {
            // sheep gradually falls behind
            transform.Translate(Vector3.back * 0.5f * Time.deltaTime);
        }

        if (isBarkedAt && Mathf.Abs(sheepDogProximityX) < 6.0f && Mathf.Abs(sheepDogProximityZ) < 10.0f)
        {
            //FleeBark(sheepDogProximity, sheepDog.transform.position.x);
        }

        // stagger jump of sheep
        //if (isBarkedJumpAt && isGrounded)
        if (isBarkedJumpAt != pastBarkJumpState && !pastBarkJumpState)
        //if(Input.GetKeyDown(KeyCode.Tab) && isGrounded)
        {
            float originZ = zBackwardBoundary;
            float furthestForwardSheepPosZ = transform.position.z - originZ;

            foreach (GameObject sheep in herd)
            {
                if ((sheep.transform.position.z - originZ) > furthestForwardSheepPosZ)
                {
                    furthestForwardSheepPosZ = sheep.transform.position.z - originZ;
                }
            }

            float jumpDelay = jumpDelayModifier * (furthestForwardSheepPosZ - (transform.position.z - originZ)) / (furthestForwardSheepPosZ);

            StartCoroutine(SheepJump(jumpDelay));
        }

        pastBarkJumpState = isBarkedJumpAt;

        // sheep try to keep a small distance from each other
        foreach (GameObject sheep in herd)
        {
            float sheepProximityX = transform.position.x - sheep.transform.position.x;
            float sheepProximityZ = transform.position.z - sheep.transform.position.z;
            Vector3 sheepProximity = new Vector3(sheepProximityX, 0, sheepProximityZ);
            if (Mathf.Abs(sheepProximityX) < 1.5f && Mathf.Abs(sheepProximityZ) < 2.5f)
            {
                Avoid(sheepProximity);
            }
        }

        // sheep remains within forest trail
        if (transform.position.x > xBoundary)
        {
            transform.position = new Vector3(xBoundary, transform.position.y, transform.position.z);
        }
        else if (transform.position.x < -xBoundary)
        {
            transform.position = new Vector3(-xBoundary, transform.position.y, transform.position.z);
        }
        else if (transform.position.z > zForwardBoundary)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, zForwardBoundary);
        }
        else if (transform.position.z < zBackwardBoundary)
        {
            // sheep is lost if allowed to drift back too far
            Destroy(gameObject);
        }
    }

    void Jump()
    {
        sheepRb.AddForce(new Vector3(0, 1, 0.05f) * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    IEnumerator SheepJump(float delay)
    {
        yield return new WaitForSeconds(delay);
        sheepRb.AddForce(new Vector3(0, 1, 0.05f) * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    IEnumerator SlowdownCooldown()
    {
        yield return new WaitForSeconds(slowdownCooldownTime);
        isSlowingDown = true;
    }


    void Avoid(Vector3 direction)
    {
        Vector3 alignDirection = (direction).normalized;
        transform.Translate(alignDirection * 2 * Time.deltaTime);
    }

    void FleeBark(Vector3 direction, float sheepDogPosX)
    {
        fleeDirection = (direction).normalized;
        float fleeDirectionX = fleeDirection.x;
        float fleeDirectionZ = fleeDirection.z;
        transform.Translate(new Vector3(fleeDirectionX*speedBurst, 0, fleeDirectionZ*speedBurst) * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            Debug.Log("Sheep Lost!");
            // REVISIT: Test once sound effects sourced
            //sheepAudio.PlayOneShot(collisionSound, 1.0f);
            Destroy(gameObject);
        }
        if (collision.gameObject.tag == "Trail Lane")
        {
            isGrounded = true;
        }
    }
}
