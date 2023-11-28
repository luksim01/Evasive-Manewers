using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SheepController2 : MonoBehaviour
{
    // bark interaction
    private GameObject sheepDog;
    public bool isBarkedAt;
    public bool isBarkedJumpAt;
    public bool pastBarkJumpState;
    public bool sheepDogGrounded;
    private Rigidbody sheepRb;

    public bool isSlowingDown = true;
    public int slowdownCooldownTime = 5;

    // trail lane alignment
    public GameObject[] trailLaneTargets;
    private float alignSpeed = 8.0f;
    private float xFleeBoundary = 4.6f;
    private float xBoundary = 7.7f;
    private float xAvoidDistance = 1.2f;
    private int zForwardBoundary = 15;
    private int zAvoidDistance = 5;
    private int zBackwardBoundary = -32;

    private Vector3 sheepDogProximity;
    private float sheepDogProximityX;
    private float sheepDogProximityZ;

    private Vector3 fleeDirection;
    private float speedBurst = 4.0f;
    private float jumpForce = 8.0f;
    public float throwForce = 4.0f;

    public GameObject[] herd;

    public bool isGrounded = false;
    private float jumpDelayModifier = 7.5f;

    private float sheepDogProximityXLimit = 2.5f;
    private float sheepDogProximityZLimit = 7.0f;

    private float sheepDogBarkProximityXLimit = 6.0f;
    private float sheepDogBarkProximityZLimit = 10.0f;

    private float sheepProximityXLimit = 1.7f;
    private float sheepProximityZLimit = 2.7f;

    private float heightBoundary = 2.0f;

    private float sheepSlowdownSpeed = 0.5f;

    private float boundaryAvoidSpeed = 0.3f;
    private int avoidSpeed = 2;

    private float hopDirectionX;
    private float hopDirectionY = 0.5f;
    private float hopDirectionZ = -0.2f;

    private Vector3 jumpDirection = new Vector3(0, 1, 0.05f);

    public float heightTrigger = 0.5f;

    private bool isGameActive;

    private GameObject uiManager;

    private GameObject audioManager;

    // Start is called before the first frame update
    void Start()
    {
        sheepDog = GameObject.Find("Sheepdog");
        sheepRb = GetComponent<Rigidbody>();

        pastBarkJumpState = isBarkedJumpAt;

        uiManager = GameObject.Find("UIManager");

        audioManager = GameObject.Find("AudioManager");
    }

    // Update is called once per frame
    void Update()
    {
        isGameActive = uiManager.GetComponent<UIManager>().isGameActive;

        if (isGameActive)
        {
            // keep track of herd
            herd = GameObject.FindGameObjectsWithTag("Sheep");

            // keep track of barks
            isBarkedAt = sheepDog.GetComponent<PlayerController>().hasBarked;
            isBarkedJumpAt = sheepDog.GetComponent<PlayerController>().hasBarkedJump;
            sheepDogGrounded = sheepDog.GetComponent<PlayerController>().isGrounded;

            // move away from the player
            sheepDogProximityX = transform.position.x - sheepDog.transform.position.x;
            sheepDogProximityZ = transform.position.z - sheepDog.transform.position.z;
            sheepDogProximity = new Vector3(sheepDogProximityX, 0, sheepDogProximityZ);

            if (Mathf.Abs(sheepDogProximityX) < sheepDogProximityXLimit && Mathf.Abs(sheepDogProximityZ) < sheepDogProximityZLimit)
            {
                Avoid(sheepDogProximity);
            }
            else if (isSlowingDown)
            {
                // sheep gradually falls behind
                transform.Translate(Vector3.back * sheepSlowdownSpeed * Time.deltaTime);
            }

            // sheep flee bark if within certain distance of it
            if (isBarkedAt && Mathf.Abs(sheepDogProximityX) < sheepDogBarkProximityXLimit && Mathf.Abs(sheepDogProximityZ) < sheepDogBarkProximityZLimit)
            {
                FleeBark(sheepDogProximity, sheepDog.transform.position.x);
            }

            // stagger jump of herd
            if (isBarkedJumpAt != pastBarkJumpState && !pastBarkJumpState)
            {
                float jumpDelay = CalculateDelay(herd);
                StartCoroutine(SheepJump(jumpDelay));
            }

            pastBarkJumpState = isBarkedJumpAt;


            // sheep try to keep a small distance from each other
            foreach (GameObject sheep in herd)
            {
                float sheepProximityX = transform.position.x - sheep.transform.position.x;
                float sheepProximityY = transform.position.y - sheep.transform.position.y;
                float sheepProximityZ = transform.position.z - sheep.transform.position.z;

                Vector3 sheepProximity = new Vector3(sheepProximityX, 0, sheepProximityZ);

                if (Mathf.Abs(sheepProximityX) < sheepProximityXLimit && Mathf.Abs(sheepProximityZ) < sheepProximityZLimit)
                {
                    Avoid(sheepProximity);
                }
            }

            // sheep remains within forest trail, and tries to move away from the forest trail boundary
            if (transform.position.x > xBoundary - xAvoidDistance && transform.position.y < heightBoundary)
            {
                sheepRb.AddForce(Vector3.left * boundaryAvoidSpeed, ForceMode.Impulse);
            }
            if (transform.position.x > xBoundary)
            {
                transform.position = new Vector3(xBoundary, transform.position.y, transform.position.z);
            }

            if (transform.position.x < -xBoundary + xAvoidDistance && transform.position.y < heightBoundary)
            {
                sheepRb.AddForce(Vector3.right * boundaryAvoidSpeed, ForceMode.Impulse);
            }
            if (transform.position.x < -xBoundary)
            {
                transform.position = new Vector3(-xBoundary, transform.position.y, transform.position.z);
            }

            // sheep doesn't move too far forward on the trail and flees backwards if forced to
            if (transform.position.z > zForwardBoundary - zAvoidDistance && transform.position.y < heightBoundary)
            {
                sheepRb.AddForce(Vector3.back * boundaryAvoidSpeed, ForceMode.Impulse);
            }
            if (transform.position.z > zForwardBoundary)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, zForwardBoundary);
            }

            // sheep is lost if allowed to drift back too far
            if (transform.position.z < zBackwardBoundary)
            {
                audioManager.GetComponent<AudioManager>().hasDetectedLostSheep = true;
                Debug.Log("Sheep Lost!");
                Destroy(gameObject);
            }
        }
    }

    private float CalculateDelay(GameObject[] herd)
    {
        float originZ = zBackwardBoundary;
        float frontSheepPosZ = transform.position.z - originZ;

        // largest distance to generate delays for staggered jump
        foreach (GameObject sheep in herd)
        {
            if ((sheep.transform.position.z - originZ) > frontSheepPosZ)
            {
                frontSheepPosZ = sheep.transform.position.z - originZ;
            }
        }

        float delay = jumpDelayModifier * (frontSheepPosZ - (transform.position.z - originZ)) / (frontSheepPosZ);
        return delay;
    }

    void Hop(float force)
    {
        hopDirectionX = Random.Range(-0.2f, 0.2f);
        sheepRb.AddForce(new Vector3(hopDirectionX, hopDirectionY, hopDirectionZ) * force, ForceMode.Impulse);
        isGrounded = false;
    }

    IEnumerator SheepJump(float delay)
    {
        yield return new WaitForSeconds(delay);
        sheepRb.AddForce(jumpDirection * jumpForce, ForceMode.Impulse);
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
        transform.Translate(alignDirection * avoidSpeed * Time.deltaTime);
    }

    void FleeBark(Vector3 direction, float sheepDogPosX)
    {
        fleeDirection = (direction).normalized;
        float fleeDirectionX = fleeDirection.x;
        float fleeDirectionZ = fleeDirection.z;
        transform.Translate(new Vector3(fleeDirectionX, 0, fleeDirectionZ) * speedBurst * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            audioManager.GetComponent<AudioManager>().hasDetectedCollision = true;
            audioManager.GetComponent<AudioManager>().hasDetectedLostSheep = true;
            Debug.Log("Sheep Lost!");
            Destroy(gameObject);
        }
        if (collision.gameObject.tag == "Trail Lane")
        {
            isGrounded = true;
        }
        // hop away if landed on top of another sheep or player
        if ((collision.gameObject.tag == "Sheep" || collision.gameObject.tag == "Player") && (transform.position.y - collision.gameObject.transform.position.y) > heightTrigger )
        {
            Hop(jumpForce);
        }

        //hope away if player lands ontop of sheep
        if (collision.gameObject.tag == "Player" && (collision.gameObject.transform.position.y - transform.position.y) > heightTrigger && isGrounded)
        {
            Hop(throwForce);
        }
    }
}
