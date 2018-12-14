using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameEvaluatorScript : MonoBehaviour {


    public GameObject[] playerList;
    bool endGame = false;
    int player1Score;
    int player2Score;

    //Text box to show the winner details.
    public Text winnerText;

    public GameObject victoryPanel;
	// Use this for initialization
	void Start () {
        playerList = GameObject.FindGameObjectsWithTag("Player");
        victoryPanel.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        DecesionMaker();
	}

    void DecesionMaker()
    {

        endGame = GetPlayerTimes();
        if (endGame)
        {
            victoryPanel.SetActive(true);
            if(player1Score == player2Score)
            {
                winnerText.text = "Draw Match!!!";
            }
            else if (player1Score>player2Score)
            {
                winnerText.text = "Player 1 Wins!!!";
            }
            else
            {
                winnerText.text = "Player 2 Wins!!!";
            }

        }
        
    }

    //Check wether the Game is over for both the players and then get their scores.
    bool GetPlayerTimes()
    {
        if(playerList[0].GetComponent<PlayerControllerScript>().timeLeft <= 0 && playerList[1].GetComponent<PlayerControllerScript>().timeLeft <= 0)
        {
            player1Score = playerList[0].GetComponent<PlayerControllerScript>().playerScore;
            player2Score = playerList[1].GetComponent<PlayerControllerScript>().playerScore;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Level1");
    }
}
