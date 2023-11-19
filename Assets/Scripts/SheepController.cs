using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepController : MonoBehaviour
{
    // bark interaction
    private GameObject sheepDog;
    public bool isBarkedAt;
    public bool pastBarkState;
    private Rigidbody sheepRb;
    public float forwardBurstSpeed = 2000.0f;
    public float sidewardBurstSpeed = 2000.0f;
    private string[] fleeDirection = { "left", "right" };
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
    private float xBoundary = 7.3f;
    private int zBoundary = 35;

    // sound effects
    // REVISIT: Test once sound effects sourced
    private AudioSource sheepAudio;
    public AudioClip collisionSound;


    // Start is called before the first frame update
    void Start()
    {
        sheepDog = GameObject.Find("Sheepdog");
        sheepRb = GetComponent<Rigidbody>();
        trailLaneTargets = GameObject.Find("LaneManager").GetComponent<LaneManager>().trailLaneTargets;
        trailLanes = GameObject.Find("LaneManager").GetComponent<LaneManager>().trailLanes;
        sheepAudio = GetComponent<AudioSource>();

        // lane boundaries creation
        laneBoundsLower = new float[trailLanes.Length];
        laneBoundsUpper = new float[trailLanes.Length];

        for (int laneIndex = 0; laneIndex < trailLanes.Length; laneIndex++)
        {
            float posX = trailLanes[laneIndex].transform.position.x;
            float width = trailLanes[laneIndex].transform.localScale.x;
            laneBoundsLower[laneIndex] = posX - (width / 2);
            laneBoundsUpper[laneIndex] = posX + (width / 2);
        }

        //xFleeBoundary = 4.6f;
        pastBarkState = isBarkedAt;
    }

    // Update is called once per frame
    void Update()
    {
        // sheep gradually falls behind
        //transform.Translate(Vector3.back * Time.deltaTime);

        // sheep realigns towards the center of the closest trail lane
        if (isSlowingDown)
        {
            //StartCoroutine(moveTowardsLaneMiddle());
        }
        
        // burst of speed after getting barked at within lane
        isBarkedAt = sheepDog.GetComponent<PlayerController>().hasBarked;


        // check for a low to high change of bark state
        if (isBarkedAt != pastBarkState && !pastBarkState)
        {

            float sheepDogPosX = sheepDog.transform.position.x;
            float sheepDogPosZ = sheepDog.transform.position.z;
            float sheepPosX = transform.position.x;
            float sheepPosZ = transform.position.z;

            //StartCoroutine(CheckLane(sheepDogPosX, sheepPosX, sheepDogPosZ, sheepPosZ));
        }
        else if (!isBarkedAt)
        {
            // sheep randomly decides a direction to flee from the bark
            directionIndex = Random.Range(0, fleeDirection.Length);
        }

        pastBarkState = isBarkedAt;

        // sheep is lost if ahead or behind too far
        if (transform.position.z > zBoundary || transform.position.z < -zBoundary)
        {
            Destroy(gameObject);
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
    }

    IEnumerator SlowdownCooldown()
    {
        yield return new WaitForSeconds(slowdownCooldownTime);
        isSlowingDown = true;
    }

    IEnumerator CheckLane(float sheepDogPosX, float sheepPosX, float sheepDogPosZ, float sheepPosZ)
    {
        // checks bark and sheep are in the same lane and sheep is ahead of bark
        for (int laneIndex = 0; laneIndex < trailLanes.Length; laneIndex++)
        {
            bool isBarkWithinLane = sheepDogPosX >= laneBoundsLower[laneIndex] && sheepDogPosX <= laneBoundsUpper[laneIndex];
            bool isSheepWithinLane = sheepPosX >= laneBoundsLower[laneIndex] && sheepPosX <= laneBoundsUpper[laneIndex];
            bool isAheadOfBark = sheepPosZ >= sheepDogPosZ;

            if (isBarkWithinLane && isSheepWithinLane && isAheadOfBark)
            {
                StartCoroutine(Flee(fleeDirection[directionIndex], sheepPosX));

                isSlowingDown = false;
                if (!isSlowingDown)
                {
                    StartCoroutine(SlowdownCooldown());
                }
            }
        }

        yield return null;
    }

    IEnumerator Flee(string direction, float sheepPosX)
    {
        if (direction == "left" && sheepPosX > -xFleeBoundary)
        {
            // if furthest left lane, then move right
            //sheepRb.AddForce(sidewardBurstSpeed * -Vector3.right, ForceMode.Impulse);
            //sheepRb.AddForce(sidewardBurstSpeed * -transform.right, ForceMode.Impulse);
            transform.Translate(new Vector3(-3.1f, 0, 1));// * Time.deltaTime);
        }
        else if (direction == "left" && sheepPosX < -xFleeBoundary)
        {
            //sheepRb.AddForce(sidewardBurstSpeed * Vector3.right, ForceMode.Impulse);
            //sheepRb.AddForce(sidewardBurstSpeed * transform.right, ForceMode.Impulse);
            transform.Translate(new Vector3(3.1f, 0, 1));// * Time.deltaTime);
        }
        else if (direction == "right" && sheepPosX < xFleeBoundary)
        {
            // if furthest right lane, then move left
            //sheepRb.AddForce(sidewardBurstSpeed * Vector3.right, ForceMode.Impulse);
            //sheepRb.AddForce(sidewardBurstSpeed * transform.right, ForceMode.Impulse);
            transform.Translate(new Vector3(3.1f, 0, 1));// * Time.deltaTime);
        }
        else if (direction == "right" && sheepPosX > xFleeBoundary)
        {
            //sheepRb.AddForce(sidewardBurstSpeed * -Vector3.right, ForceMode.Impulse);
            //sheepRb.AddForce(sidewardBurstSpeed * -transform.right, ForceMode.Impulse);
            transform.Translate(new Vector3(-3.1f, 0, 1));// * Time.deltaTime);
        }

        //transform.Translate(new Vector3(0, 0, 3.1f));// * Time.deltaTime);
        //sheepRb.AddForce(forwardBurstSpeed * transform.forward, ForceMode.Impulse);

        yield return null;
    }

    IEnumerator moveTowardsLaneMiddle()
    {
        float smallestDistanceFromLaneMiddle = 20.0f;
        int nearestLaneIndex = -1;

        // find the closest lane to align the sheep to centrally
        for (int laneIndex = 0; laneIndex< trailLaneTargets.Length; laneIndex++)
        {
            float distanceFromLaneMiddle = Mathf.Abs(trailLaneTargets[laneIndex].transform.position.x - transform.position.x);
            if (distanceFromLaneMiddle < smallestDistanceFromLaneMiddle)
            {
                smallestDistanceFromLaneMiddle = distanceFromLaneMiddle;
                nearestLaneIndex = laneIndex;
            }
        }

        // sheep gradually falls behind and aligns itself centrally to the closest lane center
        Vector3 alignDirection = (trailLaneTargets[nearestLaneIndex].transform.position - transform.position);
        sheepRb.AddForce(alignDirection * alignSpeed);

        yield return null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Obstacle")
        {
            Debug.Log("Sheep Lost!");
            // REVISIT: Test once sound effects sourced
            //sheepAudio.PlayOneShot(collisionSound, 1.0f);
            Destroy(gameObject);
        }
    }
}
