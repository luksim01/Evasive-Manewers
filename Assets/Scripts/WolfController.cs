using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfController : MonoBehaviour
{
    private float speed = 10.0f;
    private GameObject sheepDog;
    private Vector3 alignDirection;
    private Vector3 sheepDogProximity;
    private Vector3 collisionCourse = new Vector3(0, 0, 0);
    private int bounds = 40;
    private float wolfStartPosX;

    private float sheepDogProximityX;
    private float sheepDogProximityZ;

    public bool hasBitten = false;
    private bool isGameActive;

    private bool isCharging = false;

    // Start is called before the first frame update
    void Start()
    {
        sheepDog = GameObject.Find("Sheepdog");
        wolfStartPosX = transform.position.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        isGameActive = GameObject.Find("UIManager").GetComponent<UIManager>().isGameActive;

        if (isGameActive)
        {
            sheepDogProximityX = sheepDog.transform.position.x - transform.position.x;
            sheepDogProximityZ = sheepDog.transform.position.z - transform.position.z;
            sheepDogProximity = new Vector3(sheepDogProximityX, 0, sheepDogProximityZ);

            // track player position
            if (wolfStartPosX > 0 && !isCharging)
            {
                collisionCourse = sheepDogProximity;
                TrackPlayer(sheepDogProximity);
            }

            if (wolfStartPosX < 0 && !isCharging)
            {
                collisionCourse = sheepDogProximity;
                TrackPlayer(sheepDogProximity);
            }

            // once close enough, charge at player in one direction
            if (sheepDogProximity.z <= 9 && transform.position.x < 7.7f && transform.position.x > -7.7f)
            {
                isCharging = true;
            }
            if (isCharging)
            {
                TrackPlayer(collisionCourse);
            }

            // destroy if out of bounds
            if (transform.position.x > bounds || transform.position.x < -bounds)
            {
                Destroy(gameObject);
            }
            if (transform.position.z > bounds || transform.position.z < -bounds)
            {
                Destroy(gameObject);
            }
        }
    }

    void TrackPlayer(Vector3 direction)
    {
        alignDirection = (direction).normalized;
        if (alignDirection != Vector3.zero && !hasBitten)
        {
            transform.rotation = Quaternion.LookRotation(alignDirection);
        }
        transform.Translate(alignDirection * speed * Time.deltaTime);
    }
}
