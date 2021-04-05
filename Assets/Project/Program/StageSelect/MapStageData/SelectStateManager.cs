using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IceMilkTea.Core;

/// <summary>
/// ステージ選択画面の状態
/// </summary>
public enum SelectState
{
    WorldSelect,
    StageSelect,
    StagePreview,
}
[RequireComponent(typeof(StageSelectCamera))]
public class SelectStateManager : MonoBehaviour
{
    //このクラスのステートマシーン
    private ImtStateMachine<SelectStateManager> _imtStateMachine;

    /// <summary>
    /// 現在のステージセレクト画面の状態
    /// </summary>
    public SelectState CurrentSelectState
    {
        get;
        private set;
    } = SelectState.WorldSelect;

    //以下各ステートで使用するクラス
    [System.NonSerialized]
    public StageSelectCamera stageSelectCamera;

    private void Awake()
    {
        _imtStateMachine = new ImtStateMachine<SelectStateManager>(this);
        //Stateの移動する方向とIdを関連づけ
        _imtStateMachine.AddAnyTransition<WorldSelectState>((int)SelectState.WorldSelect);
        _imtStateMachine.AddAnyTransition<StageSelectState>((int)SelectState.StageSelect);
        _imtStateMachine.AddAnyTransition<StagePreviewState>((int)SelectState.StagePreview);

        _imtStateMachine.SetStartState<WorldSelectState>();
    }

    private void Start()
    {
        stageSelectCamera = GetComponent<StageSelectCamera>();
    }
    private void Update()
    {
        _imtStateMachine.Update();
    }

    /// <summary>
    /// 任意のStateに変更するIdをステートマシンに送る
    /// </summary>
    public void StateChangeSignal(SelectState selectState)
    {
        _imtStateMachine.SendEvent((int)selectState);
        CurrentSelectState = selectState;
        Debug.Log("Stateを" + selectState + "に移動しました");
    }
}
