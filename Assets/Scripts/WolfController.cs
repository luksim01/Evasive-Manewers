using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfController : MonoBehaviour
{
    public float speed = 3000.0f;
    private GameObject sheepDog;
    Vector3 alignDirection;
    public Vector3 sheepDogProximity;
    public Vector3 collisionCourse;


    // Start is called before the first frame update
    void Start()
    {
        sheepDog = GameObject.Find("Sheepdog");
    }

    // Update is called once per frame
    void Update()
    {
        sheepDogProximity = sheepDog.transform.position - transform.position;
        //Debug.Log("Proximity: x: " + Mathf.Abs(sheepDogProximity.x) + " z: " + Mathf.Abs(sheepDogProximity.z));

        // track player position
        if(sheepDogProximity.x > 1.0f)
        {
            collisionCourse = sheepDogProximity;
            alignDirection = (sheepDogProximity).normalized;
            transform.rotation = Quaternion.LookRotation(alignDirection);
            transform.Translate(alignDirection * speed * Time.deltaTime);
        }
        else
        {
            // continue collision course once close enough
            alignDirection = (collisionCourse).normalized;
            transform.rotation = Quaternion.LookRotation(alignDirection);
            transform.Translate(alignDirection * speed * 1.5f * Time.deltaTime);
        }
        
    }
}
