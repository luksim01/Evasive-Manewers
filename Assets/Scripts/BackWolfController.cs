using UnityEngine;

public class BackWolfController : MonoBehaviour
{
    float directionZ;
    float startOffsetZ;
    float movementLimitZ = 0.5f;
    float movementMidpointZ;
    float speed = 1f;

    private bool isInitialised = false;
    private bool isStablePosition = false;
    private bool isDodging = false;
    private bool isRising = false;
    private bool isFalling = false;
    private float heightLimit = 1f;

    private Ray ray;

    // ui
    private IUIManager _uiManager;

    // dependancies
    public void SetDependencies(IUIManager uiManager)
    {
        _uiManager = uiManager;
    }

    // dependancy manager
    [SerializeField] private DependancyManager dependancyManager;

    void Start()
    {
        dependancyManager.InjectBackWolfControllerDependencies(this);
    }

    void FixedUpdate()
    {
        if (_uiManager.IsGameActive)
        {
            if (!isInitialised)
            {
                isInitialised = true;

                startOffsetZ = Random.Range(-1f, 1f) * 2f;
                this.transform.position = this.transform.position + new Vector3(0, 0, startOffsetZ);

                switch (Random.Range(0, 2))
                {
                    case 0:
                        directionZ = -1;
                        break;
                    case 1:
                        directionZ = 1;
                        break;
                    default:
                        break;
                }
            }
            else 
            {
                if (_uiManager.TimeRemaining > 10)
                {
                    if (this.transform.parent.position.z > 0 && !isStablePosition)
                    {
                        isStablePosition = true;
                        movementMidpointZ = this.transform.position.z;
                    }

                    if (isStablePosition)
                    {
                        this.transform.Translate(Vector3.forward * directionZ * Time.fixedDeltaTime * speed);

                        if (this.transform.position.z <= movementMidpointZ - movementLimitZ || this.transform.position.z >= movementMidpointZ + movementLimitZ)
                        {
                            directionZ = -directionZ;
                        }
                    }
                }

                if (!isDodging)
                {
                    AvoidObstacles();
                }

                if (isDodging)
                {
                    Dodge();
                }
            }


        }
    }

    void Dodge()
    {
        if (isRising)
        {
            if (this.transform.position.y < heightLimit)
            {
                this.transform.Translate(Vector3.up * Time.fixedDeltaTime * speed);
            }

            if (this.transform.position.y > heightLimit)
            {
                this.transform.position = new Vector3(this.transform.position.x, heightLimit, this.transform.position.z);
                isRising = false;
                isFalling = true;
            }
        }

        if (isFalling)
        {
            if (this.transform.position.y > 0)
            {
                this.transform.Translate(Vector3.down * Time.fixedDeltaTime * speed);
            }

            if (this.transform.position.y < 0)
            {
                this.transform.position = new Vector3(this.transform.position.x, 0, this.transform.position.z);
                isFalling = false;
                isDodging = false;
            }
        }
    }

    void AvoidObstacles()
    {
        Vector3 raySource = new Vector3(this.transform.position.x, 1f, this.transform.position.z + 1f);
        Vector3 rayTarget = new Vector3(0, -0.2f, 1f);

        // check for obstacles in direction of movement
        ray = new Ray(raySource, rayTarget);
        float rangeMultiplier = 5f;

        if (Physics.Raycast(ray, rangeMultiplier, InteractivityUtility.obstacleMask))
        {
            // if obstacle ahead, dodge
            isDodging = true;
            isRising = true;
        }

        Debug.DrawRay(raySource, rayTarget * rangeMultiplier, Color.red, 1f);
    }
}