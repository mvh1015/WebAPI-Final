using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Linq;


public class GameController : MonoBehaviour {


    public Text questionText;
    public Text scoreText;
    public Text timeText;
    public Transform answerButtonParent;
    public int playerScore;
    public GameObject questionDisplay;
    public GameObject endGameDisplay;
    public Text highScoreDisplay;

    public Text playerNameText;


    public BasicObjectPool answerButtonPool;

    public DataController dataController;
    private RoundData roundData;
    private QuestionData[] questionPool;
    private bool isRoundActive;
    private int questionIndex;

    private float timeRemaining;

    private List<GameObject> answerButtonObjects = new List<GameObject>();

	// Use this for initialization
	void Start () {
        dataController = FindObjectOfType<DataController>();
        roundData = dataController.GetCurrentRoundData();
        questionPool = roundData.questions;
        playerScore = 0;
        questionIndex = 0;
        timeRemaining = roundData.timeLimitInSeconds;
        isRoundActive = true;

        ShowQuestions();

        
	}

   

    private void ShowQuestions()
    {
        RemoveAnswerButtons();
        QuestionData questionData = questionPool[questionIndex];
        questionText.text = questionData.questionText;

        for (int i = 0; i < questionData.answers.Length; i++) {
            GameObject answerButtonObject = answerButtonPool.GetObject();
            answerButtonObject.transform.SetParent(answerButtonParent);
            answerButtonObjects.Add(answerButtonObject);

            AnswerButton answerButton = answerButtonObject.GetComponent<AnswerButton>();
            answerButton.Setup(questionData.answers[i]);
        }

    }

    private void RemoveAnswerButtons()
    {
        while(answerButtonObjects.Count > 0)
        {
            answerButtonPool.ReturnObject(answerButtonObjects[0]);
            answerButtonObjects.RemoveAt(0);
        }
    }

    public void AnswerClicked(bool isCorrect)
    {
        if (isCorrect)
        {
            playerScore += roundData.pointsAddedForCorrectAnswers;
            scoreText.text = "Score: " + playerScore.ToString();
        }
        if (questionPool.Length > questionIndex + 1)
        {
            questionIndex++;
            ShowQuestions();
        } else
        {
            EndRound();
        }
    }

    public void EndRound()
    {
        isRoundActive = false;
        highScoreDisplay.text = "HighScores\n";
        int i = 0;
        int j = 0;

        Debug.Log(dataController.allHighScoreData.HighScorePlayerData.Count());
        foreach (HighScorePlayerData highScore in dataController.nonBrokenList)
            {
                j++;
                Debug.Log(j);
            if (highScore != null)
            {
                if (highScore.score != 0)
                {

                    i++;
                    highScoreDisplay.text += i.ToString() + ". " + highScore.playerName + "   " + highScore.score + "\n";
                }
                else
                {
                    highScore.score = 0;
                }
            }
            }
            

        
        

        questionDisplay.SetActive(false);
        endGameDisplay.SetActive(true);


    }

    public void SubmitButton()
    {
        HighScorePlayerData newScore = new HighScorePlayerData();
        newScore.playerName = playerNameText.text;
        newScore.score = playerScore;

        if (dataController.nonBrokenList.Count >= 10 && playerScore > dataController.nonBrokenList[9].score)
        {
            dataController.nonBrokenList.RemoveAt(9);
            dataController.nonBrokenList.Add(newScore);

            dataController.nonBrokenList.Sort((p1, p2) => p2.score.CompareTo(p2.score));

            /*Array.Sort(dataController.allHighScoreData.HighScorePlayerData, delegate (HighScorePlayerData player1, HighScorePlayerData player2)
            {
                return (player2.score).CompareTo((player1.score));
            });*/

        } else if (dataController.nonBrokenList.Count < 10)
        {

            dataController.nonBrokenList.Add(newScore);
            
            /*Array.Sort(dataController.allHighScoreData.HighScorePlayerData, delegate (HighScorePlayerData player1, HighScorePlayerData player2)
            {


                return (player2.score).CompareTo((player1.score));
            });*/

            dataController.nonBrokenList.Sort((p1, p2) => p2.score.CompareTo(p2.score));
        }
        //dataController.allHighScoreData

        dataController.SubmitNewPlayerScore(playerNameText.text, playerScore);
        EndRound();

    }

    public void StartOver()
    {
        SceneManager.LoadScene("MenuScreen");
    }
	
	// Update is called once per frame
	void Update () {

        if (dataController == null)
            dataController = FindObjectOfType<DataController>();

        if (isRoundActive)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTime();

            if(timeRemaining <= 0)
            {
                EndRound();
            }
        }
        
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MenuScreen");
    }

    private void UpdateTime()
    {
        timeText.text = "Time: " + Mathf.Round(timeRemaining).ToString();
    }
}
