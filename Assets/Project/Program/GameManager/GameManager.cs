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

public partial class  GameManager : MonoBehaviour,IGameManagerable
{
    /// <summary>
    /// Stateの移動時に呼ばれるイベント
    /// </summary>
    public event StateChangedHandler StateChange;

    private enum GameStateId
    {
        AllReseted,
        Play,
        Pause,
        PauseBack,
        PlayAgain,
        Finish,
    }
    //このGameManagerクラスのステートマシーン
    private ImtStateMachine<GameManager> _gameManagerStateMachine;
    private void Awake()
    {
        _gameManagerStateMachine = new ImtStateMachine<GameManager>(this);
        _gameManagerStateMachine.AddTransition<ResetState,StandbyState>((int)GameStateId.AllReseted);
        _gameManagerStateMachine.AddTransition<StandbyState,PlayingState>((int)GameStateId.Play);
        _gameManagerStateMachine.AddTransition<PlayingState, PauseState>((int)GameStateId.Pause);
        _gameManagerStateMachine.AddTransition<PauseState, PlayingState>((int)GameStateId.PauseBack);
        _gameManagerStateMachine.AddTransition<PauseState, ResetState>((int)GameStateId.PlayAgain);
        _gameManagerStateMachine.AddTransition<PlayingState,FinishState>((int)GameStateId.Finish);

        //無理やりStateを移動する用
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
        Debug.Log("Stateを"+state+"に移動しました");
    }

    public void AllResetedSignal()
    {
        //ステートマシンにAllResetedを送る
        _gameManagerStateMachine.SendEvent((int)GameStateId.AllReseted);
    }

    public void PlaySignal()
    {
        //ステートマシンにPlayを送る
        _gameManagerStateMachine.SendEvent((int)GameStateId.Play);
    }

    public void PauseSignal()
    {
        //ステートマシンにPoseを送る
        _gameManagerStateMachine.SendEvent((int)GameStateId.Pause);
    }

    public void PauseBackSignal()
    {
        //ステートマシンにBackを送る
        _gameManagerStateMachine.SendEvent((int)GameStateId.PauseBack);
    }

    public void PlayAgainSignal()
    {
        //ステートマシンにPlayAgainを送る
        _gameManagerStateMachine.SendEvent((int)GameStateId.PlayAgain);
    }

    public void FinishSignal()
    {
        //ステートマシンにFinishを送る
        _gameManagerStateMachine.SendEvent((int)GameStateId.Finish);
    }

    //Poseボタンの処理
    public void OnClickPauseButton()
    {
        PauseSignal();
    }

    //Pose状態から戻るボタン
    public void OnClickPauseBackButton()
    {
        PauseBackSignal();
    }

    //再試合ボタン
    public void OnClickPlayAgainButton()
    {
        PlayAgainSignal();
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
