using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private float obstacleSpawnIntervalUpper = 8.0f;


    // sound effects
    // REVISIT: Test once sound effects sourced
    private AudioSource spawnAudio;

    public AudioClip obstacleAlertSound;
    public AudioClip wolfGrowlSound;


    private bool isGameActive;

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

        //Invoke("SpawnWolf", wolfSpawnIntervalUpper);
        Invoke("SpawnObstacle", obstacleSpawnIntervalUpper);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        isGameActive = GameObject.Find("UIManager").GetComponent<UIManager>().isGameActive;
    }

    void SpawnObstacle()
    {
        if (isGameActive)
        {
            int obstacleIndex = Random.Range(0, obstacles.Length);
            GameObject obstacle = obstacles[obstacleIndex];

            int obstacleSpawnPosIndex;

            if (obstacle.name.Contains("Long"))
            {
                // middle lane
                obstacleSpawnPosIndex = 2;
            }
            else
            {
                // pick random lane
                obstacleSpawnPosIndex = Random.Range(0, trailLanesPos.Length); 
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
