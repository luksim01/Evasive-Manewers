using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    private int speed = 5;
    private int bounds = 35;
    public bool hasHitPlayer = false;

    // ui
    private bool isGameActive;
    private IUIManager _uiManager;

    // spawn manager
    private ISpawnManager _spawnManager;

    // dependancies
    public void SetDependencies(IUIManager uiManager, SpawnManager spawnManager)
    {
        _uiManager = uiManager;
        _spawnManager = spawnManager;
    }

    void FixedUpdate()
    {
        isGameActive = _uiManager.IsGameActive;

        if (isGameActive)
        {
            transform.Translate(Vector3.back * Time.deltaTime * speed);

            if (gameObject.CompareTag("Obstacle") && transform.position.z < -bounds)
            {
                _uiManager.Score += 100;
                Destroy(gameObject);
            }
            if (gameObject.CompareTag("Background Tree") && transform.position.z < -bounds)
            {
                //Destroy(gameObject);
                _spawnManager.ReturnPooledGameObject(gameObject);
            }
        }
    }

}
