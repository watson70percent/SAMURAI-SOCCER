using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateDebuger : MonoBehaviour
{
    public GameState gmState;
    public string gameManagerName = "GameManager";
    private GameManager _gameManager;
    private void Start()
    {
        if (!GameObject.Find(gameManagerName))
        {
            Debug.LogError("Scene上にGameManagerオブジェクトが存在しません。もし、GameManagerスクリプトがGameManegerという名前以外のオブジェクトについている場合はこのスクリプトのgameManagerNameを変更してさい。");
            return;
        }
        _gameManager = GameObject.Find(gameManagerName).GetComponent<GameManager>();
    }
    /// <summary>
    /// 現在のゲームマネージャーのStateを変更する(Debug)
    /// </summary>
    public void CurrentStateChanger()
    {
        _gameManager.CurrentStateChanger(gmState);
    }
    /// <summary>
    /// ゲームマネージャーのStateを確認する
    /// </summary>
    public void CurrentStateResercher()
    {
        _gameManager.CurrentStateResercher();
    }
}
