using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using UnityEngine.UI;

public class UIManagerLeaderboard : MonoBehaviour
{
    public TextMeshProUGUI finalScoreText;

    public TextMeshProUGUI[] scoresText;

    public TextMeshProUGUI[] namesText;

    public GameObject nameInputField;

    int[] topScores = new int[6];

    string[] topNames = new string[6];

    int newScore;
    string newName;

    //public InputField myInputField;
    //public int characterLimit = 10;

    public Dictionary<int, Dictionary<string, object>> scoreboard;

    private void Start()
    {
        newScore = MemoryManager.instance.score;
        finalScoreText.text = "Final Score " + newScore;

        DisplayScoreboard(5);
        nameInputField.SetActive(true);
    }

    private void DisplayScoreboard(int length)
    {
        MemoryManager.instance.LoadScores();

        topScores[0] = MemoryManager.instance.score_1st;
        topScores[1] = MemoryManager.instance.score_2nd;
        topScores[2] = MemoryManager.instance.score_3rd;
        topScores[3] = MemoryManager.instance.score_4th;
        topScores[4] = MemoryManager.instance.score_5th;
        
        topNames[0] = MemoryManager.instance.name_1st;
        topNames[1] = MemoryManager.instance.name_2nd;
        topNames[2] = MemoryManager.instance.name_3rd;
        topNames[3] = MemoryManager.instance.name_4th;
        topNames[4] = MemoryManager.instance.name_5th;
        
        for (int i = 0; i < length; i++)
        {
            namesText[i].text = topNames[i];
            scoresText[i].text = topScores[i].ToString();
        }
    }

    public void ReadNameInput(string nameInput)
    {
        topNames[5] = nameInput;
        topScores[5] = newScore;
        PopulateScoreboard(6);
        ReorderScoreboard(6);
        nameInputField.SetActive(false);
    }

    private void PopulateScoreboard(int length)
    {
        scoreboard = new Dictionary<int, Dictionary<string, object>>();

        for (int i = 0; i < length; i++)
        {
            AddToScoreboard(i, "name", topNames[i], "score", topScores[i]);
            //Debug.Log("PopulateScoreboard(6): " + i + " - name - " + topNames[i] + " - score - " + topScores[i]);
        }
    }

    private void ReorderScoreboard(int length)
    {
        int[] sortedTopScores = new int[topScores.Length];
        Array.Copy(topScores, sortedTopScores, topScores.Length);
        Array.Sort(sortedTopScores);
        Array.Reverse(sortedTopScores);

        for (int i = 0; i < topScores.Length; i++)
        {
            //Debug.Log("ReorderScoreboard(6): original - " + topScores[i] + " - sorted - " + sortedTopScores[i]);
        }

        // assigns ranks for leaderboard against scores
        int[] newRanks = new int[topScores.Length];
        for (int i = 0; i < topScores.Length; i++)
        {
            newRanks[i] = Array.IndexOf(sortedTopScores, topScores[i]);
            //Debug.Log(i + ": new rank - " + newRanks[i]);
        }

        // finds unique ranks if there are duplicate scores
        List<int> uniqueRanks = new List<int>();
        foreach (int rank in newRanks)
        {
            if (!uniqueRanks.Contains(rank))
            {
                uniqueRanks.Add(rank);
                //Debug.Log("ReorderScoreboard(6): unique rank - " + rank);
            }
        }


        List<int> order = new List<int>();
        // organises score values into updated order
        foreach(int uniqueRank in uniqueRanks)
        {
            for (int j = 0; j < newRanks.Length; j++)
            {
                //Debug.Log("UNIQUE RANK " + uniqueRank + "   FOLLOWED BY - " + newRanks[j]);
                if (newRanks[j] == uniqueRank)
                {
                    order.Add(newRanks[j]);
                    //Debug.Log("MATCH: order - " + j);
                }
            }
        }

        // updates names and scores with updated order
        for (int i = 0; i < length; i++)
        {
                topNames[order[i]] = (string)scoreboard[i]["name"];
                topScores[order[i]] = (int)scoreboard[i]["score"];
                //Debug.Log("ReorderScoreboard(6): rank - " + order[i] + " name - " + topNames[order[i]] + " - score - " + topScores[order[i]]);
        }

        RefreshScoreboard(5);
        SaveScoreboard();
    }

    private void RefreshScoreboard(int length)
    {
        for (int i = 0; i < length; i++)
        {
            namesText[i].text = topNames[i];
            scoresText[i].text = topScores[i].ToString();
        }
    }

    private void AddToScoreboard(int rank, string nameKey, object nameValue, string scoreKey, object scoreValue)
    {
        if (!scoreboard.ContainsKey(rank))
        {
            scoreboard.Add(rank, new Dictionary<string, object>());
        }

        scoreboard[rank].Add(nameKey, nameValue);
        scoreboard[rank].Add(scoreKey, scoreValue);
    }

    private void SaveScoreboard()
    {
        MemoryManager.instance.score_1st = topScores[0];
        MemoryManager.instance.score_2nd = topScores[1];
        MemoryManager.instance.score_3rd = topScores[2];
        MemoryManager.instance.score_4th = topScores[3];
        MemoryManager.instance.score_5th = topScores[4];

        MemoryManager.instance.name_1st = topNames[0];
        MemoryManager.instance.name_2nd = topNames[1];
        MemoryManager.instance.name_3rd = topNames[2];
        MemoryManager.instance.name_4th = topNames[3];
        MemoryManager.instance.name_5th = topNames[4];

        MemoryManager.instance.SaveScores();
    }

    public void BeginGame()
    {
        SceneManager.LoadScene(0);
    }
}
