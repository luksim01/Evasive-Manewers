using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBackwards : MonoBehaviour
{
    public int speed = 5;
    public int bounds = 35;

    public bool hasHitPlayer = false;

    private bool isGameActive;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        isGameActive = GameObject.Find("UIManager").GetComponent<UIManager>().isGameActive;

        if (isGameActive)
        {
            transform.Translate(Vector3.back * Time.deltaTime * speed);

            if (gameObject.tag == "Obstacle" && transform.position.z < -bounds)
            {
                Destroy(gameObject);
            }
        }
    }

}
