using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(GameManager))]
public class FinishProcess : MonoBehaviour
{
    public string ResultSceneName = "ResultScene";
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
        //試合終了みたいなホイッスル音とテキストの表示
        //リザルトへのシーン遷移
        SceneManager.LoadScene(ResultSceneName);
    }
}
