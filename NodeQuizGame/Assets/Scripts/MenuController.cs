using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SocketIO;

public class MenuController : MonoBehaviour {

    private DataController allData;
    private SocketIOComponent serverSocket;

    public void Start()
    {
        allData = GameObject.Find("DataController").GetComponent<DataController>();
        serverSocket = GameObject.Find("server").GetComponent<SocketIOComponent>();
        Debug.Log(GameObject.Find("DataController").GetComponent<DataController>());
    }

    // Use this for initialization
    public void StartGame () {
        SceneManager.LoadScene("Game");
        
	}

    public void SaveData()
    {
        JSONObject json = new JSONObject(JsonUtility.ToJson(allData.allRoundData[0]));
        Debug.Log(json);
        serverSocket.Emit("move", json);
    }

    public void LoadData()
    {
        Debug.Log("ATTEMPT");
        serverSocket.Emit("sendData");

    }

}
