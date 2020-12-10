using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(GameManager),typeof(EasyCPUManager))]
public class JudgeGameEnd : MonoBehaviour
{
    private GameManager _gameManager;
    private EasyCPUManager _easyCPUManager;

    public string ResultSceneName = "Result";//リザルトシーン名
    [SerializeField]
    private AudioClip finishSound;
    [SerializeField]
    private AudioSource audioSource;

    private void Start()
    {
        _gameManager = GetComponent<GameManager>();
        _easyCPUManager = GetComponent<EasyCPUManager>();
    }

    private void Update()
    {
        //FinishStateに移動していないなら(移動したらもうこの辺の処理は止める)
        if (_gameManager.CurrentGameState!= GameState.Finish)
        {
            //敵CPUの数が0以下なら勝利リザルト用の処理をSceneManagerに追加し、StateをFinishに移動
            if (_easyCPUManager.OpponentMemberCount <= 0)
            {
                //SceneManagerのイベントに勝利リザルト処理を追加
                SceneManager.sceneLoaded += GameSceneLoaded;
                _gameManager.StateChangeSignal(GameState.Finish);
                Time.timeScale = 0.2f;                
                audioSource.PlayOneShot(finishSound);
                StartCoroutine(GoResult());
            }
        }

    }

    //勝利リザルト用の処理
    void GameSceneLoaded(Scene next, LoadSceneMode mode)
    {
        ResultManager resultManager = GameObject.Find("ResultManager").GetComponent<ResultManager>();
        resultManager.SetResult(Result.Win,"敵は全滅じゃ!");

        SceneManager.sceneLoaded -= GameSceneLoaded;
    }

    IEnumerator GoResult()
    {
        yield return new WaitForSeconds(1);
        Time.timeScale = 1;
        SceneManager.LoadScene(ResultSceneName);
    }
}
