using UnityEngine;
using System.Collections.Generic;

public class WolfController : BaseCharacterController, ICollidable
{
    // wolf
    private Transform WolfTransform { get; set; }
    private bool hasBitten;
    private Rigidbody wolfRb;

    // hunt target
    private GameObject targetSheep;

    // track player 
    private bool isBarkedAt;
    private Vector3 collisionCourse;
    private bool isCharging = false;

    // ui
    private IUIManager _uiManager;

    // spawn manager
    private ISpawnManager _spawnManager;
    private float wolfSpawnPositionX;
    private bool hasTargetedSheepdog;
    private bool hasTargetedHerd;
    private bool hasInitialisedWolf = false;
    private bool hasTargetedSheep = false;

    // player
    private IPlayerController _sheepdog;

    public override void SetDependencies(IAudioManager audioManager, IUIManager uiManager, ISpawnManager spawnManager, IPlayerController playerController)
    {
        _uiManager = uiManager;
        _spawnManager = spawnManager;
        _sheepdog = playerController;
    }

    // animation
    private Animator wolfHeadAnim;

    // collision
    public bool HasCollided { get; set; }

    // interactivity
    [SerializeField] private float interactionRange;
    private float previousInteractionRange;
    private GameObject wolfInteractivityIndicator;
    public GameObject interactivityIndicator;
    public Material indicatorMaterial;
    [SerializeField] private Vector3 indicatorPositionOffset;
    private Ray ray;
    private List<Collider> trackedCollidedList;
    private List<Collider> removeCollidedList;

    // player proximity
    //[SerializeField] private Vector3 travelDirection;
    [SerializeField] private Vector3 targetDirection;
    bool hasEngaged;

    void Start()
    {
        wolfRb = GetComponent<Rigidbody>();

        // interactivity
        wolfInteractivityIndicator = InteractivityUtility.CreateInteractivityIndicator(WolfTransform, interactivityIndicator, indicatorMaterial, indicatorPositionOffset, interactionRange);
        trackedCollidedList = new List<Collider>();
        removeCollidedList = new List<Collider>();
    }

    void OnEnable()
    {
        InitialiseWolf();
    }

