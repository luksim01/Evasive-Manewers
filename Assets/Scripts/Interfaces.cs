using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public interface IAudioManager
{
    void PlaySound(AudioClip clip);

    bool HasDetectedBarkMove { get; set; }
    bool HasDetectedBarkJump { get; set; }
    bool HasDetectedCollision { get; set; }
    bool HasDetectedLostSheep { get; set; }
    bool HasDetectedWarnSingle { get; set; }
    bool HasDetectedWarnAll { get; set; }
    bool HasDetectedSpawnWolf { get; set; }
}

public interface IUIManager
{
    bool IsGameActive { get; set; }
    int TimeRemaining { get; set; }
    int Score { get; set; }
    void PauseResume();
    void Exit();
}

public interface ISpawnManager
{
    bool CheckSheepGrounded();

    bool HasTargetedSheepdog { get; set; }
    bool HasTargetedHerd { get; set; }
    Vector3 WolfSpawnPosition { get; set; }

    List<GameObject> Herd { get; set; }
    List<GameObject> Hunted { get; set; }
    List<GameObject> Pack { get; set; }
    List<GameObject> Strays { get; set; }

    int TimeSinceLostSheep { get; set; }
    Vector3 StraySheepSpawnPosition { get; set; }
    Vector3 StraySheepTargetPosition { get; set; }

    List<GameObject> SheepCollisionEffectPool { get; set; }
    int SheepCollisionEffectAmountToPool { get; set; }
    void ActivateSheepCollisionEffect(GameObject effect);

    void AddSheepToHerd(GameObject gameObject);
    void RemoveSheepFromHerd(GameObject gameObject);
    void AddSheepToHunted(GameObject gameObject);
    void RemoveSheepFromHunted(GameObject gameObject);
    void AddSheepToStrays(GameObject gameObject);
    void RemoveSheepFromStrays(GameObject gameObject);
    void AddWolfToPack(GameObject gameObject);
    void RemoveWolfFromPack(GameObject gameObject);
}

public interface IPlayerController
{
    Transform PlayerTransform { get; set; }
    Rigidbody PlayerRigidbody { get; set; }
    Collider PlayerCollider { get; set; }
    InputAction Move { get; set; }
    InputAction Jump { get; set; }
    InputAction BarkMove { get; set; }
    InputAction BarkJump { get; set; }
    int Health { get; set; }
    bool HasBarkedMove { get; set; }
    bool HasBarkedJump { get; set; }
}

public interface ISheepController
{
    Transform SheepTransform { get; set; }
    bool IsGrounded { get; set; }
}

public interface ICollidable
{
    bool HasCollided { get; set; }
    bool IsGrounded { get; set; }
    void OnCollision(GameObject collidingObject);
}

public interface IObjectPool
{
    List<GameObject> CreateGameObjectPool(string poolName, Transform poolParent, GameObject poolObject, int poolSize);
    GameObject GetPooledGameObject(int poolSize, List<GameObject> pool);
    void ReturnPooledGameObject(GameObject gameObject);
}

public interface IInteractiveCharacter
{
    void Interact();
}