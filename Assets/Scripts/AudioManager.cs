using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour, IAudioManager
{
    // sound effects
    private AudioSource audioManager;

    [SerializeField] private AudioClip barkMoveSound;
    [SerializeField] private AudioClip barkJumpSound;
    [SerializeField] private AudioClip collisionSound;
    [SerializeField] private AudioClip sheepLostSound;
    [SerializeField] private AudioClip laneWarnSingleSound;
    [SerializeField] private AudioClip laneWarnAllSound;
    [SerializeField] private AudioClip spawnWolfSound;

    private bool hasDetectedBarkMove;
    private bool hasDetectedBarkJump;
    private bool hasDetectedCollision;
    private bool hasDetectedLostSheep;
    private bool hasDetectedWarnSingle;
    private bool hasDetectedWarnAll;
    private bool hasDetectedSpawnWolf;

    // interface properties
    public bool HasDetectedBarkMove
    {
        get { return hasDetectedBarkMove; }
        set { hasDetectedBarkMove = value; }
    }

    public bool HasDetectedBarkJump
    {
        get { return hasDetectedBarkJump; }
        set { hasDetectedBarkJump = value; }
    }

    public bool HasDetectedCollision
    {
        get { return hasDetectedCollision; }
        set { hasDetectedCollision = value; }
    }

    public bool HasDetectedLostSheep
    {
        get { return hasDetectedLostSheep; }
        set { hasDetectedLostSheep = value; }
    }

    public bool HasDetectedWarnSingle
    {
        get { return hasDetectedWarnSingle; }
        set { hasDetectedWarnSingle = value; }
    }

    public bool HasDetectedWarnAll
    {
        get { return hasDetectedWarnAll; }
        set { hasDetectedWarnAll = value; }
    }

    public bool HasDetectedSpawnWolf
    {
        get { return hasDetectedSpawnWolf; }
        set { hasDetectedSpawnWolf = value; }
    }

    void Start()
    {
        audioManager = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (hasDetectedBarkMove)
        {
            PlaySound(barkMoveSound);
            hasDetectedBarkMove = false;
        }

        if (hasDetectedBarkJump)
        {
            PlaySound(barkJumpSound);
            hasDetectedBarkJump = false;
        }

        if (hasDetectedCollision)
        {
            PlaySound(collisionSound);
            hasDetectedCollision = false;
        }

        if (hasDetectedLostSheep)
        {
            PlaySound(sheepLostSound);
            hasDetectedLostSheep = false;
        }

        if (hasDetectedWarnSingle)
        {
            PlaySound(laneWarnSingleSound);
            hasDetectedWarnSingle = false;
        }

        if (hasDetectedWarnAll)
        {
            PlaySound(laneWarnAllSound);
            hasDetectedWarnAll = false;
        }

        if (HasDetectedSpawnWolf)
        {
            PlaySound(spawnWolfSound);
            hasDetectedSpawnWolf = false;
        }
    }

    public void PlaySound(AudioClip clip)
    {
        audioManager.PlayOneShot(clip, 1.0f);
    }
}

public class MockAudioManager : IAudioManager
{
    public bool HasDetectedBarkMove { get; set; }
    public bool HasDetectedBarkJump { get; set; }
    public bool HasDetectedCollision { get; set; }
    public bool HasDetectedLostSheep { get; set; }
    public bool HasDetectedWarnSingle { get; set; }
    public bool HasDetectedWarnAll { get; set; }
    public bool HasDetectedSpawnWolf { get; set; }

    public void PlaySound(AudioClip clip)
    {
        // Mock sound-playing logic. This can simply log the action for testing purposes.
        Debug.Log($"MockAudioManager: Playing sound {clip?.name}");
    }
}
