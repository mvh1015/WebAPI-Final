using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using SocketIO;

public class GameDataEditor : EditorWindow {
	string gameDataFilePath = "/StreamingAssets/data.json";
	public GameData editorData;
	static private GameObject server;
	static SocketIOComponent socket;

	[MenuItem("Window/Game Data Editor")]
	static void Init(){
		EditorWindow.GetWindow (typeof(GameDataEditor)).Show();

	}

	void OnGUI(){
		if(editorData != null){
			//display the data from json
			SerializedObject serializedObject = new SerializedObject(this);
			SerializedProperty serializedProperty = serializedObject.FindProperty ("editorData");
			EditorGUILayout.PropertyField (serializedProperty, true);
			serializedObject.ApplyModifiedProperties ();

			if(GUILayout.Button("Save Game Data")){
				SaveGameData ();
			}

			if(GUILayout.Button("Send Game Data")){
				SendGameData ();
			}
		}

		if(GUILayout.Button("Load Game Data")){
			LoadGameData();
		}
	
	}

    void GetQuestions(SocketIOEvent e)
    {
        string gameDataFilePath = "/StreamingAssets/data.json";
        Debug.Log("QUESTION RECEIVED");

        Debug.Log(e.data["questionData"]);

        string filePath = Application.dataPath + gameDataFilePath;
        File.WriteAllText(filePath, e.data["questionData"].ToString());

        Debug.Log("AHHH");
      

        if (File.Exists(filePath))
        {

            string dataAsJson = File.ReadAllText(filePath);
            Debug.Log(dataAsJson);
            dataAsJson = dataAsJson.Remove(2, 13);

            dataAsJson = dataAsJson.Insert(0, @"{""allRoundData"":");
            dataAsJson = dataAsJson + "}";
            editorData = JsonUtility.FromJson<GameData>(dataAsJson);
        }
        else
        {
            editorData = new GameData();
        }


    }

    void LoadGameData(){
        server = GameObject.Find("server");
        socket = server.GetComponent<SocketIOComponent>();
        socket.On("getQuestions", GetQuestions);
        socket.Emit("sendData");

    }

	void SaveGameData(){
		string jsonObj = JsonUtility.ToJson (editorData);

		string filePath = Application.dataPath + gameDataFilePath;
		File.WriteAllText (filePath, jsonObj);
	}

	void SendGameData(){
        server = GameObject.Find("server");
        socket = server.GetComponent<SocketIOComponent>();
        JSONObject json = new JSONObject(JsonUtility.ToJson(editorData.allRoundData[0]));
        Debug.Log(json);
        socket.Emit("move", json);

	}
}
