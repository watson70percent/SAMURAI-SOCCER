using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShotObjectScript : MonoBehaviour
{
    private string ResultSceneName = "Result";

    [SerializeField]float velocity = 30;//速さ
	//float size;
    float movedLength;//動いた距離
    float groundWidth=120;//グラウンドの幅
    GameManager gameManager;
    bool isEnd;

    //   public ShotObjectScript()
    //{

    //}

    // Start is called before the first frame update
    void Start()
    {
        //移動距離の初期化
        movedLength = 0;
        //gameManager取得
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
            //一定スピードで動かす
            gameObject.transform.position += transform.forward * Time.deltaTime * velocity;
            movedLength += Time.deltaTime * velocity;

            //グラウンドを通り過ぎたら消す
            if ((movedLength) > (groundWidth + 2) && !isEnd)
            {
                Destroy(this.gameObject);
            }
            if(gameManager.CurrentGameState == GameState.Standby) Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(gameManager.CurrentGameState == GameState.Playing)
        {
            //    //衝突がプレイヤーだったらゲームオーバー
            if (other.gameObject.tag == "Player")
            {
                GetComponent<AudioSource>().Play();
                other.transform.GetComponent<Rigidbody>();
                //SceneManagerのイベントに勝利リザルト処理を追加
                isEnd = true;
                GetComponent<AudioSource>().Play();
                SceneManager.sceneLoaded += GameSceneLoaded;
                gameManager.StateChangeSignal(GameState.Finish);
                Time.timeScale = 0.2f;

                //リザルトへのシーン遷移
                StartCoroutine(BlowAway(other.gameObject));
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

    IEnumerator BlowAway(GameObject player)
    {
        Debug.Log("blowAwayObject = " + player.tag);
        float velocity0 = 300;
        float ang = 20*Mathf.Deg2Rad;
        Vector3 rotateVec = new Vector3(4, 7, 5);
        for (int i = 0; i < 100; i++)
        {
            Vector3 pos = player.transform.position;

            pos.x -= velocity0 * Mathf.Cos(ang) * Time.deltaTime;
            pos.y += velocity0 * Mathf.Sin(ang) * Time.deltaTime;
            player.transform.position = pos;
            player.transform.Rotate(rotateVec * velocity *velocity* Time.deltaTime);
            yield return null;
        }
        
    }

    //交通事故リザルト用の処理
    void GameSceneLoaded(Scene next, LoadSceneMode mode)
    {
        ResultManager resultManager = GameObject.Find("ResultManager").GetComponent<ResultManager>();
        resultManager.SetResult(Result.Lose, "交通事故");

        SceneManager.sceneLoaded -= GameSceneLoaded;
    }

}
