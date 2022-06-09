﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace SamuraiSoccer.SoccerGame
{
    public class Penalty : MonoBehaviour
    {

        public GameObject[] yellowCard = new GameObject[2];
        int penaltycount = 0;
        GameManager gameManager;
        public GameObject gameOverPanel;

        GameState state = GameState.Reset;
        public AudioSource refereeAudio;
        public AudioClip yellowAudioClip, redAudioClip;
        public GoalText goalCanvas;

        // Start is called before the first frame update

        private void Reset(StateChangedArg a)
        {
            state = a.gameState;
            if (a.gameState == GameState.Reset)
            {
                penaltycount = 0;
                yellowCard[0].SetActive(false);
                yellowCard[1].SetActive(false);
            }

        }
        void Start()
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            gameManager.StateChange += Reset;
        }

        public void YellowCard()
        {
            if (state != GameState.Playing) { return; }
            goalCanvas.TextContent = "反則";
            yellowCard[penaltycount].SetActive(true);
            penaltycount++;
            if (penaltycount == 2)
            {
                refereeAudio.PlayOneShot(redAudioClip); //ここがバグの原因
                SceneManager.sceneLoaded += GameSceneLoaded;
                gameManager.StateChangeSignal(GameState.Finish);
                Instantiate(gameOverPanel);

            }
            else
            {
                refereeAudio.PlayOneShot(yellowAudioClip);
            }
        }

        void GameSceneLoaded(Scene next, LoadSceneMode mode)
        {
            ResultManager resultManager = GameObject.Find("ResultManager").GetComponent<ResultManager>();
            resultManager.SetResult(Result.Lose, "反則負け!");
            SceneManager.sceneLoaded -= GameSceneLoaded;
        }
    }
}