    void FixedUpdate()
    {
        if (_uiManager.IsGameActive && hasInitialisedWolf)
        {
            if (interactionRange != previousInteractionRange)
            {
                InteractivityUtility.UpdateInteractivityIndicator(wolfInteractivityIndicator, interactionRange);
            }
            previousInteractionRange = interactionRange;

            trackedCollidedList = InteractivityUtility.CastRadius(WolfTransform, wolfInteractivityIndicator.transform.position, trackedCollidedList, removeCollidedList, interactionRange);


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

    void InitialiseWolf()
    {
        if(_spawnManager != null)
        {
            wolfSpawnPositionX = _spawnManager.WolfSpawnPosition.x;
            hasTargetedSheepdog = _spawnManager.HasTargetedSheepdog;
            hasTargetedHerd = _spawnManager.HasTargetedHerd;
        }

        hasEngaged = false;

        WolfTransform = this.transform;
        wolfHeadAnim = WolfTransform.Find("wolf_head").GetComponent<Animator>();
        hasInitialisedWolf = true;
    }

    void ReturnToPoolAndReset(GameObject gameObject)
    {
        _spawnManager.RemoveWolfFromPack(gameObject);
        hasInitialisedWolf = false;
        wolfSpawnPositionX = 0f;
        hasTargetedSheepdog = false;
        hasTargetedHerd = false;
        hasTargetedSheep = false;
        isCharging = false;
        hasBitten = false;
        isBarkedAt = false;
        ObjectPoolUtility.Return(gameObject);
    }

    public override void Interact()
    {
        isBarkedAt = true;
    }

    private void DestroyBoundaries(float xBoundaryRight, float xBoundaryLeft, float zBoundaryForward, float zBoundaryBackward)
    {
        // destroy if out of bounds
        if (WolfTransform.position.x > xBoundaryRight || WolfTransform.position.x < xBoundaryLeft)
        {
            if (!hasBitten && hasTargetedSheepdog)
            {
                _uiManager.Score += 100;
            }
            ReturnToPoolAndReset(gameObject);
        }
        if (WolfTransform.position.z > zBoundaryForward || WolfTransform.position.z < zBoundaryBackward)
        {
            if (!hasBitten && hasTargetedSheepdog)
            {
                _uiManager.Score += 100;
            }
            ReturnToPoolAndReset(gameObject);
        }
    }

    int IdentifyClosestSheepIndex()
    {
        int targetIndex = -1;
        float closestSheep = 80;

        for (int i = 0; i < _spawnManager.Herd.Count; i++)
        {
            if (Mathf.Abs(_spawnManager.Herd[i].transform.position.x - WolfTransform.position.x) < closestSheep)
            {
                closestSheep = Mathf.Abs(_spawnManager.Herd[i].transform.position.x - WolfTransform.position.x);
                targetIndex = i;
            }
        }
        return targetIndex;
    }

    private void HuntSheep()
    {
        // target one sheep
        if (!hasTargetedSheep)
        {
            targetSheep = _spawnManager.Herd[IdentifyClosestSheepIndex()];
            hasTargetedSheep = true;
            targetSheep.tag = "Hunted";
        }

        if (targetSheep)
        {
            // move towards target sheep
            targetDirection = InteractivityUtility.GetTowardDirection(wolfRb.position, targetSheep.transform.position);

            if (trackedCollidedList.Contains(targetSheep.GetComponent<Collider>()))
            {
                hasEngaged = true;
            }

            if (!hasEngaged)
            {
                
                MovementUtility.MoveSmooth(wolfRb, targetDirection, 10f, 0.9f);
            }
            else
            {
                MovementUtility.MoveSmooth(wolfRb, targetDirection, 5f, 0.9f);
            }
            

            // wolf hunt sequence can be interrupted
            if (isBarkedAt && WolfTransform.position.x < 6.2 && WolfTransform.position.x > -6.2)
            {
                wolfHeadAnim.SetTrigger("isBiting");
                targetSheep.tag = "Sheep";
                isCharging = true;
            }
        }


        float xBoundaryLeft = 12.0f;
        float xBoundaryRight = -13.0f;
        float zBoundary = 40.0f;

        if (isCharging)
        {
            targetDirection = new Vector3(0f, 0f, zBoundary);
            MovementUtility.MoveSmooth(wolfRb, targetDirection, 20f, 1f);
        }
        else if (!targetSheep.activeSelf)
        {
            isCharging = true;
        }

        DestroyBoundaries(xBoundaryLeft, xBoundaryRight, zBoundary, -zBoundary);
    }

    private void HuntPlayer()
    {
        // track player position
        if (!isCharging)
        {
            targetDirection = InteractivityUtility.GetTowardDirection(wolfRb.position, _sheepdog.PlayerRigidbody.position);
            MovementUtility.MoveSmooth(wolfRb, targetDirection, 10f, 0.9f);
            MovementUtility.LookAt(wolfRb, targetDirection);
        }

        // once close enough, charge at player in one direction
        if (trackedCollidedList.Contains(_sheepdog.PlayerCollider) & !isCharging)
        {
            isCharging = true;
        }

        if (isCharging)
        {
            wolfHeadAnim.SetTrigger("isBiting");
            MovementUtility.MoveSmooth(wolfRb, targetDirection, 10f, 0.9f);
            MovementUtility.LookAt(wolfRb, targetDirection);
        }

        float xBoundaryLeft = 12.0f;
        float xBoundaryRight = -13.0f;
        float zBoundary = 40.0f;

        DestroyBoundaries(xBoundaryLeft, xBoundaryRight, zBoundary, -zBoundary);
    }

    public void OnCollision(GameObject collidingObject)
    {
        if (collidingObject.CompareTag("Player"))
        {
            HasCollided = true;
            hasBitten = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ICollidable collidable = other.gameObject.GetComponent<ICollidable>();
        if (collidable != null && !collidable.HasCollided)
        {
            collidable.OnCollision(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        ICollidable collidable = collision.gameObject.GetComponent<ICollidable>();
        if (collidable != null && !collidable.HasCollided)
        {
            collidable.OnCollision(this.gameObject);
        }
    }
}