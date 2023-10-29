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
    public float sidewardBurstSpeed = 2600.0f;
    private string[] fleeDirection = { "left", "right" };
    public int directionIndex;
    public GameObject[] trailLanes;
    public float[] laneBoundsLower;
    public float[] laneBoundsUpper;

    // trail lane alignment
    public GameObject[] trailLaneTargets;
    public float alignSpeed = 20.0f;
    private float xBoundary;


    // Start is called before the first frame update
    void Start()
    {
        sheepDog = GameObject.Find("Sheepdog");
        sheepRb = GetComponent<Rigidbody>();
        trailLaneTargets = GameObject.Find("LaneManager").GetComponent<LaneManager>().trailLaneTargets;
        trailLanes = GameObject.Find("LaneManager").GetComponent<LaneManager>().trailLanes;

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

        xBoundary = 4.6f;
        pastBarkState = isBarkedAt;
    }

    // Update is called once per frame
    void Update()
    {
        // sheep gradually falls behind
        //transform.Translate(Vector3.back * Time.deltaTime);

        // sheep realigns towards the center of the closest trail lane
        StartCoroutine(moveTowardsLaneMiddle());

        // burst of speed after getting barked at within lane
        isBarkedAt = sheepDog.GetComponent<PlayerController>().hasBarked;


        // check for a low to high change of bark state
        if (isBarkedAt != pastBarkState && !pastBarkState)
        {

            float sheepDogPosX = sheepDog.transform.position.x;
            float sheepDogPosZ = sheepDog.transform.position.z;
            float sheepPosX = transform.position.x;
            float sheepPosZ = transform.position.z;

            StartCoroutine(CheckLane(sheepDogPosX, sheepPosX, sheepDogPosZ, sheepPosZ));
        }
        else if (!isBarkedAt)
        {
            // sheep randomly decides a direction to flee from the bark
            directionIndex = Random.Range(0, fleeDirection.Length);
        }

        pastBarkState = isBarkedAt;
    }

    IEnumerator CheckLane(float sheepDogPosX, float sheepPosX, float sheepDogPosZ, float sheepPosZ)
    {
        for (int laneIndex = 0; laneIndex < trailLanes.Length; laneIndex++)
        {
            bool isBarkWithinLane = sheepDogPosX >= laneBoundsLower[laneIndex] && sheepDogPosX <= laneBoundsUpper[laneIndex];
            bool isSheepWithinLane = sheepPosX >= laneBoundsLower[laneIndex] && sheepPosX <= laneBoundsUpper[laneIndex];
            bool isAheadOfBark = sheepPosZ >= sheepDogPosZ;

            if (isBarkWithinLane && isSheepWithinLane & isAheadOfBark)
            {
                StartCoroutine(Flee(fleeDirection[directionIndex], sheepPosX));
            }
        }

        yield return null;
    }

    IEnumerator Flee(string direction, float sheepPosX)
    {
        sheepRb.AddForce(forwardBurstSpeed * transform.forward, ForceMode.Impulse);

        if (direction == "left" && sheepPosX > -xBoundary)
        {
            // if furthest left lane, then move right
            sheepRb.AddForce(sidewardBurstSpeed * -transform.right, ForceMode.Impulse);
        }
        else if (direction == "left" && sheepPosX < -xBoundary)
        {
            sheepRb.AddForce(sidewardBurstSpeed * transform.right, ForceMode.Impulse);
        }
        else if (direction == "right" && sheepPosX < xBoundary)
        {
            // if furthest right lane, then move left
            sheepRb.AddForce(sidewardBurstSpeed * transform.right, ForceMode.Impulse);
        }
        else if (direction == "right" && sheepPosX > xBoundary)
        {
            sheepRb.AddForce(sidewardBurstSpeed * -transform.right, ForceMode.Impulse);
        }
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
}
