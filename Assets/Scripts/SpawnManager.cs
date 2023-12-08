using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpawnManager : MonoBehaviour
{
    // wolf spawn
    public GameObject wolf;
    private float wolfSpawnUpperZ = -5.0f;
    private float wolfSpawnLowerZ = -15.0f;
    private float wolfSpawnX = 30.0f;
    private string[] wolfOrigin = { "left", "right" };
    Vector3 wolfSpawnPos;
    public float wolfSpawnInterval;
    private float wolfSpawnIntervalLower = 15.0f;
    private float wolfSpawnIntervalUpper = 18.0f;

    // wolf tracking
    private GameObject sheepDog;
    private float sheepDogPosZ;

    // obstacle spawn
    public GameObject[] obstacles;
    private GameObject[] trailLanes;
    private float[] trailLanesPos;

    Vector3 obstacleSpawnPos;
    private float obstacleSpawnInterval;
    private float obstacleSpawnIntervalLower = 5.0f;
    private float obstacleSpawnIntervalUpper = 6.0f;

    public GameObject[] laneWarningsText;
    private int obstacleSpawnPosIndex;

    // sheep spawn
    public GameObject straySheep;
    public int timeSinceLostSheep;
    private GameObject[] herd;
    public Vector3 straySheepSpawnPosition;
    public Vector3 straySheepTargetPosition;

    // background 
    public GameObject backgroundTree;

    // UI
    private bool isGameActive;
    private int timeRemaining;

    // audio
    private GameObject audioManager;

    // Start is called before the first frame update
    void Start()
    {
        sheepDog = GameObject.Find("Sheepdog");
        trailLanes = GameObject.Find("LaneManager").GetComponent<LaneManager>().trailLanes;

        // spawn position array
        trailLanesPos = new float[trailLanes.Length];
        for (int laneIndex = 0; laneIndex < trailLanes.Length; laneIndex++)
        {
            trailLanesPos[laneIndex] = trailLanes[laneIndex].transform.position.x;
        }

        audioManager = GameObject.Find("AudioManager");

        Invoke("SpawnWolf", wolfSpawnIntervalUpper);
        Invoke("SpawnObstacle", obstacleSpawnIntervalUpper);
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

    void SpawnBackground()
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

    bool CheckTimeSinceLostSheep(int targetSeconds)
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

    void SpawnStraySheep()
    {
        if (isGameActive)
        {
            if (CheckTimeSinceLostSheep(15))
            {
                string[] spawnSide = { "left", "right" };
                int sideIndex = Random.Range(0, spawnSide.Length);

                int straySheepSpawnPositionZ = Random.Range(0, 5);

                if (spawnSide[sideIndex] == "left")
                {
                    straySheepSpawnPosition = new Vector3(-12, 1, straySheepSpawnPositionZ);
                    straySheepTargetPosition = new Vector3(11, 1, straySheepSpawnPositionZ + 6);
                    Instantiate(straySheep, straySheepSpawnPosition, straySheep.transform.rotation);
                }
                else if (spawnSide[sideIndex] == "right")
                {
                    straySheepSpawnPosition = new Vector3(12, 1, straySheepSpawnPositionZ);
                    straySheepTargetPosition = new Vector3(-11, 1, straySheepSpawnPositionZ + 6);
                    Instantiate(straySheep, straySheepSpawnPosition, straySheep.transform.rotation);
                }

                foreach (GameObject sheep in herd)
                {
                    sheep.GetComponent<SheepController>().hasAvoidedStray = false;
                    sheep.GetComponent<SheepController>().hasEnteredStraySpace = false;
                }
            }
            Invoke("SpawnStraySheep", 1);
        }
    }

    void SpawnObstacle()
    {
        if (isGameActive)
        {
            int obstacleIndex = Random.Range(0, obstacles.Length);
            GameObject obstacle = obstacles[obstacleIndex];

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
            obstacleSpawnPos = new Vector3(trailLanesPos[obstacleSpawnPosIndex], obstacle.transform.position.y, 45.0f);

            Instantiate(obstacle, obstacleSpawnPos, obstacle.transform.rotation);

            obstacleSpawnInterval = Random.Range(obstacleSpawnIntervalLower, obstacleSpawnIntervalUpper);

            Invoke("SpawnObstacle", obstacleSpawnInterval);
        }
    }

    void SpawnWolf()
    {
        if (isGameActive)
        {
            //spawn wolf to come from either right or left side of trail 
            int wolfOriginIndex = Random.Range(0, wolfOrigin.Length);
            sheepDogPosZ = sheepDog.transform.position.z;

            if (wolfOrigin[wolfOriginIndex] == "right")
            {
                wolfSpawnPos = new Vector3(wolfSpawnX, 2.4f, sheepDogPosZ + Random.Range(wolfSpawnLowerZ, wolfSpawnUpperZ));
            }
            if (wolfOrigin[wolfOriginIndex] == "left")
            {
                wolfSpawnPos = new Vector3(-wolfSpawnX, 2.4f, sheepDogPosZ + Random.Range(wolfSpawnLowerZ, wolfSpawnUpperZ));
            }
            Instantiate(wolf, wolfSpawnPos, wolf.transform.rotation);

            wolfSpawnInterval = Random.Range(wolfSpawnIntervalLower,wolfSpawnIntervalUpper);

            Invoke("SpawnWolf", wolfSpawnInterval);
        }
    }
}
