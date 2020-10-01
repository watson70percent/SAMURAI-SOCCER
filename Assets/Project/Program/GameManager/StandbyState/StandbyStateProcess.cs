using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GameManager))]
public class StandbyStateProcess : MonoBehaviour
{
    private GameManager _gameManager;
    private Text _startSignText;
    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GetComponent<GameManager>();
        _startSignText = GameObject.FindGameObjectWithTag("StartSign").GetComponent<Text>();
        //StandbyStateに処理を追加
        _gameManager.StateChange += StandbyProcessContent;
    }

    /// <summary>
    /// 試合開始の演出(StartSignTextのフェードとStateの移動)を行う
    /// </summary>
    /// <param name="stateChangedArg"></param>
    public async void StandbyProcessContent(StateChangedArg stateChangedArg)
    {
        if (stateChangedArg.gameState == GameState.Standby)
        {
            await StandardFade.FadeIn(_startSignText, 2);
            //ホイッスル音
            await Task.Delay(1000);
            //PlayingStateへ移動
            _gameManager.StateChangeSignal(GameState.Playing);
            await StandardFade.FadeOut(_startSignText, 1);
        }
    }
}
