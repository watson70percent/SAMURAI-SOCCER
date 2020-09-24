using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Penalty : MonoBehaviour
{

    public GameObject[] yellowCard=new GameObject[2];
    int penaltycount = 0;
    GameManager gameManager;
    // Start is called before the first frame update

    private void Reset(StateChangedArg a)
    {
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
        yellowCard[penaltycount].SetActive(true);
        penaltycount++;
        if (penaltycount == 2)
        {
            SceneManager.sceneLoaded += GameSceneLoaded;
            gameManager.StateChangeSignal(GameState.Finish);
            
        }
    }

    void GameSceneLoaded(Scene next, LoadSceneMode mode)
    {
        ResultManager resultManager = GameObject.Find("ResultManager").GetComponent<ResultManager>();
        resultManager.resultState = ResultState.Violation;

        SceneManager.sceneLoaded -= GameSceneLoaded;
    }
}
