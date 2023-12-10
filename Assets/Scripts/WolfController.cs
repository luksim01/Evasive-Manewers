using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfController : MonoBehaviour
{
    // wolf 
    private float wolfStartPosX;
    public bool hasBitten = false;

    // hunt target
    private GameObject[] herd;
    private GameObject targetSheep;
    public bool hasTargetedSheepdog = false;
    public bool hasTargetedHerd = false;
    public bool hasTargetedSheep = false;
    private GameObject spawnManager;

    // track player 
    private GameObject sheepDog;
    private bool isBarkedAt;
    private Vector3 collisionCourse;
    private bool isCharging = false;

    // UI
    private bool isGameActive;


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

    private void DestroyBoundaries(float xBoundaryRight, float xBoundaryLeft, float zBoundaryForward, float zBoundaryBackward)
    {
        // destroy if out of bounds
        if (transform.position.x > xBoundaryRight || transform.position.x < xBoundaryLeft)
        {
            Destroy(gameObject);
        }
        if (transform.position.z > zBoundaryForward || transform.position.z < zBoundaryBackward)
        {
            Destroy(gameObject);
        }
    }

    int IdentifyClosestSheepIndex()
    {
        int targetIndex = -1;
        float closestSheep = 80;

        for (int i = 0; i < herd.Length; i++)
        {
            if (Mathf.Abs(herd[i].transform.position.x - transform.position.x) < closestSheep)
            {
                closestSheep = Mathf.Abs(herd[i].transform.position.x - transform.position.x);
                targetIndex = i;
            }
        }
        return targetIndex;
    }

    private void HuntSheep()
    {
        // keep track of bark
        isBarkedAt = sheepDog.GetComponent<PlayerController>().hasBarkedMove;

        // target one sheep
        if (!hasTargetedSheep)
        {
            targetSheep = herd[IdentifyClosestSheepIndex()];
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
                TravelTowards(sheepProximity, 8.0f);
            }

            // player proximity
            float sheepDogProximityX = sheepDog.transform.position.x - transform.position.x;
            float sheepDogProximityZ = sheepDog.transform.position.z - transform.position.z;

            // wolf hunt sequence can be interrupted
            if (isBarkedAt &&
                Mathf.Abs(sheepDogProximityX) < 3 &&
                Mathf.Abs(sheepDogProximityZ) < 5 &&
                transform.position.x < 5.4 &&
                transform.position.x > -5.4)
            {
                targetSheep.tag = "Sheep";
            }

            if (targetSheep.tag == "Sheep")
            {
                TravelTowards(new Vector3(transform.position.x, 0, 50 - transform.position.z), 18.0f);
            }
        }
        else
        {
            TravelTowards(new Vector3(transform.position.x, 0, 50 - transform.position.z), 18.0f);
        }

        float xBoundaryLeft = 12.0f;
        float xBoundaryRight = -13.0f;
        float zBoundary = 40.0f;

        DestroyBoundaries(xBoundaryLeft, xBoundaryRight, zBoundary, -zBoundary);
}

    private void HuntPlayer()
    {
        float sheepDogProximityX = sheepDog.transform.position.x - transform.position.x;
        float sheepDogProximityZ = sheepDog.transform.position.z - transform.position.z;
        Vector3 sheepDogProximity = new Vector3(sheepDogProximityX, 0, sheepDogProximityZ);

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

        float xBoundaryLeft = 40.0f;
        float xBoundaryRight = -40.0f;
        float zBoundary = 40.0f;

        DestroyBoundaries(xBoundaryLeft, xBoundaryRight, zBoundary, -zBoundary);
    }

    private void Track(Vector3 direction)
    {
        Vector3 alignDirection = (direction).normalized;
        if (alignDirection != Vector3.zero && !hasBitten)
        {
            transform.rotation = Quaternion.LookRotation(alignDirection);
        }
        transform.Translate(alignDirection * 10.0f * Time.deltaTime);
    }


    private void TravelTowards(Vector3 direction, float speed)
    {
        Vector3 alignDirection = (direction).normalized;
        transform.Translate(alignDirection * speed * Time.deltaTime);
    }
}
