using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepController2 : MonoBehaviour
{
    // bark interaction
    private GameObject sheepDog;
    public bool isBarkedAt;
    public bool isBarkedJumpAt;
    public bool pastBarkState;
    private Rigidbody sheepRb;
    public float forwardBurstSpeed = 2000.0f;
    public float sidewardBurstSpeed = 2000.0f;
    public int directionIndex;
    public GameObject[] trailLanes;
    public float[] laneBoundsLower;
    public float[] laneBoundsUpper;
    public bool isSlowingDown = true;
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
    public float jumpForce = 3.5f;

    public GameObject[] herd; 

    // Start is called before the first frame update
    void Start()
    {
        sheepDog = GameObject.Find("Sheepdog");
        sheepRb = GetComponent<Rigidbody>();
        sheepAudio = GetComponent<AudioSource>();

        pastBarkState = isBarkedAt;

        herd = GameObject.FindGameObjectsWithTag("Sheep");

        foreach (GameObject sheep in herd)
        {
            Debug.Log("Sheep: " + sheep.name);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        isBarkedAt = sheepDog.GetComponent<PlayerController>().hasBarked;
        isBarkedJumpAt = sheepDog.GetComponent<PlayerController>().hasBarkedJump;

        // move away from the player
        sheepDogProximityX = transform.position.x - sheepDog.transform.position.x;
        sheepDogProximityZ = transform.position.z - sheepDog.transform.position.z;
        sheepDogProximity = new Vector3(sheepDogProximityX, 0, sheepDogProximityZ);
        if( Mathf.Abs(sheepDogProximityX) < 2.5f && Mathf.Abs(sheepDogProximityZ) < 7.0f)
        {
            Avoid(sheepDogProximity);
        }
        else
        {
            // sheep gradually falls behind
            transform.Translate(Vector3.back * 0.5f * Time.deltaTime);
        }
        if (isBarkedAt && Mathf.Abs(sheepDogProximityX) < 6.0f && Mathf.Abs(sheepDogProximityZ) < 10.0f)
        {
            //Debug.Log("Bark detected within proximity");
            FleeBark(sheepDogProximity, sheepDog.transform.position.x);
        }

        if (isBarkedJumpAt)
        {
            sheepRb.AddForce(new Vector3(0, 1, 0.05f) * jumpForce, ForceMode.Impulse);
        }

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
    }
}
