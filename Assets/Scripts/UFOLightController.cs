using UnityEngine;

public class UFOLightController : MonoBehaviour
{
    void FixedUpdate()
    {
        this.transform.Rotate(0, 90f * Time.fixedDeltaTime, 0, Space.World);
    }
}
