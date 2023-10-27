using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepController : MonoBehaviour
{
    // bark interaction
    private GameObject sheepDog;
    public bool isBarkedAt;
    private Rigidbody sheepRb;
    public float forwardBurstSpeed = 5000.0f;
    public float sidewardBurstSpeed = 2700.0f;
    private string[] fleeDirection = { "left", "right" };
    public int directionIndex;

    // trail lane alignment
    public GameObject[] trailLanes;
    public float alignSpeed = 1000.0f;


    // Start is called before the first frame update
    void Start()
    {
        sheepDog = GameObject.Find("Sheepdog");
        sheepRb = GetComponent<Rigidbody>();
        trailLanes = GameObject.Find("LaneManager").GetComponent<LaneManager>().trailLanes;
    }

    // Update is called once per frame
    void Update()
    {
        // sheep gradually falls behind
        //transform.Translate(Vector3.back * Time.deltaTime);

        // sheep realigns towards the center of the closest trail lane
        StartCoroutine("moveTowardsLaneMiddle");

        // burst of speed after getting barked at
        isBarkedAt = sheepDog.GetComponent<PlayerController>().hasBarked;
        if (isBarkedAt)
        {
            StartCoroutine("BeginFleeing", fleeDirection[directionIndex]);
            //StartCoroutine("moveTowardsLaneMiddle");
        }
        else
        {
            // sheep randomly decides a direction to flee from the bark
            directionIndex = Random.Range(0, fleeDirection.Length);
        }
    }

    IEnumerator BeginFleeing(string direction)
    {
        sheepRb.AddForce(forwardBurstSpeed * Time.deltaTime * transform.forward, ForceMode.Impulse);

        if (direction == "left")
        {
            sheepRb.AddForce(sidewardBurstSpeed * Time.deltaTime * -transform.right, ForceMode.Impulse);
        }
        else if (direction == "right")
        {
            sheepRb.AddForce(sidewardBurstSpeed * Time.deltaTime * transform.right, ForceMode.Impulse);
        }
        yield return null;
    }


    IEnumerator moveTowardsLaneMiddle()
    {
        float smallestDistanceFromLaneMiddle = 20.0f;
        int nearestLaneIndex = -1;

        // find the closest lane to align the sheep to centrally
        for (int laneIndex = 0; laneIndex<trailLanes.Length; laneIndex++)
        {
            float distanceFromLaneMiddle = Mathf.Abs(trailLanes[laneIndex].transform.position.x - transform.position.x);
            //Debug.Log("Dist from middle of lane " + laneIndex + " : " + distanceFromLaneMiddle);
            if (distanceFromLaneMiddle < smallestDistanceFromLaneMiddle)
            {
                smallestDistanceFromLaneMiddle = distanceFromLaneMiddle;
                nearestLaneIndex = laneIndex;
            }
        }

        //float targetPosX = trailLanes[nearestLaneIndex].transform.position.x - transform.position.x;
        //Vector3 alignPos = new Vector3(transform.position.x, transform.position.y, targetPosX);
        //Vector3 alignDirection = (alignPos.transform.position - transform.position).normalized;
        //sheepRb.AddForce(alignPos * alignSpeed * Time.deltaTime, ForceMode.Impulse);


        Vector3 alignDirection = (trailLanes[nearestLaneIndex].transform.position - transform.position).normalized;
        sheepRb.AddForce(alignDirection * alignSpeed);
        //Debug.Log("Alignment: " + alignDirection);

        yield return null;
    }
}
