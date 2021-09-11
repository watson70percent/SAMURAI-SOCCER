using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Penalty : MonoBehaviour
{

    public GameObject[] yellowCard=new GameObject[2];
    int penaltycount = 0;
    GameManager gameManager;
    public GameObject gameOverPanel;

    GameState state = GameState.Reset;
    public AudioSource refereeAudio;
    public AudioClip yellowAudioClip, redAudioClip;

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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void YellowCard()
    {
        if (state != GameState.Playing) { return; }
        yellowCard[penaltycount].SetActive(true);
        penaltycount++;
        if (penaltycount == 2)
        {
            print("A");
            refereeAudio.PlayOneShot(redAudioClip); //ここがバグの原因
            print("B");
            SceneManager.sceneLoaded += GameSceneLoaded;
            print("C");
            gameManager.StateChangeSignal(GameState.Finish);
            print("D");
            Instantiate(gameOverPanel);
            print("E");
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
