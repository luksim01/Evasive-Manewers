using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpawnManager : MonoBehaviour, ISpawnManager
{
    // wolf spawning
    public GameObject wolf;
    public bool HasTargetedSheepdog { get; set; }
    public bool HasTargetedHerd { get; set; }

    // obstacle spawning
    public GameObject[] obstacles;
    private float[] trailLanesPos;
    public GameObject[] laneWarningsText;

    // sheep spawning
    public GameObject straySheep;
    public GameObject[] Herd { get; set; }
    public int TimeSinceLostSheep { get; set; }
    public Vector3 StraySheepSpawnPosition { get; set; }
    public Vector3 StraySheepTargetPosition { get; set; }
    public int spawnInterval = 20;

    // background spawning
    public GameObject backgroundTree;
    private List<GameObject> backgroundTreePool;
    private int treeAmountToPool = 32;

    // lanes
    [SerializeField] private GameObject[] trailLanes;

    // audio
    private IAudioManager _audioManager;

    // ui
    private bool isGameActive;
    private int timeRemaining;
    private IUIManager _uiManager;

    // player
    private IPlayerController _sheepdog;

    // sheep
    private SheepController _sheepController;

    // spawn manager
    private SpawnManager _spawnManager;

    // dependancies
    public void SetDependencies(IAudioManager audioManager, IUIManager uiManager, IPlayerController playerController, SpawnManager spawnManager)
    {
        _audioManager = audioManager;
        _uiManager = uiManager;
        _sheepdog = playerController;
        _spawnManager = spawnManager;
    }

    public void SetSheepDependancy(SheepController sheepController)
    {
        _sheepController = sheepController;
    }

    // dependancy manager
    [SerializeField] private DependancyManager dependancyManager;

    private void Awake()
    {
        GameObject initialSheep1 = Instantiate(straySheep, new Vector3(0, 0, 8), straySheep.transform.rotation);
        initialSheep1.tag = "Sheep";
        SheepController sheepController1 = initialSheep1.GetComponent<SheepController>();
        dependancyManager.InjectSheepControllerDependencies(sheepController1);

        GameObject initialSheep2 = Instantiate(straySheep, new Vector3(-3, 0, 4), straySheep.transform.rotation);
        initialSheep2.tag = "Sheep";
        SheepController sheepController2 = initialSheep2.GetComponent<SheepController>();
        dependancyManager.InjectSheepControllerDependencies(sheepController2);

        GameObject initialSheep3 = Instantiate(straySheep, new Vector3(3, 0, 4), straySheep.transform.rotation);
        initialSheep3.tag = "Sheep";
        SheepController sheepController3 = initialSheep3.GetComponent<SheepController>();
        dependancyManager.InjectSheepControllerDependencies(sheepController3);
    }

    void Start()
    {
        // spawn position array
        trailLanesPos = new float[trailLanes.Length];
        for (int laneIndex = 0; laneIndex < trailLanes.Length; laneIndex++)
        {
            trailLanesPos[laneIndex] = trailLanes[laneIndex].transform.position.x;
        }

        // creating a pool of background trees
        backgroundTreePool = new List<GameObject>();
        GameObject backgroundTreeToPool;
        for (int i = 0; i < treeAmountToPool; i++)
        {
            // instantiate water tiles as children of the manager
            backgroundTreeToPool = Instantiate(backgroundTree, this.transform);
            backgroundTreeToPool.SetActive(false);
            backgroundTreePool.Add(backgroundTreeToPool);
        }

        InvokeEncounter(8);
        Invoke("SpawnBackground", 1);
        TimeSinceLostSheep = 0;
        Invoke("SpawnStraySheep", 3);
    }

    void Update()
    {
        isGameActive = _uiManager.IsGameActive;
        timeRemaining = _uiManager.TimeRemaining;

        // keep track of herd
        Herd = GameObject.FindGameObjectsWithTag("Sheep");
    }

    public bool CheckSheepGrounded()
    {
        bool allGrounded = true;
        foreach (GameObject sheep in Herd)
        {
            SheepController sheepController = sheep.GetComponent<SheepController>();
            dependancyManager.InjectSheepControllerDependancyIntoSpawnManager(sheepController);
            allGrounded &= _sheepController.IsGrounded;
        }
        return allGrounded;
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
            _audioManager.HasDetectedWarnSingle = true;
            laneWarningsText[singleLaneIndex].SetActive(true);
            yield return new WaitForSeconds(2);
            laneWarningsText[singleLaneIndex].SetActive(false);
        }
        else if (noOfLanes == "All")
        {
            _audioManager.HasDetectedWarnAll = true;
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

    private GameObject GetPooledGameObject(int poolSize, List<GameObject> pool)
    {
        for (int i = 0; i < poolSize; i++)
        {
            if (!pool[i].activeInHierarchy)
            {
                return pool[i];
            }
        }
        return null;
    }


    public void ReturnPooledGameObject(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }

    private void SpawnBackground()
    {
        if (isGameActive)
        {
            GameObject foregroundTreeNew = GetPooledGameObject(treeAmountToPool, backgroundTreePool);
            if(foregroundTreeNew != null)
            {
                foregroundTreeNew.transform.SetPositionAndRotation(new Vector3(14, 0, 40), foregroundTreeNew.transform.rotation);
                foregroundTreeNew.SetActive(true);
                ObstacleController obstacleControllerForeground = foregroundTreeNew.GetComponent<ObstacleController>();
                dependancyManager.InjectObstacleControllerDependencies(obstacleControllerForeground);
            }

            GameObject backgroundTreeNew = GetPooledGameObject(treeAmountToPool, backgroundTreePool);
            if (backgroundTreeNew != null)
            {
                backgroundTreeNew.transform.SetPositionAndRotation(new Vector3(-9, 0, 40), backgroundTreeNew.transform.rotation);
                backgroundTreeNew.SetActive(true);
                ObstacleController obstacleControllerBackground = backgroundTreeNew.GetComponent<ObstacleController>();
                dependancyManager.InjectObstacleControllerDependencies(obstacleControllerBackground);
            }

            if (timeRemaining > 10)
            {
                Invoke("SpawnBackground", 1);
            }
        }
    }

    public bool CheckTimeSinceLostSheep(int targetSeconds)
    {
        TimeSinceLostSheep += 1;
        if (TimeSinceLostSheep >= targetSeconds)
        {
            TimeSinceLostSheep = 0;
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
                    StraySheepSpawnPosition = new Vector3(-12, 1, straySheepSpawnPositionZ);
                    StraySheepTargetPosition = new Vector3(11, 1, straySheepTargetPositionZ);
                    GameObject straySheepNew = Instantiate(straySheep, StraySheepSpawnPosition, straySheep.transform.rotation);
                    SheepController sheepController = straySheepNew.GetComponent<SheepController>();
                    dependancyManager.InjectSheepControllerDependencies(sheepController);
                }
                else if (spawnSide[sideIndex] == "right")
                {
                    StraySheepSpawnPosition = new Vector3(12, 1, straySheepSpawnPositionZ);
                    StraySheepTargetPosition = new Vector3(-11, 1, straySheepTargetPositionZ);
                    GameObject straySheepNew = Instantiate(straySheep, StraySheepSpawnPosition, straySheep.transform.rotation);
                    SheepController sheepController = straySheepNew.GetComponent<SheepController>();
                    dependancyManager.InjectSheepControllerDependencies(sheepController);
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

            GameObject obstacleNew = Instantiate(obstacle, obstacleSpawnPos, obstacle.transform.rotation);
            ObstacleController obstacleController = obstacleNew.GetComponent<ObstacleController>();
            dependancyManager.InjectObstacleControllerDependencies(obstacleController);

            InvokeEncounter(8);
        }
    }

    private void SpawnWolf()
    {
        if (isGameActive)
        {
            if (Herd.Length > 0)
            {
                // choose target for wolf to hunt
                ChooseTarget();

                // choose spawn position for wolf
                GameObject wolfNew = Instantiate(wolf, ChooseSpawnPosition(), wolf.transform.rotation);
                WolfController wolfController = wolfNew.GetComponent<WolfController>();
                dependancyManager.InjectWolfControllerDependencies(wolfController);

                // sheep reaction to wolf is reset
                foreach (GameObject sheep in Herd)
                {
                    SheepController sheepController = sheep.GetComponent<SheepController>();
                    dependancyManager.InjectSheepControllerDependancyIntoSpawnManager(sheepController);
                    _sheepController.HasAvoidedWolf = false;
                    _sheepController.HasEnteredWolfSpace = false;
                }
            }

            InvokeEncounter(8);
        }
    }

    private void ChooseTarget()
    {
        int targetIndex;
        string[] huntTarget = { "player", "sheep" };

        if (Herd.Length > 0)
        {
            targetIndex = Random.Range(0, huntTarget.Length);
        }
        else
        {
            targetIndex = 0;
        }

        if (huntTarget[targetIndex] == "player")
        {
            HasTargetedSheepdog = true;
            HasTargetedHerd = false;
        }
        else if (huntTarget[targetIndex] == "sheep")
        {
            HasTargetedHerd = true;
            HasTargetedSheepdog = false;
        }
    }

    private Vector3 ChooseSpawnPosition()
    {
        // spawn wolf to come from either right or left side of trail
        string[] wolfOrigin = { "left", "right" };
        int wolfOriginIndex = Random.Range(0, wolfOrigin.Length);

        // wolf spawn position changes based on hunt target
        float sheepDogPosZ = _sheepdog.PlayerTransform.position.z;

        Vector3 wolfSpawnPos = new();

        if (wolfOrigin[wolfOriginIndex] == "right")
        {
            if (HasTargetedSheepdog)
            {
                wolfSpawnPos = new Vector3(30.0f, 0, sheepDogPosZ + Random.Range(-15, -5));
            }
            else if (HasTargetedHerd)
            {
                wolfSpawnPos = new Vector3(11.0f, 0, Random.Range(-4, 1));
            }
        }
        if (wolfOrigin[wolfOriginIndex] == "left")
        {
            if (HasTargetedSheepdog)
            {
                wolfSpawnPos = new Vector3(-30.0f, 0, sheepDogPosZ + Random.Range(-15, -5));
            }
            else if (HasTargetedHerd)
            {
                wolfSpawnPos = new Vector3(-12.0f, 0, Random.Range(-4, 1));
            }
        }
        return wolfSpawnPos;
    }
}

public class MockSpawnManager : ISpawnManager
{
    public bool HasTargetedSheepdog { get; set; }
    public bool HasTargetedHerd { get; set; }
    public GameObject[] Herd { get; set; }
    public int TimeSinceLostSheep { get; set; }
    public Vector3 StraySheepSpawnPosition { get; set; }
    public Vector3 StraySheepTargetPosition { get; set; }

    public bool CheckSheepGrounded()
    {
        return Herd != null && Herd.Length > 0;
    }

    public void ReturnPooledGameObject(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }
}
