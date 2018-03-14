using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class DataController : MonoBehaviour {

    public RoundData[] allRoundData;
    private string gameDataFileName = "data.json";

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(gameObject);



        

        SceneManager.LoadScene("MenuScreen");
	}
	
	// Update is called once per frame
	public RoundData GetCurrentRoundData () {
        return allRoundData[0];
	}

    public void LoadGameData()
    {

        // Path.Combine combines strings into a file path
        // Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build
        string filePath = Path.Combine(Application.streamingAssetsPath, gameDataFileName);

        if (File.Exists(filePath))
        {
            // Read the json from the file into a string
            string dataAsJson = File.ReadAllText(filePath);

            dataAsJson = dataAsJson.Remove(2, 13);
            
            dataAsJson = dataAsJson.Insert(0, @"{""allRoundData"":");
            dataAsJson = dataAsJson + "}";

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

    
}
