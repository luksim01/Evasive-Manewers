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
    bool HasTargetedSheepdog { get; set; }
    bool HasTargetedHerd { get; set; }

    public int TimeSinceLostSheep { get; set; }
    public Vector3 StraySheepSpawnPosition { get; set; }
    public Vector3 StraySheepTargetPosition { get; set; }
}

public interface IPlayerController
{
    public Transform PlayerTransform { get; set; }
    public int Health { get; set; }
    public bool HasBarkedMove { get; set; }
    public bool HasBarkedJump { get; set; }
}
