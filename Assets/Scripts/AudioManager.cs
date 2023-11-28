using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    // sound effects
    private AudioSource audioManager;
    public AudioClip collisionSound;
    public AudioClip sheepLostSound;
    public AudioClip laneWarnSingleSound;
    public AudioClip laneWarnAllSound;

    public bool hasDetectedCollision;
    public bool hasDetectedLostSheep;
    public bool hasDetectedWarnSingle;
    public bool hasDetectedWarnAll;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hasDetectedCollision)
        {
            audioManager.PlayOneShot(collisionSound, 1.0f);
            hasDetectedCollision = false;
        }

        if (hasDetectedLostSheep)
        {
            audioManager.PlayOneShot(sheepLostSound, 1.0f);
            hasDetectedLostSheep = false;
        }

        if (hasDetectedWarnSingle)
        {
            audioManager.PlayOneShot(laneWarnSingleSound, 1.0f);
            hasDetectedWarnSingle = false;
        }

        if (hasDetectedWarnAll)
        {
            audioManager.PlayOneShot(laneWarnAllSound, 1.0f);
            hasDetectedWarnAll = false;
        }
    }
}
