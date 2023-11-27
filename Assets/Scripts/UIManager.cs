using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
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

    public bool isGameActive;

    public int timeRemaining;
    private int score;

    private GameObject[] herd;
    private int herdSize;

    private GameObject sheepDog;
    private int sheepdogHealth;

    // Start is called before the first frame update
    void Start()
    {
        sheepDog = GameObject.Find("Sheepdog");
        sheepdogHealth = 5;
        timeRemaining = 90;
        herdSize = 4;
        score = 0;
        isGameActive = true;
        StartCoroutine(FadeOutTitle());

        healthText.CrossFadeAlpha(0.0f, 0.0f, false);
        timeRemainingText.CrossFadeAlpha(0.0f, 0.0f, false);
        herdMultiplierText.CrossFadeAlpha(0.0f, 0.0f, false);
        scoreText.CrossFadeAlpha(0.0f, 0.0f, false);

        StartCoroutine(FadeInHUD());
    }

    private void Update()
    {
        if (isGameActive)
        {
            sheepdogHealth = sheepDog.GetComponent<PlayerController>().health;
            if (sheepdogHealth >= 0)
            {
                healthText.text = "Health " + new string('■', sheepdogHealth) + new string('□', 5-sheepdogHealth);
            }
            herd = GameObject.FindGameObjectsWithTag("Sheep");
            herdSize = herd.Length;

            if (herdSize == 0)
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
        while (isGameActive)
        {
            if (timeRemaining < 0)
            {
                reasonText.text = "You have escaped the forest!";
                GameOver();
                isGameActive = false;
            }
            timeRemainingText.text = "Time " + timeRemaining;
            scoreText.text = "Score " + score;
            herdMultiplierText.text = "Herd x" + herdSize;

            yield return new WaitForSeconds(1);

            if (timeRemaining > 0)
            {
                score += (10 * herdSize);
            }
            timeRemaining -= 1;
            
        }
    }

    public void GameOver()
    {
        finalScoreText.text = "Final Score " + score;
        hudScreen.SetActive(false);
        gameOverScreen.SetActive(true);
        isGameActive = false;
    }

    public void BeginGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
