using UnityEngine;

public class DependancyManager : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private UIManager uiManager;

    [SerializeField] private PlayerController playerController;
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private CameraController cameraController;

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
            uiManager.SetDependencies(playerController);
        }
        else
        {
            Debug.LogError("UIManager is not assigned in GameManager.");
            return;
        }

        // inject dependancies
        if (playerController != null)
        {
            playerController.SetDependencies(audioManager, uiManager);
        }
        else
        {
            Debug.LogError("PlayerController is not assigned in GameManager.");
        }

        if (spawnManager != null)
        {
            spawnManager.SetDependencies(audioManager, uiManager, playerController);
        }
        else
        {
            Debug.LogError("SpawnManager is not assigned in GameManager.");
        }

        if (cameraController != null)
        {
            cameraController.SetDependencies(uiManager);
        }
        else
        {
            Debug.LogError("CameraController is not assigned in GameManager.");
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
            wolfController.SetDependencies(uiManager, spawnManager, playerController);
        }
    }

    public void InjectMoveBackwardsDependencies(ObstacleController obstacleController)
    {
        if (obstacleController != null)
        {
            obstacleController.SetDependencies(uiManager);
        }
    }
}