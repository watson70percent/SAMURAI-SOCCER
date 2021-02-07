using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 自由の女神との衝突の処理
/// </summary>
public class CollisionWithStatue : MonoBehaviour
{
    private GameManager _gameManager;

    private RiseStatue _riseStatue;

    private AudioSource _audioSource;//自由の女神についてるAudioSource

    public string ResultSceneName = "Result";//リザルトシーン名

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _riseStatue = gameObject.GetComponentInParent<RiseStatue>();
        _audioSource = gameObject.GetComponentInParent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //プレイヤ―との衝突処理
        if (_gameManager.CurrentGameState != GameState.Finish && _riseStatue.CurrentStatueMode == StatueMode.FallDown)
        {
            if (other.gameObject.tag == "Player")
            {
                //死亡音の再生
                _audioSource.Play();
                //SceneManagerのイベントに圧死リザルト処理を追加
                SceneManager.sceneLoaded += GameSceneLoaded;
                _gameManager.StateChangeSignal(GameState.Finish);
                Time.timeScale = 0.2f;
                StartCoroutine(GoResult());
            }
        }
        //ボールとの衝突処理
        if(other.gameObject.tag == "Ball")
        {
            StartCoroutine(ResetBall(other.gameObject));
        }
    }

    /// <summary>
    /// 圧死リザルト用の処理
    /// </summary>
    /// <param name="next"></param>
    /// <param name="mode"></param>
    void GameSceneLoaded(Scene next, LoadSceneMode mode)
    {
        ResultManager resultManager = GameObject.Find("ResultManager").GetComponent<ResultManager>();
        resultManager.SetResult(Result.Lose, "つぶされてしまった!");

        SceneManager.sceneLoaded -= GameSceneLoaded;
    }

    /// <summary>
    /// リザルトへ移動するためのコルーチン
    /// </summary>
    /// <returns></returns>
    IEnumerator GoResult()
    {
        yield return new WaitForSeconds(1f);
        Time.timeScale = 1;
        SceneManager.LoadScene(ResultSceneName);
    }

    IEnumerator ResetBall(GameObject Ball)
    {
        yield return new WaitForSeconds(1f);
        if (Ball.transform.position.y < 0)
        {
            Ball.transform.position = new Vector3(Ball.transform.position.x, 5f, Ball.transform.position.z);
        }
    }
}
