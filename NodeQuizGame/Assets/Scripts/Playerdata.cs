using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Playerdata : MonoBehaviour {

	string gameDataFilePath = "/StreamingAssets/data.json";

	// Use this for initialization
	void Start () {
		/*GameData jsonObj = new GameData ();
		jsonObj.playerName = "Jordan";
		jsonObj.score = 2000;
		jsonObj.timePlayed = 12000.456f;
		jsonObj.lastLogin = "March 1, 2018";

	 	string json = JsonUtility.ToJson(jsonObj);
		string filePath = Application.dataPath + gameDataFilePath;
		File.WriteAllText (filePath, json);
		Debug.Log (json);
        */
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
