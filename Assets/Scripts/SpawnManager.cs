using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpawnManager : MonoBehaviour, ISpawnManager
{
    // wolf spawning
    public GameObject wolf;
    public List<GameObject> Pack { get; set; }
    public bool HasTargetedSheepdog { get; set; }
    public bool HasTargetedHerd { get; set; }
    public Vector3 WolfSpawnPosition { get; set; }
    private List<GameObject> wolfPool;
    private int wolfAmountToPool = 5;


    // obstacle spawning
    public GameObject[] obstacles;
    private float[] trailLanesPos;
    public GameObject[] laneWarningsText;
    private List<GameObject>[] obstaclePool;
    private int obstacleAmountToPool = 5;

    // sheep spawning
    public GameObject straySheep;
    public List<GameObject> Herd { get; set; }
    public List<GameObject> Hunted { get; set; }
    public List<GameObject> Strays { get; set; }
    public int TimeSinceLostSheep { get; set; }
    public Vector3 StraySheepSpawnPosition { get; set; }
    public Vector3 StraySheepTargetPosition { get; set; }
    public int spawnInterval = 20;
    private List<GameObject> sheepPool;
    private int sheepAmountToPool = 8;
    public GameObject warningPrompt;
    private List<GameObject> warningPromptPool;
    private int warningPromptAmountToPool = 10;

    // particle
    public GameObject sheepCollisionEffect;
    public List<GameObject> SheepCollisionEffectPool { get; set; }
    public int SheepCollisionEffectAmountToPool { get; set; }

    // background spawning
    public GameObject backgroundTree;
    private List<GameObject> backgroundTreePool;
    private int treeAmountToPool = 32;

    // herd leader indicator
    public GameObject leaderIndicator;
    public Material indicatorMaterial;
    private GameObject sheepLeaderIndicator;
    private Vector3 sheepLeaderIndicatorPosition;

    // lanes
    [SerializeField] private GameObject[] trailLanes;

    // audio
    private IAudioManager _audioManager;

    // ui
    private IUIManager _uiManager;

    // player
    private IPlayerController _sheepdog;

    // sheep
    private SheepController _sheepController;

    // spawn manager
    private SpawnManager _spawnManager;
    public float encounterInterval = 5f;
    private float encounterIntervalMin = 3f;
    private float encounterIntervalMax = 5f;

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

    void Start()
    {
        // spawn position array
        trailLanesPos = new float[trailLanes.Length];
        for (int laneIndex = 0; laneIndex < trailLanes.Length; laneIndex++)
        {
            trailLanesPos[laneIndex] = trailLanes[laneIndex].transform.position.x;
        }

        // creating a pool of warning prompts
        warningPromptPool = ObjectPoolUtility.Create("WarningPromptPool", gameObject.transform, warningPrompt, warningPromptAmountToPool);

        // creating a pool of boundary trees
        backgroundTreePool = ObjectPoolUtility.Create("BoundaryTreePool", gameObject.transform, backgroundTree, treeAmountToPool);

        // creating a pool of sheep
        Herd = new List<GameObject>();
        Hunted = new List<GameObject>();
        Strays = new List<GameObject>();
        sheepPool = ObjectPoolUtility.Create("SheepPool", gameObject.transform, straySheep, sheepAmountToPool);
        // spawning initial herd
        int[] xPosition = { -3, 0, 3 };
        int[] zPosition = {  4, 8, 4 };
        for (int i = 0; i < 3; i++)
        {
            GameObject sheepNew = ObjectPoolUtility.Get(sheepAmountToPool, sheepPool);
            if (sheepNew != null)
            {
                sheepNew.transform.SetPositionAndRotation(new Vector3(xPosition[i], 0, zPosition[i]), sheepNew.transform.rotation);
                sheepNew.tag = "Sheep";
                sheepNew.SetActive(true);
                AddSheepToHerd(sheepNew);
                SheepController sheepNewController = sheepNew.GetComponent<SheepController>();
                dependancyManager.InjectSheepControllerDependencies(sheepNewController);
            }
        }

        // creating a pool of sheep collision effects
        SheepCollisionEffectPool = ObjectPoolUtility.Create("SheepCollisionEffectPool", gameObject.transform, sheepCollisionEffect, SheepCollisionEffectAmountToPool = sheepAmountToPool);

        // create a pool of obstacles
        obstaclePool = new List<GameObject>[obstacles.Length];
        for (int i = 0; i < obstacles.Length; i++)
        {
            obstaclePool[i] = ObjectPoolUtility.Create("ObstaclePool", gameObject.transform, obstacles[i], obstacleAmountToPool);
        }

        // create a pool of wolves
        Pack = new List<GameObject>();
        wolfPool = ObjectPoolUtility.Create("WolfPool", gameObject.transform, wolf, wolfAmountToPool);

        // herd leader indicator
        sheepLeaderIndicator = InteractivityUtility.CreateLeaderIndicator(leaderIndicator, indicatorMaterial);

        //InteractivityUtility.UpdateLeaderIndicator(sheepLeaderIndicator, Vector3.forward * 15f);
        //sheepLeaderIndicator.SetActive(false);

        InvokeEncounter(8);
        Invoke("SpawnBackground", 0.7f);
        TimeSinceLostSheep = 0;
        Invoke("SpawnStraySheep", 3);
        Invoke("CheckHerdLeader", 0.1f);
    }

    public void AddSheepToHerd(GameObject gameObject)
    {
        Herd.Add(gameObject);
    }

    public void RemoveSheepFromHerd(GameObject gameObject)
    {
        Herd.Remove(gameObject);
    }

    public void AddSheepToHunted(GameObject gameObject)
    {
        Hunted.Add(gameObject);
    }

    public void RemoveSheepFromHunted(GameObject gameObject)
    {
        Hunted.Remove(gameObject);
    }

    public void AddSheepToStrays(GameObject gameObject)
    {
        Strays.Add(gameObject);
    }

    public void RemoveSheepFromStrays(GameObject gameObject)
    {
        Strays.Remove(gameObject);
    }

    public void AddWolfToPack(GameObject gameObject)
    {
        Pack.Add(gameObject);
    }

    public void RemoveWolfFromPack(GameObject gameObject)
    {
        Pack.Remove(gameObject);
    }

    public void IncreaseGameSpeed()
    {
        if (encounterInterval > encounterIntervalMin)
        {
            encounterInterval -= 0.1f;
        }
        else
        {
            encounterInterval = encounterIntervalMin;
        }
    }

    public void DecreaseGameSpeed()
    {
        if (encounterInterval < encounterIntervalMax)
        {
            encounterInterval += 0.2f;
        }
        else
        {
            encounterInterval = encounterIntervalMax;
        }
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

    private void CheckHerdLeader()
    {
        float leaderPositionZ = -40f;
        Vector3 leaderPosition = new Vector3(0, 0, leaderPositionZ);

        foreach (GameObject sheep in Herd)
        {
            SheepController sheepController = sheep.GetComponent<SheepController>();
            if (sheepController.SheepTransform.position.z > leaderPositionZ)
            {
                leaderPosition = sheepController.SheepTransform.position;
                leaderPositionZ = leaderPosition.z;
            }
        }

        InteractivityUtility.UpdateLeaderIndicator(sheepLeaderIndicator, leaderPosition);

        Invoke("CheckHerdLeader", 0.1f);
    }

    private void InvokeEncounter(float encounterInterval)
    {
        if(_uiManager != null)
        {
            if (_uiManager.TimeRemaining > 12)
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
            else if (_uiManager.TimeRemaining > 12)
            {
                Invoke("SpawnObstacle", encounterInterval);
            }
        }
        else
        {
            Invoke("SpawnObstacle", encounterInterval);
        }
    }

    IEnumerator DisplayLaneWarning(int singleLaneIndex, string noOfLanes)
    {
        if(noOfLanes == "Single")
        {
            _audioManager.HasDetectedWarnSingle = true;
            float warningPromptPositionX = trailLanesPos[singleLaneIndex];
            GameObject warningPromptNew = ObjectPoolUtility.Get(warningPromptAmountToPool, warningPromptPool);

            Vector3 warningPromptNewPosition = new Vector3(warningPromptPositionX, warningPromptNew.transform.position.y, warningPromptNew.transform.position.z);

            if (warningPromptNew != null)
            {
                warningPromptNew.transform.SetPositionAndRotation(warningPromptNewPosition, warningPromptNew.transform.rotation);
                warningPromptNew.SetActive(true);
            }
            yield return new WaitForSeconds(2);

            ObjectPoolUtility.Return(warningPromptNew);
        }
        else if (noOfLanes == "All")
        {
            _audioManager.HasDetectedWarnAll = true;

            GameObject[] warningPromptArray = new GameObject[warningPromptAmountToPool];

            for (int laneIndex = 0; laneIndex < laneWarningsText.Length; laneIndex++)
            {
                float warningPromptPositionX = trailLanesPos[laneIndex];
                GameObject warningPromptNew = ObjectPoolUtility.Get(warningPromptAmountToPool, warningPromptPool);

                Vector3 warningPromptNewPosition = new Vector3(warningPromptPositionX, warningPromptNew.transform.position.y, warningPromptNew.transform.position.z);

                if (warningPromptNew != null)
                {
                    warningPromptNew.transform.SetPositionAndRotation(warningPromptNewPosition, warningPromptNew.transform.rotation);
                    warningPromptNew.SetActive(true);
                }

                warningPromptArray[laneIndex] = warningPromptNew;
            }

            yield return new WaitForSeconds(2);

            for (int laneIndex = 0; laneIndex < laneWarningsText.Length; laneIndex++)
            {
                ObjectPoolUtility.Return(warningPromptArray[laneIndex]);
            }
        }
    }

    public void ActivateSheepCollisionEffect(GameObject effect)
    {
        StartCoroutine(CollisionEffectDuration(effect, 2f));
    }

    public IEnumerator CollisionEffectDuration(GameObject gameObject, float durationTime)
    {
        yield return new WaitForSeconds(durationTime);
        ObjectPoolUtility.Return(gameObject);
    }

    private void SpawnBackground()
    {
        if (_uiManager.IsGameActive)
        {
            float angle = 8f;
            GameObject foregroundTreeNew = ObjectPoolUtility.Get(treeAmountToPool, backgroundTreePool);
            if(foregroundTreeNew != null)
            {
                Quaternion rotation = Quaternion.Euler(Random.Range(-angle, angle), 0f, Random.Range(-angle, angle));
                foregroundTreeNew.transform.SetPositionAndRotation(new Vector3(14, 0, 40), rotation);
                foregroundTreeNew.SetActive(true);
                ObstacleController obstacleControllerForeground = foregroundTreeNew.GetComponent<ObstacleController>();
                dependancyManager.InjectObstacleControllerDependencies(obstacleControllerForeground);
            }

            GameObject backgroundTreeNew = ObjectPoolUtility.Get(treeAmountToPool, backgroundTreePool);
            if (backgroundTreeNew != null)
            {
                Quaternion rotation = Quaternion.Euler(Random.Range(-angle, angle), 0f, Random.Range(-angle, angle));
                backgroundTreeNew.transform.SetPositionAndRotation(new Vector3(-9, 0, 40), rotation);
                backgroundTreeNew.SetActive(true);
                ObstacleController obstacleControllerBackground = backgroundTreeNew.GetComponent<ObstacleController>();
                dependancyManager.InjectObstacleControllerDependencies(obstacleControllerBackground);
            }

            if (_uiManager.TimeRemaining > 10)
            {
                Invoke("SpawnBackground", 0.7f);
            }
        }
    }

    public bool CheckTimeSinceLastSheep(int targetSeconds)
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
        if (_uiManager.IsGameActive && _uiManager.TimeRemaining > 10)
        {
            if (CheckTimeSinceLastSheep(spawnInterval))
            {
                string[] spawnSide = { "left", "right" };
                int sideIndex = Random.Range(0, spawnSide.Length);


                int straySheepSpawnPositionZ = Random.Range(0, 5);
                int straySheepTargetPositionZ = straySheepSpawnPositionZ + 6;

                GameObject straySheepNew = ObjectPoolUtility.Get(sheepAmountToPool, sheepPool);
                AddSheepToStrays(straySheepNew);

                if (spawnSide[sideIndex] == "left")
                {
                    StraySheepSpawnPosition = new Vector3(-12, 0, straySheepSpawnPositionZ);
                    StraySheepTargetPosition = new Vector3(11, 0, straySheepTargetPositionZ);

                }
                else if (spawnSide[sideIndex] == "right")
                {
                    StraySheepSpawnPosition = new Vector3(12, 0, straySheepSpawnPositionZ);
                    StraySheepTargetPosition = new Vector3(-11, 0, straySheepTargetPositionZ);
                }

                if (straySheepNew != null)
                {
                    straySheepNew.transform.SetPositionAndRotation(StraySheepSpawnPosition, straySheepNew.transform.rotation);
                    straySheepNew.SetActive(true);
                    //UserTestManager.instance.SaveUserEventData(straySheepNew.name + " spawned");

                    SheepController sheepStrayController = straySheepNew.GetComponent<SheepController>();
                    dependancyManager.InjectSheepControllerDependencies(sheepStrayController);
                }
            }
            Invoke("SpawnStraySheep", 1);
        }
    }

    private void SpawnObstacle()
    {
        if (_uiManager.IsGameActive)
        {
            int obstacleIndex = Random.Range(0, obstacles.Length);
            GameObject obstacleNew = ObjectPoolUtility.Get(obstacleAmountToPool, obstaclePool[obstacleIndex]);

            int obstacleSpawnPosIndex;

            if (obstacleNew.name.Contains("Long"))
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

            Vector3 obstacleSpawnPos = new Vector3(trailLanesPos[obstacleSpawnPosIndex], obstacleNew.transform.position.y, 40.0f);

            if (obstacleNew != null)
            {
                obstacleNew.transform.SetPositionAndRotation(obstacleSpawnPos, obstacleNew.transform.rotation);
                obstacleNew.SetActive(true);
                //UserTestManager.instance.SaveUserEventData(obstacleNew.name + " spawned");
                ObstacleController obstacleController = obstacleNew.GetComponent<ObstacleController>();
                dependancyManager.InjectObstacleControllerDependencies(obstacleController);
            }

            InvokeEncounter(encounterInterval);
        }
    }

    private void SpawnWolf()
    {
        if (_uiManager.IsGameActive)
        {
            if (Herd.Count > 0)
            {
                // choose target for wolf to hunt
                ChooseTarget();

                // choose spawn position for wolf
                GameObject wolfNew = ObjectPoolUtility.Get(wolfAmountToPool, wolfPool);
                if (wolfNew != null)
                {
                    wolfNew.transform.SetPositionAndRotation(WolfSpawnPosition = ChooseSpawnPosition(), transform.rotation);

                    WolfController wolfController = wolfNew.GetComponent<WolfController>();
                    dependancyManager.InjectWolfControllerDependencies(wolfController);

                    _audioManager.HasDetectedSpawnWolf = true;

                    wolfNew.SetActive(true);
                    string targetCharacter = HasTargetedSheepdog ? "Player" : "Sheep";
                    //UserTestManager.instance.SaveUserEventData(wolfNew.name + " spawned, targeting " + targetCharacter);

                    AddWolfToPack(wolfNew);
                }
            }


            InvokeEncounter(encounterInterval);
        }
    }

    private void ChooseTarget()
    {
        int targetIndex;
        //string[] huntTarget = { "sheep" };
        string[] huntTarget = { "player", "sheep" };

        if (Herd.Count > 0)
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
                wolfSpawnPos = new Vector3(11.0f, 0, sheepDogPosZ + Random.Range(-8, -8));
            }
            else if (HasTargetedHerd)
            {
                wolfSpawnPos = new Vector3(11.0f, 0, Random.Range(-15, -5));
            }
        }
        if (wolfOrigin[wolfOriginIndex] == "left")
        {
            if (HasTargetedSheepdog)
            {
                wolfSpawnPos = new Vector3(-12.0f, 0, sheepDogPosZ + Random.Range(-8, -8));
            }
            else if (HasTargetedHerd)
            {
                wolfSpawnPos = new Vector3(-12.0f, 0, Random.Range(-15, -5));
            }
        }
        return wolfSpawnPos;
    }
}

