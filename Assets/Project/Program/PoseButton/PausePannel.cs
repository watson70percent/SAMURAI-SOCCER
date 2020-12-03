using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausePannel : MonoBehaviour
{
    GameManager gameManager;
    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }


    public void ReturnGame()
    {
        gameManager.StateChangeSignal(GameState.Playing);
        this.gameObject.SetActive(false);
    }

    public void Replay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Suspention()
    {

    }
}
