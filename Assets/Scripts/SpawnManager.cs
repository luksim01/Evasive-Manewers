using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpawnManager : MonoBehaviour
{
    // wolf spawn
    public GameObject wolf;
    private GameObject sheepDog;
    private float wolfSpawnUpperZ = -5.0f;
    private float wolfSpawnLowerZ = -15.0f;
    private float wolfSpawnX = 30.0f;
    private float sheepDogPosZ;
    private string[] wolfOrigin = { "left", "right" };
    Vector3 wolfSpawnPos;

    public float wolfSpawnInterval;
    private float wolfSpawnIntervalLower = 15.0f;
    private float wolfSpawnIntervalUpper = 18.0f;

    // obstacle spawn
    public GameObject[] obstacles;
    public GameObject[] trailLanes;
    public float[] trailLanesPos;

    Vector3 obstacleSpawnPos;
    public float obstacleSpawnInterval;
    private float obstacleSpawnIntervalLower = 5.0f;
    private float obstacleSpawnIntervalUpper = 6.0f;

    public GameObject backgroundTree;

    // sound effects
    // REVISIT: Test once sound effects sourced
    private AudioSource spawnAudio;

    public AudioClip obstacleAlertSound;
    public AudioClip wolfGrowlSound;


    private bool isGameActive;
    private int timeRemaining;

    public GameObject[] laneWarningsText;
    public int obstacleSpawnPosIndex;

    // Start is called before the first frame update
    void Start()
    {
        sheepDog = GameObject.Find("Sheepdog");
        trailLanes = GameObject.Find("LaneManager").GetComponent<LaneManager>().trailLanes;
        spawnAudio = GetComponent<AudioSource>();

        trailLanesPos = new float[trailLanes.Length];

        for (int laneIndex = 0; laneIndex < trailLanes.Length; laneIndex++)
        {
            trailLanesPos[laneIndex] = trailLanes[laneIndex].transform.position.x;
        }

        //for (int laneIndex = 0; laneIndex < laneWarningsText.Length; laneIndex++)
        //{
        //    //laneWarningsText[laneIndex].CrossFadeAlpha(0.0f, 0.0f, false); ;
        //}

        Invoke("SpawnWolf", wolfSpawnIntervalUpper);
        Invoke("SpawnObstacle", obstacleSpawnIntervalUpper);
        Invoke("SpawnBackground", 1);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        isGameActive = GameObject.Find("UIManager").GetComponent<UIManager>().isGameActive;
        timeRemaining = GameObject.Find("UIManager").GetComponent<UIManager>().timeRemaining;
    }

    IEnumerator DisplayLaneWarning(int singleLaneIndex, string noOfLanes)
    {
        if(noOfLanes == "Single")
        {
            Debug.Log("lane warn pos i: " + singleLaneIndex);
            laneWarningsText[singleLaneIndex].SetActive(true);
            //laneWarningsText[singleLaneIndex].CrossFadeAlpha(1.0f, 0.4f, false);
            yield return new WaitForSeconds(2);
            laneWarningsText[singleLaneIndex].SetActive(false);
            //laneWarningsText[singleLaneIndex].CrossFadeAlpha(0.0f, 0.4f, false);
        }
        else if (noOfLanes == "All")
        {
            for (int laneIndex = 0; laneIndex < laneWarningsText.Length; laneIndex++)
            {
                laneWarningsText[laneIndex].SetActive(true);
                //laneWarningsText[laneIndex].CrossFadeAlpha(1.0f, 0.4f, false);
            }
            yield return new WaitForSeconds(2);
            for (int laneIndex = 0; laneIndex < laneWarningsText.Length; laneIndex++)
            {
                laneWarningsText[laneIndex].SetActive(false);
                //laneWarningsText[laneIndex].CrossFadeAlpha(0.0f, 0.4f, false);
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

    void SpawnObstacle()
    {
        if (isGameActive)
        {
            int obstacleIndex = Random.Range(0, obstacles.Length);
            GameObject obstacle = obstacles[obstacleIndex];

            //int obstacleSpawnPosIndex;

            if (obstacle.name.Contains("Long"))
            {
                // middle lane
                obstacleSpawnPosIndex = 2;
                StartCoroutine(DisplayLaneWarning(obstacleSpawnPosIndex, "All"));
            }
            else
            {
                // pick random lane
                obstacleSpawnPosIndex = Random.Range(0, trailLanesPos.Length);
                Debug.Log("obstacle spawn pos i: " + obstacleSpawnPosIndex);
                StartCoroutine(DisplayLaneWarning(obstacleSpawnPosIndex, "Single"));
            }
            obstacleSpawnPos = new Vector3(trailLanesPos[obstacleSpawnPosIndex], obstacle.transform.position.y, 45.0f);

            Instantiate(obstacle, obstacleSpawnPos, obstacle.transform.rotation);

            // REVISIT: Test once sound effects sourced
            //spawnAudio.PlayOneShot(obstacleAlertSound, 1.0f);

            obstacleSpawnInterval = Random.Range(obstacleSpawnIntervalLower, obstacleSpawnIntervalUpper);

            Invoke("SpawnObstacle", obstacleSpawnInterval);
        }
    }

    void SpawnWolf()
    {
        if (isGameActive)
        {
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

            // REVISIT: Test once sound effects sourced
            //spawnAudio.PlayOneShot(wolfGrowlSound, 1.0f);

            wolfSpawnInterval = Random.Range(wolfSpawnIntervalLower,wolfSpawnIntervalUpper);

            Invoke("SpawnWolf", wolfSpawnInterval);
        }
    }
}
