using UnityEngine;

public abstract class BaseCharacterController : MonoBehaviour, IInteractiveCharacter
{
    public abstract void SetDependencies(IAudioManager audioManager, IUIManager uiManager, ISpawnManager spawnManager, IPlayerController playerController);
    public abstract void Interact();
    public abstract void AddOutline();
    public abstract void RemoveOutline();
}
