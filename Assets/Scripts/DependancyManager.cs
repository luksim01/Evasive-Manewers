using UnityEngine;

public class DependancyManager : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private UIManager uiManager;

    [SerializeField] private PlayerController playerController;
    [SerializeField] private SpawnManager spawnManager;

    void Start()
    {
        // dependancies
        if (audioManager == null)
        {
            Debug.LogError("AudioManager is not assigned in GameManager.");
            return;
        }

        if (uiManager != null)
        {
            uiManager.SetDependencies(playerController, spawnManager);
        }
        else
        {
            Debug.LogError("UIManager is not assigned in GameManager.");
            return;
        }

        // inject dependancies
        if (playerController != null)
        {
            playerController.SetDependencies(audioManager, uiManager, spawnManager);
        }
        else
        {
            Debug.LogError("PlayerController is not assigned in GameManager.");
        }

        if (spawnManager != null)
        {
            spawnManager.SetDependencies(audioManager, uiManager, playerController, spawnManager);
        }
        else
        {
            Debug.LogError("SpawnManager is not assigned in GameManager.");
        }
    }

    public void InjectBackBoundaryControllerDependencies(BackBoundaryController backBoundaryController)
    {
        if(backBoundaryController != null)
        {
            backBoundaryController.SetDependencies(uiManager);
        }
    }

    public void InjectBackWolfControllerDependencies(BackWolfController backWolfController)
    {
        if (backWolfController != null)
        {
            backWolfController.SetDependencies(uiManager);
        }
    }

    public void InjectSheepControllerDependencies(SheepController sheepController)
    {
        if (sheepController != null)
        {
            sheepController.SetDependencies(audioManager, uiManager, spawnManager, playerController);
        }
    }

    public void InjectWolfControllerDependencies(WolfController wolfController)
    {
        if (wolfController != null)
        {
            wolfController.SetDependencies(audioManager, uiManager, spawnManager, playerController);
        }
    }

    public void InjectObstacleControllerDependencies(ObstacleController obstacleController)
    {
        if (obstacleController != null)
        {
            obstacleController.SetDependencies(uiManager, spawnManager);
        }
    }

    public void InjectWolfControllerDependancyIntoPlayerController(WolfController wolfController)
    {
        if (playerController != null)
        {
            playerController.SetWolfDependancy(wolfController);
        }
    }

    public void InjectSheepControllerDependancyIntoPlayerController(SheepController sheepController)
    {
        if (playerController != null)
        {
            playerController.SetSheepDependancy(sheepController);
        }
    }

    public void InjectSheepControllerDependancyIntoSpawnManager(SheepController sheepController)
    {
        if (playerController != null)
        {
            spawnManager.SetSheepDependancy(sheepController);
        }
    }
}