public class MockSpawnManager : ISpawnManager
{
    public bool HasTargetedSheepdog { get; set; }
    public bool HasTargetedHerd { get; set; }
    public Vector3 WolfSpawnPosition { get; set; }
    public List<GameObject> Herd { get; set; }
    public List<GameObject> Hunted { get; set; }
    public List<GameObject> Pack { get; set; }
    public List<GameObject> Strays { get; set; }
    public int TimeSinceLostSheep { get; set; }
    public Vector3 StraySheepSpawnPosition { get; set; }
    public Vector3 StraySheepTargetPosition { get; set; }

    public bool CheckSheepGrounded()
    {
        return Herd != null && Herd.Count > 0;
    }

    public void ReturnPooledGameObject(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }

    public List<GameObject> SheepCollisionEffectPool { get; set; }
    public int SheepCollisionEffectAmountToPool { get; set; }
    public void ActivateSheepCollisionEffect(GameObject effect) { }

    public void AddSheepToHerd(GameObject gameObject) { }
    public void RemoveSheepFromHerd(GameObject gameObject) { }
    public void AddSheepToHunted(GameObject gameObject) { }
    public void RemoveSheepFromHunted(GameObject gameObject) { }
    public void AddSheepToStrays(GameObject gameObject) { }
    public void RemoveSheepFromStrays(GameObject gameObject) { }
    public void AddWolfToPack(GameObject gameObject) { }
    public void RemoveWolfFromPack(GameObject gameObject) { }
    public void IncreaseGameSpeed() { }
    public void DecreaseGameSpeed() { }
}
