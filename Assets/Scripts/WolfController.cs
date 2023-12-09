using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfController : MonoBehaviour
{
    private float speed = 10.0f;
    private GameObject sheepDog;
    private Vector3 alignDirection;
    private Vector3 sheepDogProximity;
    private Vector3 collisionCourse = new Vector3(0, 0, 0);
    private int bounds = 40;
    private float wolfStartPosX;

    private float sheepDogProximityX;
    private float sheepDogProximityZ;

    public bool hasBitten = false;
    private bool isGameActive;

    private bool isCharging = false;

    private GameObject[] herd;

    // hunt target
    string[] huntTarget = { "player", "sheep" };
    int targetIndex;
    private GameObject targetSheep;
    public bool hasTargetedSheepdog = false;
    public bool hasTargetedHerd = false;
    public bool hasTargetedSheep = false;
    private GameObject spawnManager;

    private bool isBarkedAt;
    private float sheepDogBarkProximityXLimit = 6.0f;
    private float sheepDogBarkProximityZLimit = 10.0f;
    // interaction boundaries
    private float xBoundary = 7.6f;
    private float xAvoidDistance = 1.2f;


    // Start is called before the first frame update
    void Start()
    {
        sheepDog = GameObject.Find("Sheepdog");
        wolfStartPosX = transform.position.x;

        spawnManager = GameObject.Find("SpawnManager");

        hasTargetedSheepdog = spawnManager.GetComponent<SpawnManager>().hasTargetedSheepdog;
        hasTargetedHerd = spawnManager.GetComponent<SpawnManager>().hasTargetedHerd;
    }

    // Update is called once per frame
    void Update()
    {
        isGameActive = GameObject.Find("UIManager").GetComponent<UIManager>().isGameActive;

        if (isGameActive)
        {
            herd = GameObject.FindGameObjectsWithTag("Sheep");

            if (hasTargetedSheepdog)
            {
                HuntPlayer();
            }
            else if (hasTargetedHerd)
            {
                HuntSheep();
            }
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

    void HuntSheep()
    {
        // keep track of barks
        isBarkedAt = sheepDog.GetComponent<PlayerController>().hasBarked;

        if (!hasTargetedSheep)
        {
            int targetIndex = -1;
            float closestSheep = bounds*2;

            for (int i = 0; i < herd.Length; i++)
            {
                if(Mathf.Abs(herd[i].transform.position.x - transform.position.x) < closestSheep)
                {
                    closestSheep = Mathf.Abs(herd[i].transform.position.x - transform.position.x);
                    targetIndex = i;
                }
            }

            targetSheep = herd[targetIndex];
            hasTargetedSheep = true;
            targetSheep.tag = "Hunted";
        }

        if (targetSheep)
        {
            // sheep proximity
            float sheepProximityX = targetSheep.transform.position.x - transform.position.x;
            float sheepProximityZ = targetSheep.transform.position.z - transform.position.z;
            Vector3 sheepProximity = new Vector3(sheepProximityX, 0, sheepProximityZ);

            if (Mathf.Abs(sheepProximityX) > 2 || Mathf.Abs(sheepProximityZ) > 4)
            {
                alignDirection = (sheepProximity).normalized;
                transform.Translate(alignDirection * speed * 0.8f * Time.deltaTime);
            }

            sheepDogProximityX = sheepDog.transform.position.x - transform.position.x;
            sheepDogProximityZ = sheepDog.transform.position.z - transform.position.z;
            sheepDogProximity = new Vector3(sheepDogProximityX, 0, sheepDogProximityZ);

            // wolf hunt sequence can be interrupted
            if (isBarkedAt &&
                Mathf.Abs(sheepDogProximityX) < sheepDogBarkProximityXLimit / 2 &&
                Mathf.Abs(sheepDogProximityZ) < sheepDogBarkProximityZLimit / 2 &&
                transform.position.x < xBoundary - xAvoidDistance &&
                transform.position.x > -xBoundary + xAvoidDistance)
            {
                targetSheep.tag = "Sheep";
            }

            if (targetSheep.tag == "Sheep")
            {
                alignDirection = (new Vector3(0, 0, 50 - transform.position.x)).normalized;
                transform.Translate(alignDirection * speed * 2 * Time.deltaTime);
            }
        }
        else
        {
            alignDirection = (new Vector3(0, 0, 50 - transform.position.x)).normalized;
            transform.Translate(alignDirection * speed * 2 * Time.deltaTime);
        }
       

        Boundaries(12, -13, bounds, -bounds);
    }

    void HuntPlayer()
    {
        sheepDogProximityX = sheepDog.transform.position.x - transform.position.x;
        sheepDogProximityZ = sheepDog.transform.position.z - transform.position.z;
        sheepDogProximity = new Vector3(sheepDogProximityX, 0, sheepDogProximityZ);

        // track player position
        if (wolfStartPosX > 0 && !isCharging)
        {
            collisionCourse = sheepDogProximity;
            Track(sheepDogProximity);
        }

        if (wolfStartPosX < 0 && !isCharging)
        {
            collisionCourse = sheepDogProximity;
            Track(sheepDogProximity);
        }

        // once close enough, charge at player in one direction
        if (sheepDogProximity.z <= 9 && transform.position.x < 7.7f && transform.position.x > -7.7f)
        {
            isCharging = true;
        }
        if (isCharging)
        {
            Track(collisionCourse);
        }

        // destroy if out of bounds
        Boundaries(bounds, -bounds, bounds, -bounds);
    }

    void Track(Vector3 direction)
    {
        alignDirection = (direction).normalized;
        if (alignDirection != Vector3.zero && !hasBitten)
        {
            transform.rotation = Quaternion.LookRotation(alignDirection);
        }
        transform.Translate(alignDirection * speed * Time.deltaTime);
    }
}
