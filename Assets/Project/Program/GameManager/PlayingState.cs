using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IceMilkTea.Core;

public class PlayingState : ImtStateMachine<GameManager>.State
{
    JudgeGameEnd judgeGameEnd;//ゲーム終了判定クラス
    // 状態へ突入時の処理はこのEnterで行う
    protected override void Enter()
    {
        Context.StartStateChangeEvent(GameState.Playing);
        judgeGameEnd = Context.GetComponent<JudgeGameEnd>();
    }

    // 状態の更新はこのUpdateで行う
    protected override void Update()
    {
        //ゲームエンドフラグで試合終了
        if (judgeGameEnd.EndFlag)
        {
            Context.gameObject.GetComponent<GameManager>().StateChangeSignal(GameState.Finish);
        }
    }

    // 状態から脱出する時の処理はこのExitで行う
    protected override void Exit()
    {
    }
}
