using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(GameManager))]
public class FinishProcess : MonoBehaviour
{
    public string ResultSceneName = "ResultScene";
    [SerializeField]
    private AudioClip finishSound;
    [SerializeField]
    private AudioSource audioSource;
    private GameManager _gameManager;
    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GetComponent<GameManager>();
        //FinishStateに処理を追加
        _gameManager.StateChange += FinishProcessContent;
    }

    public void FinishProcessContent(StateChangedArg stateChangedArg)
    {
        if (stateChangedArg.gameState == GameState.Finish)
        {
            //試合終了みたいなホイッスル音とテキストの表示
            
        }

    }

    IEnumerator GoResult()
    {
        yield return new WaitForSeconds(1);
        Time.timeScale = 1;
        SceneManager.LoadScene(ResultSceneName);
    }
}
