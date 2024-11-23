using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfController : MonoBehaviour, IWolfController
{
    // wolf 
    private float wolfStartPosX;
    public bool HasBitten { get; set; }

    // hunt target
    private GameObject targetSheep;

    // track player 
    private bool isBarkedAt;
    private Vector3 collisionCourse;
    private bool isCharging = false;

    // ui
    private bool isGameActive;
    private IUIManager _uiManager;

    // spawn manager
    private ISpawnManager _spawnManager;
    private bool hasTargetedSheepdog;
    private bool hasTargetedHerd;
    private bool hasTargetedSheep = false;

    // player
    private IPlayerController _sheepdog;

    public void SetDependencies(IUIManager uiManager, ISpawnManager spawnManager, IPlayerController playerController)
    {
        _uiManager = uiManager;
        _spawnManager = spawnManager;
        _sheepdog = playerController;
    }

    // animation
    private Animator wolfHeadAnim;

    void Start()
    {
        wolfStartPosX = transform.position.x;

        hasTargetedSheepdog = _spawnManager.HasTargetedSheepdog;
        hasTargetedHerd = _spawnManager.HasTargetedHerd;

        wolfHeadAnim = this.transform.Find("wolf_head").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        isGameActive = _uiManager.IsGameActive;

        if (isGameActive)
        {
            if (hasTargetedSheepdog)
            {
                HuntPlayer();
            }
            else if (hasTargetedHerd)
            {
                HuntSheep();
            }
        }
    }

    private void DestroyBoundaries(float xBoundaryRight, float xBoundaryLeft, float zBoundaryForward, float zBoundaryBackward)
    {
        // destroy if out of bounds
        if (transform.position.x > xBoundaryRight || transform.position.x < xBoundaryLeft)
        {
            if (!HasBitten && hasTargetedSheepdog)
            {
                _uiManager.Score += 100;
            }
            Destroy(gameObject);
        }
        if (transform.position.z > zBoundaryForward || transform.position.z < zBoundaryBackward)
        {
            if (!HasBitten && hasTargetedSheepdog)
            {
                _uiManager.Score += 100;
            }
            Destroy(gameObject);
        }
    }

    int IdentifyClosestSheepIndex()
    {
        int targetIndex = -1;
        float closestSheep = 80;

        for (int i = 0; i < _spawnManager.Herd.Length; i++)
        {
            if (Mathf.Abs(_spawnManager.Herd[i].transform.position.x - transform.position.x) < closestSheep)
            {
                closestSheep = Mathf.Abs(_spawnManager.Herd[i].transform.position.x - transform.position.x);
                targetIndex = i;
            }
        }
        return targetIndex;
    }

    private void HuntSheep()
    {
        // keep track of bark
        isBarkedAt = _sheepdog.HasBarkedMove;

        // target one sheep
        if (!hasTargetedSheep)
        {
            targetSheep = _spawnManager.Herd[IdentifyClosestSheepIndex()];
            hasTargetedSheep = true;
            targetSheep.tag = "Hunted";
        }

        if (targetSheep)
        {
            // sheep proximity
            float sheepProximityX = targetSheep.transform.position.x - transform.position.x;
            float sheepProximityZ = targetSheep.transform.position.z - transform.position.z;
            Vector3 sheepProximity = new Vector3(sheepProximityX, 0, sheepProximityZ);

            if (Mathf.Abs(sheepProximityX) > 2 || Mathf.Abs(sheepProximityZ) > 4)
            {
                TravelTowards(sheepProximity, 8.0f);
            }

            // player proximity
            float sheepDogProximityX = _sheepdog.PlayerTransform.position.x - transform.position.x;
            float sheepDogProximityZ = _sheepdog.PlayerTransform.position.z - transform.position.z;

            // wolf hunt sequence can be interrupted
            if (isBarkedAt &&
                Mathf.Abs(sheepDogProximityX) < 3 &&
                Mathf.Abs(sheepDogProximityZ) < 5 &&
                transform.position.x < 5.4 &&
                transform.position.x > -5.4)
            {
                wolfHeadAnim.SetTrigger("isBiting");
                targetSheep.tag = "Sheep";
            }

            if (targetSheep.CompareTag("Sheep"))
            {
                TravelTowards(new Vector3(transform.position.x, 0, 50 - transform.position.z), 18.0f);
            }
        }
        else
        {
            TravelTowards(new Vector3(transform.position.x, 0, 50 - transform.position.z), 18.0f);
        }

        float xBoundaryLeft = 12.0f;
        float xBoundaryRight = -13.0f;
        float zBoundary = 40.0f;

        DestroyBoundaries(xBoundaryLeft, xBoundaryRight, zBoundary, -zBoundary);
}

    private void HuntPlayer()
    {
        float sheepDogProximityX = _sheepdog.PlayerTransform.transform.position.x - transform.position.x;
        float sheepDogProximityZ = _sheepdog.PlayerTransform.position.z - transform.position.z;
        Vector3 sheepDogProximity = new Vector3(sheepDogProximityX, 0, sheepDogProximityZ);

        // track player position
        if (wolfStartPosX > 0 && !isCharging)
        {
            collisionCourse = sheepDogProximity;
            Track(sheepDogProximity);
        }

        if (wolfStartPosX < 0 && !isCharging)
        {
            collisionCourse = sheepDogProximity;
            Track(sheepDogProximity);
        }

        // once close enough, charge at player in one direction
        if (sheepDogProximity.z <= 8 && transform.position.x < 7.7f && transform.position.x > -7.7f)
        {
            isCharging = true;
        }
        if (isCharging)
        {
            if (Mathf.Abs(sheepDogProximityZ) < 6)
            {
                wolfHeadAnim.SetTrigger("isBiting");
            }
            Track(collisionCourse);
        }

        float xBoundaryLeft = 40.0f;
        float xBoundaryRight = -40.0f;
        float zBoundary = 40.0f;

        DestroyBoundaries(xBoundaryLeft, xBoundaryRight, zBoundary, -zBoundary);
    }

    private void Track(Vector3 direction)
    {
        Vector3 alignDirection = (direction).normalized;
        if (alignDirection != Vector3.zero && !HasBitten)
        {
            transform.rotation = Quaternion.LookRotation(alignDirection);
        }
        transform.Translate(alignDirection * 10.0f * Time.deltaTime);
    }


    private void TravelTowards(Vector3 direction, float speed)
    {
        Vector3 alignDirection = (direction).normalized;
        transform.Translate(alignDirection * speed * Time.deltaTime);
    }
}
