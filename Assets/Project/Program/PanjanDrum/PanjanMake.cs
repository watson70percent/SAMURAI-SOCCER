using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PanjanMake : MonoBehaviour
{
    GameManager gameManager;
    public GameObject panjan;
    bool panjanExist;
    bool isEnd;
    Vector3 respone = new Vector3(30, 2.0f, 110);
    private string ResultSceneName = "Result";

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(gameManager.CurrentGameState == GameState.Playing && !panjanExist)
        {
            Instantiate(panjan,respone,Quaternion.identity);
            panjanExist = true;
        }
        if(gameManager.CurrentGameState == GameState.Standby)
        {
            panjanExist = false;
        }
    }

    public void Burn()
    {
        if (!isEnd)
        {
            //SceneManagerのイベントに勝利リザルト処理を追加
            isEnd = true;
            //GetComponent<AudioSource>().Play();
            SceneManager.sceneLoaded += GameSceneLoaded;
            gameManager.StateChangeSignal(GameState.Finish);
            Time.timeScale = 0.2f;

            //リザルトへのシーン遷移
            StartCoroutine(GoResult());

        }
    }

    //スロー演出からのシーン遷移
    IEnumerator GoResult()
    {
        yield return new WaitForSeconds(0.8f);
        Time.timeScale = 1;
        SceneManager.LoadScene(ResultSceneName);
    }

    //パンジャンリザルト用の処理
    void GameSceneLoaded(Scene next, LoadSceneMode mode)
    {
        ResultManager resultManager = GameObject.Find("ResultManager").GetComponent<ResultManager>();
        resultManager.SetResult(Result.Lose, "爆破");

        SceneManager.sceneLoaded -= GameSceneLoaded;
    }
}
