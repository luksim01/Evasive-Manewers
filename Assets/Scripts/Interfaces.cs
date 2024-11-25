using UnityEngine;

public interface IAudioManager
{
    void PlaySound(AudioClip clip);

    bool HasDetectedBarkMove { get; set; }
    bool HasDetectedBarkJump { get; set; }
    bool HasDetectedCollision { get; set; }
    bool HasDetectedLostSheep { get; set; }
    bool HasDetectedWarnSingle { get; set; }
    bool HasDetectedWarnAll { get; set; }
}

public interface IUIManager
{
    bool IsGameActive { get; set; }
    int TimeRemaining { get; set; }
    int Score { get; set; }
}

public interface ISpawnManager
{
    bool CheckSheepGrounded();

    bool HasTargetedSheepdog { get; set; }
    bool HasTargetedHerd { get; set; }

    GameObject[] Herd { get; set; }
    int TimeSinceLostSheep { get; set; }
    Vector3 StraySheepSpawnPosition { get; set; }
    Vector3 StraySheepTargetPosition { get; set; }
}

public interface IPlayerController
{
    Transform PlayerTransform { get; set; }
    int Health { get; set; }
    bool HasBarkedMove { get; set; }
    bool HasBarkedJump { get; set; }

    float BarkInteractionRadius { get; set; }
}

public interface IWolfController
{
    bool HasBitten { get; set; }
}

public interface ISheepController
{
    Transform SheepTransform { get; set; }
    bool IsGrounded { get; set; }
    bool HasEnteredWolfSpace { get; set; }
    bool HasAvoidedWolf { get; set; }
    bool IsSlowingDown { get; set; }
}

public interface IInteractiveCharacter
{
    bool Interact();
}
