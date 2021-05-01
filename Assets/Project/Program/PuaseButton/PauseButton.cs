using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseButton : MonoBehaviour
{
    public GameObject pausePanel;
    GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void OnClick()
    {
        pausePanel.SetActive(true);
        gameManager.StateChangeSignal(GameState.Pause);
    }

    public void ContinueButton()
    {
        gameManager.StateChangeSignal(GameState.Playing);
        pausePanel.SetActive(false);
    }

    public void RestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MenuBackButton()
    {

    }

}
