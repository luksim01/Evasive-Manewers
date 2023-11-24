using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI timeRemainingText;
    public TextMeshProUGUI herdMultiplierText;
    public TextMeshProUGUI scoreText;

    public TextMeshProUGUI finalScoreText;

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
        timeRemaining = 60;
        herdSize = 4;
        score = 0;
        isGameActive = true;
        StartCoroutine(TimeCountdown());
    }

    private void FixedUpdate()
    {
        sheepdogHealth = sheepDog.GetComponent<PlayerController>().health;
        if (sheepdogHealth >= 0)
        {
            healthText.text = "Health " + new string('O', sheepdogHealth);
        }
        herd = GameObject.FindGameObjectsWithTag("Sheep");
        herdSize = herd.Length;
    }

    IEnumerator TimeCountdown()
    {
        while (isGameActive)
        {
            if (timeRemaining < 0)
            {
                GameOver();
                isGameActive = false;
            }
            timeRemainingText.text = "Time " + timeRemaining;
            scoreText.text = "Score " + score;
            herdMultiplierText.text = "Herd x" + herdSize;
            yield return new WaitForSeconds(1);
            if(timeRemaining > 0)
            {
                score += (10 * herdSize);
            }
            timeRemaining -= 1;
            
        }
    }

    public void GameOver()
    {
        //gameOverText.gameObject.SetActive(true);
        //finalScoreText.gameObject.SetActive(true);
        //replayButton.gameObject.SetActive(true);
        finalScoreText.text = "Final Score " + score;
        hudScreen.SetActive(false);
        gameOverScreen.SetActive(true);
        isGameActive = false;
    }

    public void ReplayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
