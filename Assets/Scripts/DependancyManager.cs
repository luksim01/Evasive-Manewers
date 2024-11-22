using UnityEngine;

public class DependancyManager : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;

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

        // inject dependancies
        if (playerController != null)
        {
            playerController.SetDependencies(audioManager);
        }
        else
        {
            Debug.LogError("PlayerController is not assigned in GameManager.");
        }

        if (spawnManager != null)
        {
            spawnManager.SetDependencies(audioManager);
        }
        else
        {
            Debug.LogError("SpawnManager is not assigned in GameManager.");
        }
    }

    public void InjectDependencies(SheepController sheepController)
    {
        if (sheepController != null)
        {
            sheepController.SetDependencies(audioManager);
        }
    }
}
