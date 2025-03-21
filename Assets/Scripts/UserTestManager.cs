using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;
using System;
using System.Text;
using UnityEngine.Networking;
using System.Collections;


public class UserTestManager : MonoBehaviour
{
    public static UserTestManager instance;

    public GameObject nameInputField;
    public GameObject nameArea;
    public TextMeshProUGUI nameText;
    public GameObject nameInputInstruction;
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
        StartCoroutine(SendDataToServer(json));
    }

    public void ReadNameInput(string name)
    {
        nameText.text = name;
        userName = name;
        nameInputField.SetActive(false);
        nameArea.SetActive(true);
        nameInputInstruction.SetActive(false);
        BeginGame();
        //playButton.SetActive(true);
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

    public IEnumerator SendDataToServer(string json)
    {
        string url = "https://evasive-manewers-backend.onrender.com/saveEvent";

        using (UnityWebRequest request = UnityWebRequest.Post(url, json, "application/json"))
        {
            yield return request.SendWebRequest();

            //if (request.result == UnityWebRequest.Result.Success)
            //    Debug.Log("Data sent successfully: " + request.downloadHandler.text);
            //else
            //    Debug.LogError("Error sending data: " + request.error);
        }
    }
}
