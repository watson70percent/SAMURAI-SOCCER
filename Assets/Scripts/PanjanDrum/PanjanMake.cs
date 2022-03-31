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
    Vector3 respone = new Vector3(30, 2, 95);
    private string ResultSceneName = "Result";
    Quaternion quaternion = new Quaternion(0,1,0,0);

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
            Instantiate(panjan,respone,quaternion);
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
