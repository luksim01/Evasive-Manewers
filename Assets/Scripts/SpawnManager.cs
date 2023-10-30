using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject wolf;
    private GameObject sheepDog;
    private float wolfSpawnUpperZ = -5.0f;
    private float wolfSpawnLowerZ = -15.0f;
    private float wolfSpawnX = 30.0f;
    private float sheepDogPosZ;
    private string[] wolfOrigin = { "left", "right" };
    private int wolfOriginIndex;
    Vector3 wolfSpawnPos;

    public float wolfSpawnInterval;
    private float wolfSpawnIntervalLower = 10.0f;
    private float wolfSpawnIntervalUpper = 15.0f;

    // Start is called before the first frame update
    void Start()
    {
        sheepDog = GameObject.Find("Sheepdog");
        Invoke("SpawnWolf", wolfSpawnIntervalUpper);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnWolf()
    {
        wolfOriginIndex = Random.Range(0, wolfOrigin.Length);
        sheepDogPosZ = sheepDog.transform.position.z;

        if (wolfOrigin[wolfOriginIndex] == "right")
        {
            wolfSpawnPos = new Vector3(wolfSpawnX, 2.4f, sheepDogPosZ + Random.Range(wolfSpawnLowerZ, wolfSpawnUpperZ));
        }
        if (wolfOrigin[wolfOriginIndex] == "left")
        {
            wolfSpawnPos = new Vector3(-wolfSpawnX, 2.4f, sheepDogPosZ + Random.Range(wolfSpawnLowerZ, wolfSpawnUpperZ));
        }
        Instantiate(wolf, wolfSpawnPos, wolf.transform.rotation);

        wolfSpawnInterval = Random.Range(wolfSpawnIntervalLower,wolfSpawnIntervalUpper);
        Invoke("SpawnWolf", wolfSpawnInterval);
    }
}
