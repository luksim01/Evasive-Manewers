using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    private int speed = 5;
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

    void FixedUpdate()
    {
        if (_uiManager.IsGameActive && hasInitialisedObstacle)
        {
            ObstacleTransform.Translate(Vector3.back * Time.deltaTime * speed);

            if (gameObject.CompareTag("Obstacle") && ObstacleTransform.position.z < -bounds)
            {
                _uiManager.Score += 100;
                ObjectPoolUtility.Return(gameObject);
            }
            if (gameObject.CompareTag("Background Tree") && ObstacleTransform.position.z < -bounds)
            {
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
}
