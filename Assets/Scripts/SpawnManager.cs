using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpawnManager : MonoBehaviour
{
    // wolf spawning
    public GameObject wolf;
    public bool hasTargetedSheepdog = false;
    public bool hasTargetedHerd = false;
    private GameObject sheepDog;

    // obstacle spawning
    public GameObject[] obstacles;
    private float[] trailLanesPos;
    public GameObject[] laneWarningsText;

    // sheep spawning
    public GameObject straySheep;
    public int timeSinceLostSheep;
    private GameObject[] herd;
    public Vector3 straySheepSpawnPosition;
    public Vector3 straySheepTargetPosition;
    public int spawnInterval = 20;

    // background spawning
    public GameObject backgroundTree;

    // UI
    private bool isGameActive;
    private int timeRemaining;

    // global audio
    private GameObject audioManager;

    // Start is called before the first frame update
    void Start()
    {
        sheepDog = GameObject.Find("Sheepdog");

        // spawn position array
        GameObject[] trailLanes = GameObject.Find("LaneManager").GetComponent<LaneManager>().trailLanes;
        trailLanesPos = new float[trailLanes.Length];
        for (int laneIndex = 0; laneIndex < trailLanes.Length; laneIndex++)
        {
            trailLanesPos[laneIndex] = trailLanes[laneIndex].transform.position.x;
        }

        audioManager = GameObject.Find("AudioManager");

        InvokeEncounter(8);
        Invoke("SpawnBackground", 1);
        timeSinceLostSheep = 0;
        Invoke("SpawnStraySheep", 3);
    }

    void Update()
    {
        isGameActive = GameObject.Find("UIManager").GetComponent<UIManager>().isGameActive;
        timeRemaining = GameObject.Find("UIManager").GetComponent<UIManager>().timeRemaining;

        // keep track of herd
        herd = GameObject.FindGameObjectsWithTag("Sheep");
    }

    private void InvokeEncounter(float encounterInterval)
    {
        switch (Random.Range(0, 2))
        {
            case 0:
                Invoke("SpawnWolf", encounterInterval);
                break;
            case 1:
                Invoke("SpawnObstacle", encounterInterval);
                break;
            default:
                break;
        }
    }

    IEnumerator DisplayLaneWarning(int singleLaneIndex, string noOfLanes)
    {
        if(noOfLanes == "Single")
        {
            audioManager.GetComponent<AudioManager>().hasDetectedWarnSingle = true;
            laneWarningsText[singleLaneIndex].SetActive(true);
            yield return new WaitForSeconds(2);
            laneWarningsText[singleLaneIndex].SetActive(false);
        }
        else if (noOfLanes == "All")
        {
            audioManager.GetComponent<AudioManager>().hasDetectedWarnAll = true;
            for (int laneIndex = 0; laneIndex < laneWarningsText.Length; laneIndex++)
            {
                laneWarningsText[laneIndex].SetActive(true);
            }
            yield return new WaitForSeconds(2);
            for (int laneIndex = 0; laneIndex < laneWarningsText.Length; laneIndex++)
            {
                laneWarningsText[laneIndex].SetActive(false);
            }
        }
    }

    private void SpawnBackground()
    {
        if (isGameActive)
        {
            Instantiate(backgroundTree, new Vector3(14, 0, 40), backgroundTree.transform.rotation);
            Instantiate(backgroundTree, new Vector3(-9, 0, 40), backgroundTree.transform.rotation);
            if(timeRemaining > 10)
            {
                Invoke("SpawnBackground", 1);
            }
        }
    }

    public bool CheckTimeSinceLostSheep(int targetSeconds)
    {
        timeSinceLostSheep += 1;
        if (timeSinceLostSheep >= targetSeconds)
        {
            timeSinceLostSheep = 0;
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SpawnStraySheep()
    {
        if (isGameActive)
        {
            if (CheckTimeSinceLostSheep(spawnInterval))
            {
                string[] spawnSide = { "left", "right" };
                int sideIndex = Random.Range(0, spawnSide.Length);


                int straySheepSpawnPositionZ = Random.Range(0, 5);
                int straySheepTargetPositionZ = straySheepSpawnPositionZ + 6;

                if (spawnSide[sideIndex] == "left")
                {
                    straySheepSpawnPosition = new Vector3(-12, 1, straySheepSpawnPositionZ);
                    straySheepTargetPosition = new Vector3(11, 1, straySheepTargetPositionZ);
                    Instantiate(straySheep, straySheepSpawnPosition, straySheep.transform.rotation);
                }
                else if (spawnSide[sideIndex] == "right")
                {
                    straySheepSpawnPosition = new Vector3(12, 1, straySheepSpawnPositionZ);
                    straySheepTargetPosition = new Vector3(-11, 1, straySheepTargetPositionZ);
                    Instantiate(straySheep, straySheepSpawnPosition, straySheep.transform.rotation);
                }
            }
            Invoke("SpawnStraySheep", 1);
        }
    }

    private void SpawnObstacle()
    {
        if (isGameActive)
        {
            int obstacleIndex = Random.Range(0, obstacles.Length);
            GameObject obstacle = obstacles[obstacleIndex];

            int obstacleSpawnPosIndex;

            if (obstacle.name.Contains("Long"))
            {
                // spawn in middle lane
                obstacleSpawnPosIndex = 2;
                StartCoroutine(DisplayLaneWarning(obstacleSpawnPosIndex, "All"));
            }
            else
            {
                // spawn in random lane
                obstacleSpawnPosIndex = Random.Range(0, trailLanesPos.Length);
                StartCoroutine(DisplayLaneWarning(obstacleSpawnPosIndex, "Single"));
            }

            Vector3 obstacleSpawnPos = new Vector3(trailLanesPos[obstacleSpawnPosIndex], obstacle.transform.position.y, 40.0f);

            Instantiate(obstacle, obstacleSpawnPos, obstacle.transform.rotation);

            InvokeEncounter(8);
        }
    }

    private void SpawnWolf()
    {
        if (isGameActive)
        {
            if (herd.Length > 0)
            {
                // choose target for wolf to hunt
                ChooseTarget();

                // choose spawn position for wolf
                Instantiate(wolf, ChooseSpawnPosition(), wolf.transform.rotation);

                // sheep reaction to wolf is reset
                foreach (GameObject sheep in herd)
                {
                    sheep.GetComponent<SheepController>().hasAvoidedWolf = false;
                    sheep.GetComponent<SheepController>().hasEnteredWolfSpace = false;
                }
            }

            InvokeEncounter(8);
        }
    }

    private void ChooseTarget()
    {
        int targetIndex;
        string[] huntTarget = { "player", "sheep" };

        if (herd.Length > 0)
        {
            targetIndex = Random.Range(0, huntTarget.Length);
        }
        else
        {
            targetIndex = 0;
        }

        if (huntTarget[targetIndex] == "player")
        {
            hasTargetedSheepdog = true;
            hasTargetedHerd = false;
        }
        else if (huntTarget[targetIndex] == "sheep")
        {
            hasTargetedHerd = true;
            hasTargetedSheepdog = false;
        }
    }

    private Vector3 ChooseSpawnPosition()
    {
        // spawn wolf to come from either right or left side of trail
        string[] wolfOrigin = { "left", "right" };
        int wolfOriginIndex = Random.Range(0, wolfOrigin.Length);

        // wolf spawn position changes based on hunt target
        float sheepDogPosZ = sheepDog.transform.position.z;

        Vector3 wolfSpawnPos = new();

        if (wolfOrigin[wolfOriginIndex] == "right")
        {
            if (hasTargetedSheepdog)
            {
                wolfSpawnPos = new Vector3(30.0f, 0, sheepDogPosZ + Random.Range(-15, -5));
            }
            else if (hasTargetedHerd)
            {
                wolfSpawnPos = new Vector3(11.0f, 0, Random.Range(-4, 1));
            }
        }
        if (wolfOrigin[wolfOriginIndex] == "left")
        {
            if (hasTargetedSheepdog)
            {
                wolfSpawnPos = new Vector3(-30.0f, 0, sheepDogPosZ + Random.Range(-15, -5));
            }
            else if (hasTargetedHerd)
            {
                wolfSpawnPos = new Vector3(-12.0f, 0, Random.Range(-4, 1));
            }
        }
        return wolfSpawnPos;
    }
}
