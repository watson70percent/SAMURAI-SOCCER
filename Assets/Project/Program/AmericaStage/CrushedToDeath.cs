using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CrushedToDeath : MonoBehaviour
{
    private GameManager _gameManager;
    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_gameManager.CurrentGameState != GameState.Finish)
        {
            if (other.gameObject.tag == "Player")
            {
                //SceneManagerのイベントに圧死リザルト処理を追加
                SceneManager.sceneLoaded += GameSceneLoaded;
                _gameManager.StateChangeSignal(GameState.Finish);
            }
        }
    }

    //圧死リザルト用の処理
    void GameSceneLoaded(Scene next, LoadSceneMode mode)
    {
        ResultManager resultManager = GameObject.Find("ResultManager").GetComponent<ResultManager>();
        resultManager.SetResult(Result.Lose, "つぶされてしまった!");

        SceneManager.sceneLoaded -= GameSceneLoaded;
    }
}
