using UnityEngine;

public abstract class BaseController : MonoBehaviour
{
    public abstract void SetDependencies(IAudioManager audioManager, IUIManager uiManager, ISpawnManager spawnManager, IPlayerController playerController);
}
