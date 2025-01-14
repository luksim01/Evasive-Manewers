using UnityEngine;

public class BackWolfController : MonoBehaviour
{
    float startPositionZ;
    float directionZ;
    float startOffsetZ;
    float movementLimitZ = 0.5f;
    float speed = 1f;

    void Start()
    {
        startOffsetZ = Random.Range(-0.5f, 0.5f);
        startPositionZ = this.transform.position.z;
        this.transform.position = this.transform.position + new Vector3(0, 0, startOffsetZ);

        switch (Random.Range(0, 2))
        {
            case 0:
                directionZ = -1;
                break;
            case 1:
                directionZ = 1;
                break;
            default:
                break;
        }        
    }

    void FixedUpdate()
    {
        this.transform.Translate(Vector3.forward * directionZ * Time.fixedDeltaTime * speed);
        if(this.transform.position.z >= startPositionZ + movementLimitZ || this.transform.position.z <= startPositionZ - movementLimitZ)
        {
            directionZ *= -1;
        }
    }
}
