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
			LoadGameData ();
		}
	
	}

	void LoadGameData(){
		string filePath = Application.dataPath + gameDataFilePath;

		if (File.Exists (filePath)) {
			string gameData = File.ReadAllText (filePath);
			editorData = JsonUtility.FromJson<GameData> (gameData);
		} else {
			editorData = new GameData ();
		}
	}

	void SaveGameData(){
		string jsonObj = JsonUtility.ToJson (editorData);

		string filePath = Application.dataPath + gameDataFilePath;
		File.WriteAllText (filePath, jsonObj);
	}

	void SendGameData(){
		string jsonObj = JsonUtility.ToJson (editorData);
		server = GameObject.Find ("server");
		socket = server.GetComponent<SocketIOComponent> ();
		socket.Emit ("send data", new JSONObject(jsonObj));

	}
}
