using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(GameManager))]
public class StandbyStateProcess : MonoBehaviour
{
    private GameManager _gameManager;
    private TextMeshProUGUI _opponentInfoText;//開始前の相手の情報
    public static string OpponentInfo { get; set; } = "敵をすべて切り倒せ!";//相手の情報
    public AudioSource audioSource;//オーディオソース
    public AudioClip whistle;//ホイッスル音
    public AudioClip katana;//開戦の刀音
    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GetComponent<GameManager>();
        _opponentInfoText = GameObject.FindGameObjectWithTag("StartSign").GetComponent<TextMeshProUGUI>();
        _opponentInfoText.text = OpponentInfo;
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
            //ホイッスル音
            await Task.Delay(1000);
            audioSource.PlayOneShot(katana);
            await Task.Delay(6000);
            audioSource.PlayOneShot(whistle);
            await Task.Delay(1000);
            //PlayingStateへ移動
            _gameManager.StateChangeSignal(GameState.Playing);
            await StandardFade.FadeOut(_opponentInfoText, 1);
        }
    }

}
