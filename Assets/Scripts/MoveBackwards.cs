using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBackwards : MonoBehaviour
{
    private int speed = 5;
    private int bounds = 35;
    public bool hasHitPlayer = false;

    private bool isGameActive;

    void FixedUpdate()
    {
        isGameActive = GameObject.Find("UIManager").GetComponent<UIManager>().isGameActive;

        if (isGameActive)
        {
            transform.Translate(Vector3.back * Time.deltaTime * speed);

            if (gameObject.tag == "Obstacle" && transform.position.z < -bounds)
            {
                GameObject.Find("UIManager").GetComponent<UIManager>().score += 100;
                Destroy(gameObject);
            }
            if (gameObject.tag == "Background Tree" && transform.position.z < -bounds)
            {
                Destroy(gameObject);
            }
        }
    }

}
