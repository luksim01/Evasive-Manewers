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

    // dependancies
    public void SetDependencies(IUIManager uiManager)
    {
        _uiManager = uiManager;
    }

    void FixedUpdate()
    {
        isGameActive = _uiManager.IsGameActive;

        if (isGameActive)
        {
            transform.Translate(Vector3.back * Time.deltaTime * speed);

            if (gameObject.tag == "Obstacle" && transform.position.z < -bounds)
            {
                _uiManager.Score += 100;
                Destroy(gameObject);
            }
            if (gameObject.tag == "Background Tree" && transform.position.z < -bounds)
            {
                Destroy(gameObject);
            }
        }
    }

}
