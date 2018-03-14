using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System.IO;

public class Network : MonoBehaviour {
 	static SocketIOComponent socket;
    public DataController allData;
    
	

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
        socket = GetComponent<SocketIOComponent> ();
        //allData.LoadGameData();

        socket.On("OnConnected", OnConnected);

        
        socket.On ("getQuestions", GetQuestions);
		/*socket.On ("spawn player", OnSpawned);
		socket.On ("disconnected", OnDisconnected);
		socket.On ("move", OnMove);
		players = new Dictionary<string, GameObject> ();*/
	}

    void OnConnected(SocketIOEvent e)
    {
        
        
        //socket.Emit("move", json);
    }


    void GetQuestions (SocketIOEvent e)
    {
        string gameDataFilePath = "/StreamingAssets/data.json";
        Debug.Log("QUESTION RECEIVED");
        
        Debug.Log(e.data["questionData"]);
        
        string filePath = Application.dataPath + gameDataFilePath;
        File.WriteAllText(filePath, e.data["questionData"].ToString());

        allData.LoadGameData(false);

        //string gameData = File.ReadAllText(filePath);
        //allData = JsonUtility.FromJson<DataController>(gameData);


        //Debug.Log(e.data["favorites"]);

    }
    /*
	// Tells us we are connected
	void OnConnected (SocketIOEvent e) {
		Debug.Log ("We are Connected");
		//socket.Emit ("playerhere");
	}

	void OnSpawned(SocketIOEvent e){
		Debug.Log ("Player Spawned!" + e.data);
		//var player = Instantiate (playerPrefab);
		//players.Add (e.data ["id"].ToString(), player);
		//Debug.Log ("count " + players.Count);
	}

	void OnDisconnected(SocketIOEvent e){
		Debug.Log ("player disconnected: " + e.data);

		var id = e.data ["id"].ToString ();

		var player = players [id];
		Destroy (player);
		//players.Remove (id);
	}
	void OnMove(SocketIOEvent e){
		Debug.Log ("Networked player is moving " + e.data);

		var id = e.data["id"].ToString();
		var player = players [id];

		var pos = new Vector3 (GetFloatFromJson(e.data,"x"), 0 ,GetFloatFromJson(e.data,"y"));
		var h = GetFloatFromJson(e.data, "h");
		var v = GetFloatFromJson(e.data, "v");
		Debug.Log ("pos: " + pos);
		var netMove = player.GetComponent<CharacterMovement> ();

		netMove.NetworkMovement (pos, h, v);
	}
    */
	string GetStringFromJson(JSONObject data, string key){
		return data [key].ToString().Replace("\"", "");
	}

    
}
