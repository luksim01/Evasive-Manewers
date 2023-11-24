using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfController : MonoBehaviour
{
    public float speed = 3000.0f;
    private GameObject sheepDog;
    Vector3 alignDirection;
    public Vector3 sheepDogProximity;
    public Vector3 collisionCourse = new Vector3(0, 0, 0);
    private int bounds = 40;
    private float wolfStartPosX;

    public float sheepDogProximityX;
    public float sheepDogProximityZ;

    public bool hasBitten = false;

    // Start is called before the first frame update
    void Start()
    {
        sheepDog = GameObject.Find("Sheepdog");
        wolfStartPosX = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        sheepDogProximityX = sheepDog.transform.position.x - transform.position.x;
        sheepDogProximityZ = sheepDog.transform.position.z - transform.position.z;
        sheepDogProximity = new Vector3(sheepDogProximityX, 0 , sheepDogProximityZ);

        // track player position
        if(sheepDogProximity.x < -1.0f && wolfStartPosX > 0.0f)
        {
            collisionCourse = sheepDogProximity;
            TrackPlayer(sheepDogProximity);
        }
        else if (sheepDogProximity.x > 1.0f && wolfStartPosX < 0.0f)
        {
            collisionCourse = sheepDogProximity;
            TrackPlayer(sheepDogProximity);
        }
        else
        {
            // continue collision course once close enough
            TrackPlayer(collisionCourse);
        }

        // destroy if out of bounds
        if(transform.position.x > bounds || transform.position.x < -bounds)
        {
            Destroy(gameObject);
        }
        if (transform.position.z > bounds || transform.position.z < -bounds)
        {
            Destroy(gameObject);
        }

        void TrackPlayer(Vector3 direction)
        {
            alignDirection = (direction).normalized;
            if (alignDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(alignDirection);
            }
            transform.Translate(alignDirection * speed * Time.deltaTime);
        }

    }
}
