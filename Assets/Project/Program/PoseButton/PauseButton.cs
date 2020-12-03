using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButton : MonoBehaviour
{
    GameState state;
    public GameObject pausePannel;
    void SwitchState(StateChangedArg a)
    {
        state = a.gameState;
    }
    GameManager gameManager;
    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>(); gameManager.StateChange += SwitchState;
        state = gameManager.CurrentGameState;
    }

    public void OnClick()
    {
        if (state == GameState.Playing)
        {
            gameManager.StateChangeSignal(GameState.Pause);
            pausePannel.SetActive(true);
        }
    }
}
