using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 弾丸との衝突の処理,自滅処理
/// </summary>
public class BulletFunction : MonoBehaviour
{
    private bool _isActive = true;//true 稼働中
    private Vector3 _tmpvelocity = Vector3.zero; //一時的に速度を保持する

    private GameManager _gameManager;
    private BallControler _ball;
    private Rigidbody _rb;

    private AudioSource _audioSource;//弾丸についてるAudioSource

    public string ResultSceneName = "Result";//リザルトシーン名

    void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _ball = GameObject.FindGameObjectWithTag("Ball").GetComponent<BallControler>();
        _rb = GetComponent<Rigidbody>();
        _audioSource = gameObject.GetComponent<AudioSource>();
        _ball.Goal += DisableFunction;
        StartCoroutine(DestroyOneself());
    }

    private void Update()
    {
        //ゴールしたり、リセットしたらこの弾丸は削除
        if (_gameManager.CurrentGameState == GameState.Standby)
        {
            Destroy(gameObject);
        }
        else if (_gameManager.CurrentGameState == GameState.Pause)
        {
            if (_rb.velocity != Vector3.zero)
            {
                _tmpvelocity = _rb.velocity;
                _rb.velocity = Vector3.zero;
            }
        }
        else if (_gameManager.CurrentGameState == GameState.Playing)
        {
            if (_rb.velocity == Vector3.zero)
            {
                _rb.velocity = _tmpvelocity;
                _tmpvelocity = Vector3.zero;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //プレイヤ―との衝突処理
        if (_gameManager.CurrentGameState != GameState.Finish && _isActive)
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
        resultManager.SetResult(Result.Lose, "砲弾に撃たれた！");

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
    /// ゴールが入ったら衝突時の機能を停止する
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="goalEventArgs"></param>
    public void DisableFunction(object sender, GoalEventArgs goalEventArgs)
    {
        _isActive = false;
        _ball.Goal -= DisableFunction;
    }

    /// <summary>
    /// 時間がたつと勝手に消滅する
    /// </summary>
    /// <returns></returns>
    IEnumerator DestroyOneself()
    {
        yield return new WaitForSeconds(20f);
        Destroy(gameObject);
    }
}
