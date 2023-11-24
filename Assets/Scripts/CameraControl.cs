using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraControl : MonoBehaviour
{
    private bool isGameActive;
    PostProcessVolume ppVolume;

    void Start()
    {
        ppVolume = Camera.main.gameObject.GetComponent<PostProcessVolume>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        isGameActive = GameObject.Find("UIManager").GetComponent<UIManager>().isGameActive;

        if (!isGameActive)
        {
            ppVolume.enabled = true;
        }
    }
}
