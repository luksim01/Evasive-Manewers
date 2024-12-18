using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Profiling;
using UnityEngine.Rendering.PostProcessing;

public class UIManager : MonoBehaviour, IUIManager
{
    public TextMeshProUGUI titleScreenText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI healthBarText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI timeRemainingText;
    public TextMeshProUGUI herdText;
    public TextMeshProUGUI herdMultiplierText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI scoreValueText;

    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI reasonText;

    public GameObject gameOverScreen;
    public GameObject hudScreen;

    public bool IsGameActive { get; set; }
    public int TimeRemaining { get; set; }
    public int Score { get; set; }
    private int previousScore;

    private int herdCount;
    private int previousHerdCount;
    private int strayCount;

    // player
    private IPlayerController _sheepdog;
    private int sheepdogHealth;
    private int previousSheepdogHealth;

    // spawn manager
    private ISpawnManager _spawnManager;

    // dependancies
    public void SetDependencies(IPlayerController playerController, SpawnManager spawnManager)
    {
        _sheepdog = playerController;
        _spawnManager = spawnManager;
    }

    // pause
    [SerializeField] private bool isPaused = false;

    // camera
    PostProcessVolume postprocessVolume;
    PostProcessLayer postprocessLayer;

    private void Awake()
    {
        IsGameActive = true;
        postprocessVolume = Camera.main.gameObject.GetComponent<PostProcessVolume>();
        postprocessLayer = Camera.main.gameObject.GetComponent<PostProcessLayer>();
    }

    void Start()
    {
        // initialise 
        TimeRemaining = 90;
        herdCount = 3;
        Score = 0;
        StartCoroutine(FadeOutTitle());

        // make GUI invisible initially
        healthText.CrossFadeAlpha(0.0f, 0.0f, false);
        healthBarText.CrossFadeAlpha(0.0f, 0.0f, false);
        timeText.CrossFadeAlpha(0.0f, 0.0f, false);
        timeRemainingText.CrossFadeAlpha(0.0f, 0.0f, false);
        herdText.CrossFadeAlpha(0.0f, 0.0f, false);
        herdMultiplierText.CrossFadeAlpha(0.0f, 0.0f, false);
        scoreText.CrossFadeAlpha(0.0f, 0.0f, false);
        scoreValueText.CrossFadeAlpha(0.0f, 0.0f, false);

        StartCoroutine(FadeInHUD());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!isPaused)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }

        if (IsGameActive)
        {
            sheepdogHealth = _sheepdog.Health;

            if (sheepdogHealth != previousSheepdogHealth)
            {
                healthBarText.text = new string('O', sheepdogHealth) + new string('-', 5 - sheepdogHealth); ;
            }

            herdCount = _spawnManager.Herd.Count;
            strayCount = _spawnManager.Strays.Count;

            if (herdCount != previousHerdCount)
            {
                herdMultiplierText.text = herdCount.ToString();
            }

            if (Score != previousScore)
            {
                scoreValueText.text = Score.ToString();
            }

            if (herdCount == 0 && strayCount == 0)
            {
                reasonText.text = "The herd was lost...";
                GameOver();
            }
            if(sheepdogHealth == 0)
            {
                reasonText.text = "The dog is too weak to continue...";
                GameOver();
            }

            previousSheepdogHealth = sheepdogHealth;
            previousHerdCount = herdCount;
            previousScore = Score;
        }
    }

    public void EnablePostProcessing()
    {
        postprocessLayer.enabled = true;
        postprocessVolume.enabled = true;
    }

    public void DisablePostProcessing()
    {
        postprocessLayer.enabled = false;
        postprocessVolume.enabled = false;
    }

    void PauseGame()
    {
        isPaused = true;
        Profiler.enabled = false;
        Time.timeScale = 0f;
        EnablePostProcessing();
    }

    void ResumeGame()
    {
        isPaused = false;
        Profiler.enabled = true;
        Time.timeScale = 1f;
        DisablePostProcessing();
    }

    IEnumerator FadeOutTitle()
    {
        yield return new WaitForSeconds(2);
        titleScreenText.CrossFadeAlpha(0.0f, 1.0f, false);
    }

    IEnumerator FadeInHUD()
    {
        yield return new WaitForSeconds(2);

        healthText.CrossFadeAlpha(1.0f, 1.0f, false);
        healthBarText.CrossFadeAlpha(1.0f, 1.0f, false);
        timeText.CrossFadeAlpha(1.0f, 1.0f, false);
        timeRemainingText.CrossFadeAlpha(1.0f, 1.0f, false);
        herdText.CrossFadeAlpha(1.0f, 1.0f, false);
        herdMultiplierText.CrossFadeAlpha(1.0f, 1.0f, false);
        scoreText.CrossFadeAlpha(1.0f, 1.0f, false);
        scoreValueText.CrossFadeAlpha(1.0f, 1.0f, false);

        StartCoroutine(TimeCountdown());
    }

    IEnumerator TimeCountdown()
    {
        timeRemainingText.text = TimeRemaining.ToString();

        yield return new WaitForSeconds(1);

        TimeRemaining -= 1;

        if (TimeRemaining >= 0)
        {
            Score += (10 * herdCount);
        }

        if (TimeRemaining >= 0)
        {
            StartCoroutine(TimeCountdown());
        }

        if (TimeRemaining < 0)
        {
            reasonText.text = "You have escaped the forest!";
            IsGameActive = false;
            GameOver();
        }   
    }

    public void GameOver()
    {
        finalScoreText.text = "Final Score " + Score;
        MemoryManager.instance.score = Score;
        hudScreen.SetActive(false);
        gameOverScreen.SetActive(true);
        IsGameActive = false;
        EnablePostProcessing();
    }

    public void BeginGame()
    {
        SceneManager.LoadScene(0);
    }

    public void SubmitScore()
    {
        SceneManager.LoadScene(1);
    }
}

public class MockUIManager : IUIManager
{
    public bool IsGameActive { get; set; }
    public int TimeRemaining { get; set; }
    public int Score { get; set; }
}
