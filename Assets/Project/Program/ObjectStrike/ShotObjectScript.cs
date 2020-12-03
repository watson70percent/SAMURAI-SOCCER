using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShotObjectScript : MonoBehaviour
{
    private string ResultSceneName = "Result";
    float velocity = 30;//速さ
	//float size;
    float movedLength;//動いた距離
    float groundWidth=120;//グラウンドの幅
    GameManager gameManager;

    //   public ShotObjectScript()
    //{

    //}

    // Start is called before the first frame update
    void Start()
    {
        //移動距離の初期化
        movedLength = 0;
        //gameManager取得
        gameManager = this.gameObject.transform.root.GetComponent<StrikeObjectScript>().gameManager;
    }

    // Update is called once per frame
    void Update()
    {
        //一定スピードで動かす
		gameObject.transform.position += transform.forward * Time.deltaTime * velocity;
        movedLength+= Time.deltaTime * velocity;

        //グラウンドを通り過ぎたら消す
        if ((movedLength) > (groundWidth + 2))
        {
            Destroy(this.gameObject);
        }

    }

	private void OnCollisionEnter(Collision collision)
	{

        //衝突が人間だったら吹っ飛ばす

            Rigidbody rb = collision.transform.GetComponent<Rigidbody>();
            Vector3 v3 = transform.forward * velocity * velocity;
            v3.y += velocity * 5;
            rb.AddForce(v3);
        //衝突がプレイヤーだったらゲームオーバー
        if (collision.gameObject.tag == "Player")
        {
            SceneManager.sceneLoaded += GameSceneLoaded;
            gameManager.StateChangeSignal(GameState.Finish);
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



    //交通事故リザルト用の処理
    void GameSceneLoaded(Scene next, LoadSceneMode mode)
    {
        ResultManager resultManager = GameObject.Find("ResultManager").GetComponent<ResultManager>();
        resultManager.SetResult(Result.Lose, "交通事故");

        SceneManager.sceneLoaded -= GameSceneLoaded;
    }

}
