using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepController : MonoBehaviour
{
    public bool barkedAt;
    private GameObject sheepDog;

    // Start is called before the first frame update
    void Start()
    {
        sheepDog = GameObject.Find("Sheepdog");
    }

    // Update is called once per frame
    void Update()
    {
        barkedAt = sheepDog.GetComponent<PlayerController>().hasBarked;
    }
}
