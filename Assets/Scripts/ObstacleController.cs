using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    private int speed = 7;
    private int bounds = 35;
    public bool hasHitPlayer = false;
    private Transform ObstacleTransform;
    private bool hasInitialisedObstacle = false;

    // ui
    private IUIManager _uiManager;

    // spawn manager
    private ISpawnManager _spawnManager;

    // dependancies
    public void SetDependencies(IUIManager uiManager, SpawnManager spawnManager)
    {
        _uiManager = uiManager;
        _spawnManager = spawnManager;
    }

    private void OnEnable()
    {
        InitialiseObstacle();
    }

    void Update()
    {
        if (_uiManager.IsGameActive && hasInitialisedObstacle)
        {
            ObstacleTransform.Translate(Vector3.back * Time.deltaTime * speed, Space.World);

            if (ObstacleTransform.position.z < -bounds)
            {
                if (gameObject.CompareTag("Obstacle"))
                {
                    ObstacleTransform.rotation = Quaternion.Euler(0, 0, 0);
                    _uiManager.Score += 100;
                    _spawnManager.IncreaseGameSpeed();
                }
                ObjectPoolUtility.Return(gameObject);
            }
        }
    }

    void InitialiseObstacle()
    {
        ObstacleTransform = this.transform;
        hasInitialisedObstacle = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        ICollidable collidable = collision.gameObject.GetComponent<ICollidable>();
        if (collidable != null && !collidable.HasCollided)
        {
            collidable.OnCollision(this.gameObject);
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
}
