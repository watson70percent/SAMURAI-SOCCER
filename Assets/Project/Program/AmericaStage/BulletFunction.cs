using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 弾丸との衝突の処理,自滅処理
/// </summary>
public class BulletFunction : MonoBehaviour
{
    private GameManager _gameManager;

    private AudioSource _audioSource;//弾丸についてるAudioSource

    public string ResultSceneName = "Result";//リザルトシーン名

    void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _audioSource = gameObject.GetComponent<AudioSource>();
        StartCoroutine(DestroyOneself());
    }

    private void Update()
    {
        //ゴールしたり、リセットしたらこの弾丸は削除
        if (_gameManager.CurrentGameState == GameState.Standby)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //プレイヤ―との衝突処理
        if (_gameManager.CurrentGameState != GameState.Finish)
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
    }

    /// <summary>
    /// 圧死リザルト用の処理
    /// </summary>
    /// <param name="next"></param>
    /// <param name="mode"></param>
    void GameSceneLoaded(Scene next, LoadSceneMode mode)
    {
        ResultManager resultManager = GameObject.Find("ResultManager").GetComponent<ResultManager>();
        resultManager.SetResult(Result.Lose, "睡眠弾に撃たれた！");

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

    /// <summary>
    /// 時間がたつと勝手に消滅する
    /// </summary>
    /// <returns></returns>
    IEnumerator DestroyOneself()
    {
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }
}
