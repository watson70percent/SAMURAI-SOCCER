using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseButton : MonoBehaviour
{
    public GameObject pausePanel;
    GameManager gameManager;

    GameState state = GameState.Reset;
    void SwitchState(StateChangedArg a)
    {
        state = a.gameState;
    }


    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.StateChange += SwitchState;
    }

    public void OnClick()
    {
        if (state != GameState.Playing) { return; }

        pausePanel.SetActive(true);
        gameManager.StateChangeSignal(GameState.Pause);
        this.gameObject.SetActive(false);
    }

    public void ContinueButton()
    {
        gameManager.StateChangeSignal(GameState.Playing);
        pausePanel.SetActive(false);
        this.gameObject.SetActive(true);
    }

    public void RestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MenuBackButton()
    {
        SceneManager.LoadScene("StageSelect");
    }

}
