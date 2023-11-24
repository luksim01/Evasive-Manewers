using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveLane : MonoBehaviour
{
    private GameObject sheepDog;

    public float laneBoundsLower;
    public float laneBoundsUpper;

    private Material laneMaterial;
    private Renderer rend;
    public Color activeLaneColour;
    

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        laneMaterial = rend.material;
        sheepDog = GameObject.Find("Sheepdog");

        float posX = transform.position.x;
        float width = transform.localScale.x;
        laneBoundsLower = posX - (width / 2);
        laneBoundsUpper = posX + (width / 2);

    }

    // Update is called once per frame
    void Update()
    {
        //if (sheepDog.transform.position.x >= laneBoundsLower && sheepDog.transform.position.x <= laneBoundsUpper)
        //{
        //    //Debug.Log(name);
        //    // REVISIT: Change color of the lane that is active
        //    //rend.material.color = activeLaneColour;
        //}
    }
}
