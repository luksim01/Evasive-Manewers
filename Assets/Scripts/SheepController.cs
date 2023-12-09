using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SheepController : MonoBehaviour
{
    // bark interaction
    private GameObject sheepDog;
    private bool isBarkedAt;
    private bool isBarkedJumpAt;
    private bool pastBarkJumpState;
    private bool sheepDogGrounded;
    private Rigidbody sheepRb;

    public bool isSlowingDown = true;

    // interaction boundaries
    private float xBoundary = 7.6f;
    private float xAvoidDistance = 1.2f;
    private int zForwardBoundary = 15;
    private int zAvoidDistance = 5;
    private int zBackwardBoundary = -32;

    // escape controls
    private Vector3 fleeDirection;
    private float speedBurst = 4.0f;
    private float jumpForce = 8.0f;
    private float throwForce = 8.0f;

    private float sheepSlowdownSpeed = 0.5f;
    private float boundaryAvoidSpeed = 0.3f;
    private int avoidSpeed = 2;

    private float heightBoundary = 2.0f;

    private float hopDirectionX;
    private float hopDirectionY = 0.5f;
    private float hopDirectionZ = -0.2f;
    private Vector3 jumpDirection = new Vector3(0, 1.2f, 0.1f);
    private float heightTrigger = 2.2f;

    // staggered jump
    private GameObject[] herd;
    public bool isGrounded = false;
    private float jumpDelayModifier = 7.5f;

    // player proximity
    private Vector3 sheepDogProximity;
    private float sheepDogProximityX;
    private float sheepDogProximityZ;

    private float sheepDogProximityXLimit = 2.5f;
    private float sheepDogProximityZLimit = 7.0f;

    private float sheepDogBarkProximityXLimit = 6.0f;
    private float sheepDogBarkProximityZLimit = 10.0f;

    private float sheepProximityXLimit = 2.0f;
    private float sheepProximityZLimit = 3.0f;

    private float straySheepProximityXLimit = 2.7f;
    private float straySheepProximityZLimit = 3.7f;

    private float wolfProximityXLimit = 2.5f;
    private float wolfProximityZLimit = 7.0f;

    // UI
    private bool isGameActive;
    private GameObject uiManager;

    // audio
    private GameObject audioManager;

    // spawn manager
    private GameObject spawnManager;

    // stray sheep
    public bool isHerdSheep;

    // wolf 
    public bool hasEnteredWolfSpace;
    public bool hasAvoidedWolf;

    private float xDistanceFromBoundary = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        sheepDog = GameObject.Find("Sheepdog");
        sheepRb = GetComponent<Rigidbody>();
        pastBarkJumpState = isBarkedJumpAt;
        uiManager = GameObject.Find("UIManager");
        audioManager = GameObject.Find("AudioManager");
        spawnManager = GameObject.Find("SpawnManager");
    }

    // Update is called once per frame
    void Update()
    {
        isGameActive = uiManager.GetComponent<UIManager>().isGameActive;

        if (isGameActive)
        {
            // keep track of barks
            isBarkedAt = sheepDog.GetComponent<PlayerController>().hasBarked;
            isBarkedJumpAt = sheepDog.GetComponent<PlayerController>().hasBarkedJump;
            sheepDogGrounded = sheepDog.GetComponent<PlayerController>().isGrounded;

            // player proximity
            sheepDogProximityX = transform.position.x - sheepDog.transform.position.x;
            sheepDogProximityZ = transform.position.z - sheepDog.transform.position.z;
            sheepDogProximity = new Vector3(sheepDogProximityX, 0, sheepDogProximityZ);

            // keep track of herd
            herd = GameObject.FindGameObjectsWithTag("Sheep");

            // sheep behaviour based on tag
            CheckSheepTypeIsHerd();
        }
    }

    void StraySheepBehaviour()
    {
        //unaffected by physics while outside of trail
        if( transform.position.x < xBoundary + xDistanceFromBoundary && transform.position.x > -xBoundary - xDistanceFromBoundary)
        {
            sheepRb.isKinematic = false;
            if (isGrounded)
            {
                sheepRb.AddForce(jumpDirection * jumpForce, ForceMode.Impulse);
                isGrounded = false;
            }
        }
        else
        {
            sheepRb.isKinematic = true;
        }

        // movement across trail
        Vector3 spawnPosition = spawnManager.GetComponent<SpawnManager>().straySheepSpawnPosition;
        Vector3 targetPosition = spawnManager.GetComponent<SpawnManager>().straySheepTargetPosition;
        Vector3 targetDirection = targetPosition - spawnPosition;
        MoveAcrossTrail(targetDirection);

        // stray sheep avoids herd sheep
        foreach (GameObject sheep in herd)
        {
            float sheepProximityX = transform.position.x - sheep.transform.position.x;
            float sheepProximityZ = transform.position.z - sheep.transform.position.z;

            Vector3 sheepProximity = new Vector3(sheepProximityX, 0, sheepProximityZ);

            if (Mathf.Abs(sheepProximityX) < straySheepProximityXLimit && Mathf.Abs(sheepProximityZ) < straySheepProximityZLimit)
            {
                Avoid(sheepProximity, avoidSpeed);
            }
        }

        // stray sheep avoid player
        if (Mathf.Abs(sheepDogProximityX) < straySheepProximityXLimit && Mathf.Abs(sheepDogProximityZ) < straySheepProximityZLimit)
        {
            Avoid(sheepDogProximity, avoidSpeed);
        }

        // stray sheep added to herd with close proximity bark wihtin trail
        if (isBarkedAt &&
            Mathf.Abs(sheepDogProximityX) < sheepDogBarkProximityXLimit/2 &&
            Mathf.Abs(sheepDogProximityZ) < sheepDogBarkProximityZLimit/2 &&
            transform.position.x < xBoundary - xAvoidDistance &&
            transform.position.x > -xBoundary + xAvoidDistance)
        {
            gameObject.tag = "Sheep";
        }

        // stray sheep is gone once beyond the forest boundary
        if (Mathf.Abs(targetPosition.x - transform.position.x) <= 0.2)
        {
            Destroy(gameObject);
        }
    }

    void MoveAcrossTrail(Vector3 direction)
    {
        Vector3 alignDirection = (direction).normalized;
        transform.Translate(alignDirection * 3 * Time.deltaTime);
    }

    void HerdSheepBehaviour()
    {
        // keep track of stray sheep
        GameObject straySheep = GameObject.FindGameObjectWithTag("Stray");

        // keep track of wolf
        GameObject wolf = GameObject.FindGameObjectWithTag("Wolf");
        
        // avoid player
        if (Mathf.Abs(sheepDogProximityX) < sheepDogProximityXLimit && Mathf.Abs(sheepDogProximityZ) < sheepDogProximityZLimit)
        {
            Avoid(sheepDogProximity, avoidSpeed);
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

        // track change of state of bark low to high
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
                Avoid(sheepProximity, avoidSpeed);
            }
        }


        // sheep avoid any active wolf
        //if (wolf)
        //{
        //    float wolfProximityX = transform.position.x - wolf.transform.position.x;
        //    float wolfProximityY = transform.position.y - wolf.transform.position.y;
        //    float wolfProximityZ = transform.position.z - wolf.transform.position.z;

        //    Vector3 wolfProximity = new Vector3(wolfProximityX, 0, 0);

        //    if (!hasAvoidedWolf && Mathf.Abs(wolfProximityX) < sheepProximityXLimit + 3 && Mathf.Abs(wolfProximityZ) < sheepProximityZLimit + 4)
        //    {
        //        Avoid(wolfProximity, avoidSpeed);
        //        hasEnteredWolfSpace = true;
        //    }
        //}

        // REVISIT: Reuse for bark, wolf hunting sheep or wolf running towards sheep
        if (wolf)
        {
            float wolfProximityX = transform.position.x - wolf.transform.position.x;
            float wolfProximityY = transform.position.y - wolf.transform.position.y;
            float wolfProximityZ = transform.position.z - wolf.transform.position.z;

            Vector3 wolfProximity = new Vector3(wolfProximityX, 0, 0);

            if (!hasAvoidedWolf && Mathf.Abs(wolfProximityX) < sheepProximityXLimit + 3 && Mathf.Abs(wolfProximityZ) < sheepProximityZLimit + 4)
            {
                if (isGrounded)
                {
                    sheepRb.AddForce(jumpDirection * jumpForce, ForceMode.Impulse);
                    isGrounded = false;
                }
                Avoid(wolfProximity, avoidSpeed);
                hasEnteredWolfSpace = true;
            }

            if (hasEnteredWolfSpace && Mathf.Abs(wolfProximityZ) > sheepProximityZLimit + 4)
            {
                hasAvoidedWolf = true;
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
            spawnManager.GetComponent<SpawnManager>().timeSinceLostSheep = 0;
            Destroy(gameObject);
        }
    }

    void HuntedSheepBehaviour()
    {
        //unaffected by physics while outside of trail
        if (transform.position.x < xBoundary + xDistanceFromBoundary && transform.position.x > -xBoundary - xDistanceFromBoundary)
        {
            sheepRb.isKinematic = false;
        }
        else
        {
            sheepRb.isKinematic = true;
        }

        // keep track of wolf
        GameObject wolf = GameObject.FindGameObjectWithTag("Wolf");

        if (wolf)
        {
            float wolfProximityX = transform.position.x - wolf.transform.position.x;
            float wolfProximityY = transform.position.y - wolf.transform.position.y;
            float wolfProximityZ = transform.position.z - wolf.transform.position.z;

            Vector3 wolfProximity = new Vector3(wolfProximityX, 0, wolfProximityZ);

            // flee wolf
            if (Mathf.Abs(wolfProximityX) < wolfProximityXLimit + 2 && Mathf.Abs(wolfProximityZ) < wolfProximityZLimit + 2)
            {
                Avoid(wolfProximity, avoidSpeed*2);
            }
        }
        else
        {
            Boundaries(12, -13, 30, -30);
        }
    }

    void Boundaries(float xBoundRight, float xBoundLeft, float zBoundForward, float zBoundBack)
    {
        // destroy if out of bounds
        if (transform.position.x > xBoundRight || transform.position.x < xBoundLeft)
        {
            Destroy(gameObject);
        }
        if (transform.position.z > zBoundForward || transform.position.z < zBoundBack)
        {
            Destroy(gameObject);
        }
    }

    void CheckSheepTypeIsHerd()
    {
        switch (tag)
        {
            case "Sheep":
                HerdSheepBehaviour();
                break;
            case "Stray":
                StraySheepBehaviour();
                break;
            case "Hunted":
                HuntedSheepBehaviour();
                break;
            default:
                break;
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

    void Avoid(Vector3 direction, float speed)
    {
        Vector3 alignDirection = (direction).normalized;
        transform.Translate(alignDirection * speed * Time.deltaTime);
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
        if (tag == "Sheep" && collision.gameObject.tag == "Obstacle")
        {
            audioManager.GetComponent<AudioManager>().hasDetectedCollision = true;
            audioManager.GetComponent<AudioManager>().hasDetectedLostSheep = true;
            spawnManager.GetComponent<SpawnManager>().timeSinceLostSheep = 0;

            Debug.Log("Sheep Lost!");
            Destroy(gameObject);
        }

        if (collision.gameObject.tag == "Trail Lane" || collision.gameObject.tag == "Boundary Lane")
        {
            isGrounded = true;
        }

        // hop away if landed on top of another sheep or player
        if ((collision.gameObject.tag == "Sheep" || collision.gameObject.tag == "Player") && (transform.position.y - collision.gameObject.transform.position.y) > heightTrigger )
        {
            Hop(jumpForce);
        }

        // hop away if player lands ontop of sheep
        if (collision.gameObject.tag == "Player" && (collision.gameObject.transform.position.y - transform.position.y) > heightTrigger && isGrounded)
        {
            Hop(throwForce);
        }
    }
}
