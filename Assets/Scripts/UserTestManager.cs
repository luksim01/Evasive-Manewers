using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;
using System;

public class UserTestManager : MonoBehaviour
{
    public static UserTestManager instance;

    public GameObject nameInputField;
    public GameObject nameArea;
    public TextMeshProUGUI nameText;
    public GameObject playButton;
    public string userName;
    public int gameCount = 0;

    [System.Serializable]
    class UserEventData
    {
        public string testUser;
        public int gameCount;
        public long epochTimestamp;
        public string eventTriggered;
    }

    private void Awake()
    {
        // singleton 
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        nameText.text = "";
        nameArea.SetActive(false);
        playButton.SetActive(false);
    }

    public void SaveUserEventData(string eventInfo)
    {
        UserEventData userEventData = new UserEventData();
        userEventData.testUser = userName;
        userEventData.gameCount = gameCount;
        userEventData.epochTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        userEventData.eventTriggered = eventInfo;

        string json = JsonUtility.ToJson(userEventData);

        //File.WriteAllText(Application.persistentDataPath + "/userEventData.json", json);
        File.AppendAllText(Application.persistentDataPath + "/userEventData.json", json);
    }

    public void ReadNameInput(string name)
    {
        nameText.text = name;
        userName = name;
        nameInputField.SetActive(false);
        nameArea.SetActive(true);
        playButton.SetActive(true);
    }

    public void IncrementGameCount()
    {
        gameCount++;
    }

    public void BeginGame()
    {
        IncrementGameCount();
        SceneManager.LoadScene("Alpha");
    }
}
