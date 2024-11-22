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

public interface IAnimationManager
{
    bool PlayDogBarkMoveCommandAnimation { get; set; }
    bool PlayDogJumpAnimation { get; set; }
}
