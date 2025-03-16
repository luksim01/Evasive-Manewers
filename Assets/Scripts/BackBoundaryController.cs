using UnityEngine;

public class BackBoundaryController : MonoBehaviour
{
    [SerializeField] private float speed;

    // ui
    private IUIManager _uiManager;

    // dependancies
    public void SetDependencies(IUIManager uiManager)
    {
        _uiManager = uiManager;
    }

    // dependancy manager
    [SerializeField] private DependancyManager dependancyManager;

    void Start()
    {
        dependancyManager.InjectBackBoundaryControllerDependencies(this);
    }

    private void FixedUpdate()
    {
        if (_uiManager.IsGameActive)
        {

            // back wolves and light enter scene
            if(_uiManager.TimeRemaining > 10)
            {
                if (this.transform.position.z < 0)
                {
                    this.transform.Translate(Vector3.forward * Time.fixedDeltaTime * speed);
                }
            }
            // back wolves and light leave scene
            else
            {
                if (this.transform.position.z > -14)
                {
                    this.transform.Translate(Vector3.back * Time.fixedDeltaTime * speed * 0.25f);
                }
            }
        }
    }
}