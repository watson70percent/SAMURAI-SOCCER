using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseBackScript : MonoBehaviour
{

    public bool playing;//試合中のフラグ
    bool end = false;//試合終了のフラグ
    public GameManager gameManager;
    JudgeGameEnd judgeGameEnd;//ゲーム終了判定クラス
    Button button;


    public void PauseBackButton(StateChangedArg stateChangedArg)
    {
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

    private void Awake()
    {
        gameManager.StateChange += PauseBackButton;
        judgeGameEnd = GetComponent<JudgeGameEnd>();
    }

    public void OnClick()
    {
        gameManager.StateChangeSignal(GameState.Playing);

    }

    public void Pause()//停止する
    {
        playing = false;
        gameObject.SetActive(true);
    }

    public void Playing()//停止解除する
    {
        playing = true;
        gameObject.SetActive(false);
    }

    public void Reset()
    {

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


        
    }
}
