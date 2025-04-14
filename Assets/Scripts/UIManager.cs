using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Profiling;
using System;

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
    public TextMeshProUGUI gameText;
    public TextMeshProUGUI gameCountValueText;

    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI reasonText;

    public GameObject gameOverScreen;
    public GameObject hudScreen;
    public GameObject pauseScreen;
    public GameObject instructionScreen;

    public bool IsGameActive { get; set; }
    public int TimeRemaining { get; set; }
    public int Score { get; set; }
    private int previousScore;
    private int previousGameCount;

    private int herdCount;
    private int previousHerdCount;
    private int strayCount;

    // player
    private IPlayerController _sheepdog;
    private int sheepdogHealth;
    private int previousSheepdogHealth;

    // spawn manager
    private ISpawnManager _spawnManager;

    // post processing
    public GameObject postprocessingVolume;

    // dependancies
    public void SetDependencies(IPlayerController playerController, SpawnManager spawnManager)
    {
        _sheepdog = playerController;
        _spawnManager = spawnManager;
    }

    // pause
    [SerializeField] private bool isPaused = false;

    private void Awake()
    {
        IsGameActive = true;
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
        gameText.CrossFadeAlpha(0.0f, 0.0f, false);
        gameCountValueText.CrossFadeAlpha(0.0f, 0.0f, false);

        StartCoroutine(FadeInHUD());
    }

    private void Update()
    {
        if (IsGameActive)
        {
            sheepdogHealth = _sheepdog.Health;

            if (sheepdogHealth != previousSheepdogHealth)
            {
                healthBarText.text = new string('O', sheepdogHealth) + new string('-', 5 - sheepdogHealth); ;
            }

            herdCount = _spawnManager.Herd.Count + _spawnManager.Hunted.Count;
            strayCount = _spawnManager.Strays.Count;

            if (herdCount != previousHerdCount)
            {
                herdMultiplierText.text = herdCount.ToString();
            }

            if (Score != previousScore)
            {
                scoreValueText.text = Score.ToString();
            }

            //if (UserTestManager.instance.gameCount != previousGameCount)
            //{
            //    gameCountValueText.text = UserTestManager.instance.gameCount.ToString();
            //}

            if (herdCount == 0 && strayCount == 0)
            {
                //UserTestManager.instance.SaveUserEventData("Herd Lost");
                reasonText.text = "The herd was lost...";
                GameOver();
            }
            if(sheepdogHealth == 0)
            {
                //UserTestManager.instance.SaveUserEventData("Player Lost");
                reasonText.text = "The dog is too weak to continue...";
                GameOver();
            }

            previousSheepdogHealth = sheepdogHealth;
            previousHerdCount = herdCount;
            previousScore = Score;
            //previousGameCount = UserTestManager.instance.gameCount;
        }
    }

    public void PauseResume()
    {
        if (!isPaused)
        {
            PauseGame();
            PauseProfiler();
            EnablePauseScreen();
        }
        else
        {
            ResumeGame();
            ResumeProfiler();
            DisablePauseScreen();
        }
    }

    public void EnablePostProcessing()
    {
        postprocessingVolume.SetActive(true);
    }

    public void DisablePostProcessing()
    {
        postprocessingVolume.SetActive(false);
    }

    public void PauseGame()
    {
        isPaused = true;
        _sheepdog.Move.Disable();
        _sheepdog.Jump.Disable();
        _sheepdog.BarkMove.Disable();
        _sheepdog.BarkJump.Disable();
        Time.timeScale = 0f;
        EnablePostProcessing();
    }

    public void Exit()
    {
        if (isPaused)
        {
            ResumeGame();
            SceneManager.LoadScene("Title");
        }
    }

    void PauseProfiler()
    {
        Profiler.enabled = false;
    }

    void ResumeGame()
    {
        isPaused = false;
        _sheepdog.Move.Enable();
        _sheepdog.Jump.Enable();
        _sheepdog.BarkMove.Enable();
        _sheepdog.BarkJump.Enable();
        Time.timeScale = 1f;
        DisablePostProcessing();
    }

    void EnablePauseScreen()
    {
        _sheepdog.Move.Disable();
        _sheepdog.Jump.Disable();
        _sheepdog.BarkMove.Disable();
        _sheepdog.BarkJump.Disable();
        pauseScreen.SetActive(true);
        instructionScreen.SetActive(true);
    }

    void DisablePauseScreen()
    {
        _sheepdog.Move.Enable();
        _sheepdog.Jump.Enable();
        _sheepdog.BarkMove.Enable();
        _sheepdog.BarkJump.Enable();
        pauseScreen.SetActive(false);
        instructionScreen.SetActive(false);
    }

    void ResumeProfiler()
    {
        Profiler.enabled = true;
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
        gameText.CrossFadeAlpha(1.0f, 1.0f, false);
        gameCountValueText.CrossFadeAlpha(1.0f, 1.0f, false);

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
            reasonText.text = "You have escaped!";
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
        //Debug.Log("Begin upload: " + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        //UserTestManager.instance.SaveUserEventData("Score Achieved " + Score.ToString());
        //StartCoroutine(UserTestManager.instance.SendQueuedDataToServer(PauseGame));
    }

    public void BeginGame()
    {
        ResumeGame();
        //UserTestManager.instance.IncrementGameCount();
        SceneManager.LoadScene("Alpha");
    }

    public void SubmitScore()
    {
        ResumeGame();
        SceneManager.LoadScene("Highscore");
    }
}

public class MockUIManager : IUIManager
{
    public bool IsGameActive { get; set; }
    public int TimeRemaining { get; set; }
    public int Score { get; set; }
    public void PauseResume() { }
    public void Exit() { }
}
