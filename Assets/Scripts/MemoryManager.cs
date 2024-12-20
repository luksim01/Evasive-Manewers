using UnityEngine;
using System.IO;

public class MemoryManager : MonoBehaviour
{
    public static MemoryManager instance;

    public int score;

    public int score_1st;
    public int score_2nd;
    public int score_3rd;
    public int score_4th;
    public int score_5th;

    public string name_1st;
    public string name_2nd;
    public string name_3rd;
    public string name_4th;
    public string name_5th;

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
    }

    [System.Serializable]
    class SaveData
    {
        public int score_1st;
        public int score_2nd;
        public int score_3rd;
        public int score_4th;
        public int score_5th;

        public string name_1st;
        public string name_2nd;
        public string name_3rd;
        public string name_4th;
        public string name_5th;
    }

    public void SaveScores()
    {
        SaveData savedData = new SaveData();
        savedData.score_1st = score_1st;
        savedData.score_2nd = score_2nd;
        savedData.score_3rd = score_3rd;
        savedData.score_4th = score_4th;
        savedData.score_5th = score_5th;

        savedData.name_1st = name_1st;
        savedData.name_2nd = name_2nd;
        savedData.name_3rd = name_3rd;
        savedData.name_4th = name_4th;
        savedData.name_5th = name_5th;

        string json = JsonUtility.ToJson(savedData);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadScores()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData savedData = JsonUtility.FromJson<SaveData>(json);

            score_1st = savedData.score_1st;
            score_2nd = savedData.score_2nd;
            score_3rd = savedData.score_3rd;
            score_4th = savedData.score_4th;
            score_5th = savedData.score_5th;

            name_1st = savedData.name_1st;
            name_2nd = savedData.name_2nd;
            name_3rd = savedData.name_3rd;
            name_4th = savedData.name_4th;
            name_5th = savedData.name_5th;
        }
    }
}
