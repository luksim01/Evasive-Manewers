using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBackwards : MonoBehaviour
{
    public int speed = 8;
    public int bounds = 35;
    private Rigidbody obstacleRb;

    // Start is called before the first frame update
    void Start()
    {
        obstacleRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //obstacleRb.AddForce(Vector3.back * Time.deltaTime * speed);
        transform.Translate(Vector3.back * Time.deltaTime * speed);

        if (transform.position.z < -bounds)
        {
            Destroy(gameObject);
        }
    }
}
