using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TimerScript : MonoBehaviour
{
    private string ResultSceneName = "Result";
    [SerializeField]
    private AudioClip finishSound;
    [SerializeField]
    private AudioSource audioSource;
    public bool playing;//試合中のフラグ
    bool end = false;//試合終了のフラグ
    public float elapsedTime;//経過時間
    public float limitTime;//制限時間

    public GameObject displayText;//時間を表示させるもの
    public Text timeText;//時間を表示させるもの



    public GameManager gameManager;

    public void Timer(StateChangedArg stateChangedArg) {
        switch (stateChangedArg.gameState)
        {
            case GameState.Pause:
                Pause();
                break;
            case GameState.Playing:
                Playing();
                break;
            case GameState.Reset:
                Reset();
                break;
        }
    }

    public void Reset()
    {
        elapsedTime = 0;
    }

    public bool isTimeUp()//タイムアップか否かを返す  true:タイムアップ false:まだ
    {
        return end;
    }

    public void setTimer(int sec)//時間をセットする  (int 分　int 秒)
    {
        limitTime = sec;
        //end = false;
    }

    public void Pause()//停止する
    {
        playing = false;
    }

    public void Playing()//停止解除する
    {
        playing = true;
    }

    private void Awake()
    {
        gameManager.StateChange += Timer;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playing && !end)
        {
            if (limitTime > elapsedTime)
            {
                elapsedTime += Time.deltaTime;
                //Debug.Log(elapsedTime);
                int displayTime = (int)(limitTime - elapsedTime);
                timeText.text = ((int)(displayTime / 60)).ToString("0") + ":" + Mathf.CeilToInt(displayTime % 60).ToString("00");

            }
            else
            {
                playing = false;
                end = true;
                //FinishStateに移動していないなら(移動したらもうこの辺の処理は止める)
                if (gameManager.CurrentGameState != GameState.Finish)
                {
                    //SceneManagerのイベントにTimeUpリザルト処理を追加
                    SceneManager.sceneLoaded += GameSceneLoaded;
                    gameManager.StateChangeSignal(GameState.Finish);
                    audioSource.clip = finishSound;
                    audioSource.Play();
                    //リザルトへのシーン遷移
                    StartCoroutine(GoResult());
                }
            }
        }
       
    }

    IEnumerator GoResult()
    {
        yield return new WaitForSeconds(1);
        Time.timeScale = 1;
        SceneManager.LoadScene(ResultSceneName);
    }

    //TimeUpリザルト用の処理
    void GameSceneLoaded(Scene next, LoadSceneMode mode)
    {
        ResultManager resultManager = GameObject.Find("ResultManager").GetComponent<ResultManager>();
        resultManager.SetResult(Result.Lose,"時間切れ!");

        SceneManager.sceneLoaded -= GameSceneLoaded;
    }
}
