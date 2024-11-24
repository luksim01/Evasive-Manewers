using UnityEngine;

public abstract class BaseController : MonoBehaviour, IInteractiveCharacter
{
    public abstract void SetDependencies(IAudioManager audioManager, IUIManager uiManager, ISpawnManager spawnManager, IPlayerController playerController);
    public abstract void Interact();
}
