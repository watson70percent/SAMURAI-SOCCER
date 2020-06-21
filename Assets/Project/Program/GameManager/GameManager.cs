using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IceMilkTea.Core;

public class GameManager : MonoBehaviour
{
    private enum GameStateId
    {
        AllReseted,
        Play,
        Pose,
        PoseBack,
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
        _gameManagerStateMachine.AddTransition<PlayingState, PoseState>((int)GameStateId.Pose);
        _gameManagerStateMachine.AddTransition<PoseState, PlayingState>((int)GameStateId.PoseBack);
        _gameManagerStateMachine.AddTransition<PoseState, ResetState>((int)GameStateId.PlayAgain);
        _gameManagerStateMachine.AddTransition<PlayingState,FinishState>((int)GameStateId.Finish);

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

    public void AllResetedSignal()
    {
        //ステートマシンにAllResetedを送る
        _gameManagerStateMachine.SendEvent((int)GameStateId.AllReseted);
    }

    public void Playsignal()
    {
        //ステートマシンにPlayを送る
        _gameManagerStateMachine.SendEvent((int)GameStateId.Play);
    }

    public void PoseSignal()
    {
        //ステートマシンにPoseを送る
        _gameManagerStateMachine.SendEvent((int)GameStateId.Pose);
    }

    public void PoseBackSignal()
    {
        //ステートマシンにBackを送る
        _gameManagerStateMachine.SendEvent((int)GameStateId.PoseBack);
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



}
