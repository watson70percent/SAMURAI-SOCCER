using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using IceMilkTea.Core;

/// <summary>
/// メインゲームの状態
/// </summary>
public enum GameState
{
    Reset,
    Standby,
    Playing,
    Pause,
    Finish
}

public class StateChangedArg : EventArgs
{
    public GameState gameState;
    public StateChangedArg(GameState state) { gameState = state; }
}

public delegate void StateChangedHandler(StateChangedArg arg);

public partial class  GameManager : MonoBehaviour
{
    /// <summary>
    /// Stateの移動時に呼ばれるイベント
    /// </summary>
    public event StateChangedHandler StateChange;

    //このGameManagerクラスのステートマシーン
    private ImtStateMachine<GameManager> _gameManagerStateMachine;

    GameState state = GameState.Reset;
    //MainGameの現在の状態
    public GameState CurrentGameState
    {
        get { return state; }
        private set { state = value; }
    }

    private void Awake()
    {
        _gameManagerStateMachine = new ImtStateMachine<GameManager>(this);

        //Stateの移動する方向とIdを関連づけ
        _gameManagerStateMachine.AddAnyTransition<ResetState>((int)GameState.Reset);
        _gameManagerStateMachine.AddAnyTransition<StandbyState>((int)GameState.Standby);
        _gameManagerStateMachine.AddAnyTransition<PlayingState>((int)GameState.Playing);
        _gameManagerStateMachine.AddAnyTransition<PauseState>((int)GameState.Pause);
        _gameManagerStateMachine.AddAnyTransition<FinishState>((int)GameState.Finish);

        _gameManagerStateMachine.SetStartState<ResetState>();
    }
    private void Start()
    {
        //ステートマシンを起動
        _gameManagerStateMachine.Update();
    }
    private void Update()
    {
        //ステートマシンを更新
        _gameManagerStateMachine.Update();
    }

    /// <summary>
    /// 任意のStateに変更するIdをステートマシンに送る
    /// </summary>
    public void StateChangeSignal(GameState state)
    {
        _gameManagerStateMachine.SendEvent((int)state);
        CurrentGameState = state;
        Debug.Log("Stateを"+state+"に移動しました");
    }

    /// <summary>
    /// イベント発火関数
    /// </summary>
    /// <param name="gameState"></param>
    public void StartStateChangeEvent(GameState gameState)
    {
        StateChange?.Invoke(new StateChangedArg(gameState));
    }


    //デバッグ用関数
    /// <summary>
    /// デバッグ用　現在のStateを変更する
    /// </summary>
    public void CurrentStateChanger(GameState gameState )
    {
        Debug.Log("デバッグ用関数を確認");
        StateChangeSignal(gameState);
    }
    /// <summary>
    /// デバッグ用　現在のStateを確認する
    /// </summary>
    public void CurrentStateResercher()
    {
        Debug.Log("デバッグ用関数を確認");
        Debug.Log("現在のStateは"+_gameManagerStateMachine.CurrentStateName);
    }

}
