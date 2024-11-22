using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour, IUIManager
{
    public TextMeshProUGUI titleScreenText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI timeRemainingText;
    public TextMeshProUGUI herdMultiplierText;
    public TextMeshProUGUI scoreText;

    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI reasonText;

    public GameObject gameOverScreen;
    public GameObject hudScreen;

    public bool IsGameActive { get; set; }
    public int TimeRemaining { get; set; }
    public int Score { get; set; }

    //public bool isGameActive;

    private GameObject[] herd;
    private int herdSize;

    // player
    private IPlayerController _sheepdog;
    private int sheepdogHealth;

    // dependancies
    public void SetDependencies(IPlayerController playerController)
    {
        _sheepdog = playerController;
    }

    private void Awake()
    {
        IsGameActive = true;
    }

    void Start()
    {
        // initialise 
        TimeRemaining = 90;
        herdSize = 3;
        Score = 0;
        StartCoroutine(FadeOutTitle());

        // make GUI invisible initially
        healthText.CrossFadeAlpha(0.0f, 0.0f, false);
        timeRemainingText.CrossFadeAlpha(0.0f, 0.0f, false);
        herdMultiplierText.CrossFadeAlpha(0.0f, 0.0f, false);
        scoreText.CrossFadeAlpha(0.0f, 0.0f, false);

        StartCoroutine(FadeInHUD());
    }

    private void Update()
    {
        if (IsGameActive)
        {
            sheepdogHealth = _sheepdog.Health;
            if (sheepdogHealth >= 0)
            {
                healthText.text = "Health " + new string('■', sheepdogHealth) + new string('□', 5-sheepdogHealth);
            }
            herd = GameObject.FindGameObjectsWithTag("Sheep");
            GameObject straySheep = GameObject.FindGameObjectWithTag("Stray");
            GameObject huntedSheep = GameObject.FindGameObjectWithTag("Hunted");
            herdSize = herd.Length + (huntedSheep ? 1 : 0);

            if (herdSize == 0 && !straySheep)
            {
                reasonText.text = "The herd was lost...";
                GameOver();
            }
            if(sheepdogHealth == 0)
            {
                reasonText.text = "The dog is too weak to continue...";
                GameOver();
            }
        }
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
        timeRemainingText.CrossFadeAlpha(1.0f, 1.0f, false);
        herdMultiplierText.CrossFadeAlpha(1.0f, 1.0f, false);
        scoreText.CrossFadeAlpha(1.0f, 1.0f, false);
        
        StartCoroutine(TimeCountdown());
    }

    IEnumerator TimeCountdown()
    {
        while (IsGameActive)
        {
            if (TimeRemaining < 0)
            {
                reasonText.text = "You have escaped the forest!";
                GameOver();
                IsGameActive = false;
            }
            timeRemainingText.text = "Time " + TimeRemaining;
            scoreText.text = "Score " + Score;
            herdMultiplierText.text = "Herd x" + herdSize;

            yield return new WaitForSeconds(1);

            if (TimeRemaining > 0)
            {
                Score += (10 * herdSize);
            }
            TimeRemaining -= 1;
            
        }
    }

    public void GameOver()
    {
        finalScoreText.text = "Final Score " + Score;
        MemoryManager.instance.score = Score;
        hudScreen.SetActive(false);
        gameOverScreen.SetActive(true);
        IsGameActive = false;
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
