using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PanjanExplode : MonoBehaviour
{
    GameManager gameManager;
    bool isEnd;
    private string ResultSceneName = "Result";
    //GameObject PrefabFire;
    // Start is called before the first frame update
    void Start()
    {
        //PrefabFire = Resources.Load<GameObject>("Assets/Design_/Sugi'sTemp/Ninja/Sources/Fire");
        //gameManager取得
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        //transform=new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100));
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (gameManager.CurrentGameState == GameState.Playing)
        {
            //    //衝突がプレイヤーだったらゲームオーバー
            if (other.gameObject.tag == "Player")
            {
                other.transform.Find("Fire").gameObject.SetActive(true);
                other.transform.GetComponent<Rigidbody>();
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
