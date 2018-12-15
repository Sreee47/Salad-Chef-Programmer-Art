using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
public class GameEvaluatorScript : MonoBehaviour {


    public GameObject[] playerList;


    //To store the power pickups prefabs.
    public GameObject[] pickUpsList;

    bool endGame = false;
    bool notComplete = true;
    //for storing player scores
    int player1Score;
    int player2Score;

    //Text box to show the winner details.
    public Text winnerText;

    //Text box to show top 10 high scores.
    public Text highscores;

    public GameObject victoryPanel;
	// Use this for initialization
	void Start () {
        playerList = GameObject.FindGameObjectsWithTag("Player");
        victoryPanel.SetActive(false);

        //Stores and accesses player preferences between game sessions.
        // For storing top 10 highscores.

        //checks wether the key is available for session storage.
        if (!PlayerPrefs.HasKey("HighScore"))
        {
            var scoreArray = new int[10];
            for(int i = 0; i < 10; i++)
            {
                scoreArray[i] = -100;
            }

            //Storing Datas in array. 
            //PlayerPrefsX script is added in the thirdparty/scripts folder. 
            PlayerPrefsX.SetIntArray("HighScore",scoreArray);
        }
	}
	
	// Update is called once per frame
	void Update () {
        DecesionMaker();
	}

    //To spawn random pickups at random position and then destroys after 5 seconds
    
    public void SpawnPickUp()
    {        
        //Finds a random positon
        Vector3 powerUpspawnPoints = pickUpsList[UnityEngine.Random.Range(0, pickUpsList.Length)].transform.position;

        //For selecting a random pickup.
        int pickUpIndex = UnityEngine.Random.Range(0,pickUpsList.Length);
        GameObject pickUp = Instantiate(pickUpsList[pickUpIndex],powerUpspawnPoints,Quaternion.identity);
        Destroy(pickUp,10);
        
    }


    //For deciding the winner.
    void DecesionMaker()
    {
        if (notComplete)
        {
            endGame = GetPlayerTimes();
            if (endGame)
            {
                notComplete = false;
                endGame = false;
                victoryPanel.SetActive(true);
                if (player1Score == player2Score)
                {
                    winnerText.text = "Draw Match!!!";
                    AddHighScore(player1Score);
                }
                else if (player1Score > player2Score)
                {
                    winnerText.text = "Player 1 Wins!!!";
                    AddHighScore(player1Score);
                }
                else
                {
                    winnerText.text = "Player 2 Wins!!!";
                    AddHighScore(player2Score);
                }

            }
        }
        
        
    }

    //Adding the highest score to game session.
    void AddHighScore(int score)
    {
        int[] currentHighScores = PlayerPrefsX.GetIntArray("HighScore");
       if (currentHighScores[currentHighScores.Length - 1] < score)
        {
            currentHighScores[currentHighScores.Length-1] = score;
            Array.Sort(currentHighScores);
            PlayerPrefsX.SetIntArray("HighScore",currentHighScores);
        }
        highscores.text += ": ";
        for(int i = 0; i < currentHighScores.Length; i++)
        {
            highscores.text += currentHighScores[i].ToString() +",";

        }


    }

    //Check wether the Game is over for both the players and then get their scores.
    bool GetPlayerTimes()
    {
        if(playerList[0].GetComponent<PlayerControllerScript>().timeLeft <= 0 && playerList[1].GetComponent<PlayerControllerScript>().timeLeft <= 0)
        {
            player2Score = playerList[0].GetComponent<PlayerControllerScript>().playerScore;
            player1Score = playerList[1].GetComponent<PlayerControllerScript>().playerScore;
            return true;
        }
        else
        {
            return false;
        }
    }


    //Restart the game.
    public void RestartGame()
    {
        SceneManager.LoadScene("Level1");
    }
}
