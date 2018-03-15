using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using SocketIO;

public class DataController : MonoBehaviour {

    public RoundData[] allRoundData;

    public List<HighScorePlayerData> allPlayerData;
    private string gameDataFileName = "data.json";
    public List<HighScoreData> allHighScoreData;
    private PlayerProgress playerProgress;
    

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(gameObject);
        LoadGameData(true);

        LoadPlayerProgress();

        SceneManager.LoadScene("MenuScreen");
        
    }

    public void SubmitNewPlayerScore(int newScore)
    {
        HighScorePlayerData newData = new HighScorePlayerData();
        
        newData.playerName = "Mike";
        newData.score = newScore.ToString();
        allPlayerData.Add(newData);

        GameObject.Find("server").GetComponent<SocketIOComponent>().Emit(JsonUtility.ToJson(newData));

        // If newScore is greater than playerProgress.highestScore, update playerProgress with the new value and call SavePlayerProgress()
        if (newScore > playerProgress.highestScore)
        {
            playerProgress.highestScore = newScore;
            SavePlayerProgress();
        }
    }

    // This function could be extended easily to handle any additional data we wanted to store in our PlayerProgress object
    private void LoadPlayerProgress()
    {
        // Create a new PlayerProgress object
        playerProgress = new PlayerProgress();

        // If PlayerPrefs contains a key called "highestScore", set the value of playerProgress.highestScore using the value associated with that key
        if (PlayerPrefs.HasKey("highestScore"))
        {
            playerProgress.highestScore = PlayerPrefs.GetInt("highestScore");
        }
    }

    // This function could be extended easily to handle any additional data we wanted to store in our PlayerProgress object
    private void SavePlayerProgress()
    {
        // Save the value playerProgress.highestScore to PlayerPrefs, with a key of "highestScore"
        PlayerPrefs.SetInt("highestScore", playerProgress.highestScore);
    }


    public int GetHighestPlayerScore()
    {
        return playerProgress.highestScore;
    }

    // Update is called once per frame
    public RoundData GetCurrentRoundData () {
        return allRoundData[0];
	}

    public void LoadGameData(bool isFromNode)
    {

        // Path.Combine combines strings into a file path
        // Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build
        string filePath = Path.Combine(Application.streamingAssetsPath, gameDataFileName);

        if (File.Exists(filePath))
        {
            // Read the json from the file into a string
            string dataAsJson = File.ReadAllText(filePath);
            if (!dataAsJson.EndsWith("}"))
            {
                dataAsJson = dataAsJson.Remove(2, 13);

                dataAsJson = dataAsJson.Insert(0, @"{""allRoundData"":");
                dataAsJson = dataAsJson + "}";
            }
            Debug.Log(dataAsJson);
            // Pass the json to JsonUtility, and tell it to create a GameData object from it
            GameData loadedData = JsonUtility.FromJson<GameData>(dataAsJson);

            // Retrieve the allRoundData property of loadedData
            allRoundData = loadedData.allRoundData;
        }
        else
        {
            Debug.LogError("Cannot load game data!");
        }
    }
    /*
    public void LoadHighScoreData()
    {

        // Path.Combine combines strings into a file path
        // Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build
        string filePath = Path.Combine(Application.streamingAssetsPath, "highScore.json");

        if (File.Exists(filePath))
        {
            // Read the json from the file into a string
            string dataAsJson = File.ReadAllText(filePath);
            
            Debug.Log(dataAsJson);
            // Pass the json to JsonUtility, and tell it to create a GameData object from it
            HighScoreData loadedData = JsonUtility.FromJson<HighScoreData>(dataAsJson);

            // Retrieve the allRoundData property of loadedData
            allPlayerData = loadedData.players;
        }
        else
        {
            Debug.LogError("Cannot load highscore data!");
        }
    }
    */

}
