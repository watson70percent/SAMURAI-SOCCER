using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IceMilkTea.Core;

public class StageSelectState : ImtStateMachine<SelectStateManager>.State
{
    // 状態へ突入時の処理はこのEnterで行う
    protected override void Enter()
    {

    }

    // 状態の更新はこのUpdateで行う
    protected override void Update()
    {
        Context.stageSelectCamera.StageSelectStateUpdate();
    }

    // 状態から脱出する時の処理はこのExitで行う
    protected override void Exit()
    {

    }
}
