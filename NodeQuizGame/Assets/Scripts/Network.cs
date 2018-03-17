using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System.IO;


public class Network : MonoBehaviour {
    static SocketIOComponent socket;
    public DataController allData;

    public HighScorePlayerData[] theData;



    // Use this for initialization
    void Start() {
        DontDestroyOnLoad(gameObject);
        socket = GetComponent<SocketIOComponent>();
        //allData.LoadGameData();

        socket.On("OnConnected", OnConnected);

        socket.On("GetHighScore", GetHighScore);
        socket.On("getQuestions", GetQuestions);

        HighScoreMove();   //ask node to send highscore here

        /*socket.On ("spawn player", OnSpawned);
		socket.On ("disconnected", OnDisconnected);
		socket.On ("move", OnMove);
		players = new Dictionary<string, GameObject> ();*/
    }

    void OnConnected(SocketIOEvent e)
    {
        HighScoreMove();
    }


    void GetQuestions(SocketIOEvent e)
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

    void GetHighScore(SocketIOEvent e)  //recieve highscores from mongo
    {
       
        string highScoreDataFilePath = "/StreamingAssets/highScore.json";
        Debug.Log("highscore recieved");
        
        string filePath = Application.dataPath + highScoreDataFilePath;
        File.WriteAllText(filePath, e.data["highScoreData"].ToString());            //throw it in highscore json

        string dataAsJson = File.ReadAllText(filePath);

        dataAsJson = dataAsJson.Insert(0, @"{""HighScorePlayerData"": "); 
        dataAsJson = dataAsJson + "}";
       
        Debug.Log(dataAsJson);

        theData = JsonHelper.FromJson<HighScorePlayerData>(dataAsJson);

        
        
        //allData.allHighScoreData.highScore = JsonHelper.FromJson<HighScorePlayerData>(dataAsJson);
        //Debug.Log(allData.allHighScoreData.highScore.Length);
        


        
    }

    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }
        

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }

    void HighScoreMove()
    {
        Debug.Log("EMITTING highscoremove");
        socket.Emit("highscoremove");
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
	void OnHighScoreMove(SocketIOEvent e){
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
