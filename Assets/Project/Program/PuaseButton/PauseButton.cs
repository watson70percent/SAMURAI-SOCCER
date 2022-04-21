using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseButton : MonoBehaviour
{
    public GameObject pausePanel;
    GameManager gameManager;
    

    public bool goalFlag = false;

    GameState state = GameState.Reset;
    void SwitchState(StateChangedArg a)
    {
        state = a.gameState;
        print(this.gameObject.name);
        if (state == GameState.Playing)
        {
            goalFlag = false;
        }
    }
    int countTest=0;

    public void Goal(object sender, GoalEventArgs e)
    {
        goalFlag = true;
        
    }



    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.StateChange += SwitchState;
        var ballControler = GameObject.FindGameObjectWithTag("Ball").GetComponent<BallControler>();
        ballControler.Goal += Goal;

    }

    public void OnClick()
    {
        if (state != GameState.Playing || goalFlag) { return; }

        pausePanel.SetActive(true);
        gameManager.StateChangeSignal(GameState.Pause);
        this.gameObject.SetActive(false);
        
        Time.timeScale = 1e-10f;
    }

    public void ContinueButton()
    {
        Time.timeScale = 1;
        gameManager.StateChangeSignal(GameState.Playing);
        pausePanel.SetActive(false);
        this.gameObject.SetActive(true);
    }

    public void RestartButton()
    {
        Time.timeScale = 1;
        var data = GameObject.Find("DefaultStage").GetComponent<StageDataHolder>();
        data.SetStageData(StageDataHolder.NowStageData);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MenuBackButton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("StageSelect");
    }

}
