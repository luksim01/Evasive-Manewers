using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraController : MonoBehaviour
{
    PostProcessVolume ppVolume;

    // ui
    private IUIManager _uiManager;

    // dependancies
    public void SetDependencies(IUIManager uiManager)
    {
        _uiManager = uiManager;
    }

    void Start()
    {
        ppVolume = Camera.main.gameObject.GetComponent<PostProcessVolume>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!_uiManager.IsGameActive)
        {
            ppVolume.enabled = true;
        }
    }
}
