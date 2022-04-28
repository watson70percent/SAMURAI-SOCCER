using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace SamuraiSoccer.Event
{
    /// <summary>
    /// 試合中のイベントを管理
    /// </summary>
    public class InGameEvent
    {
        private Subject<Unit> m_resetSubject = new Subject<Unit>();

        /// <summary>
        /// ResetイベントのSubscribe先
        /// </summary>
        public IObservable<Unit> Reset
        {
            get { return m_resetSubject; }
        }

        /// <summary>
        /// Resetのイベントを発行
        /// </summary>
        public void ResetOnNext()
        {
            m_resetSubject.OnNext(Unit.Default);
        }

        private Subject<Unit> m_standbySubject = new Subject<Unit>();

        /// <summary>
        /// StandbyイベントのSubscribe先
        /// </summary>
        public IObservable<Unit> Standby
        {
            get { return m_standbySubject; }
        }

        /// <summary>
        /// Standbyイベントを発行
        /// </summary>
        public void StandbyOnNext()
        {
            m_standbySubject.OnNext(Unit.Default);
        }

        private Subject<Unit> m_playSubject = new Subject<Unit>();

        /// <summary>
        /// PlayイベントのSubscribe先
        /// </summary>
        public IObservable<Unit> Play
        {
            get { return m_playSubject; }
        }

        /// <summary>
        /// Playイベントを発行
        /// </summary>
        public void PlayOnNext()
        {
            m_playSubject.OnNext(Unit.Default);
        }

        private Subject<Unit> m_goalSubject = new Subject<Unit>();

        /// <summary>
        /// GoalイベントのSubscribe先
        /// </summary>
        public IObservable<Unit> Goal
        {
            get { return m_goalSubject; }
        }

        /// <summary>
        /// Goalイベントの発行
        /// </summary>
        public void GoalOnNext()
        {
            m_goalSubject.OnNext(Unit.Default);
        }

        private Subject<bool> m_pauseSubject = new Subject<bool>();

        /// <summary>
        /// PauseイベントのSubscribe先 true:一時停止, false:解除 
        /// </summary>
        public IObservable<bool> Pause
        {
            get { return m_pauseSubject; }
        }

        /// <summary>
        /// Pauseイベントの発行
        /// </summary>
        /// <param name="isPause">true:一時停止, false:解除</param>
        public void PauseOnNext(bool isPause)
        {
            m_pauseSubject.OnNext(isPause);
        }

        private Subject<Unit> m_finishSubject = new Subject<Unit>();

        /// <summary>
        /// FinishイベントのSubscribe先
        /// </summary>
        public IObservable<Unit> Finish
        {
            get { return m_finishSubject; }
        }

        /// <summary>
        /// Finishイベントの発行
        /// </summary>
        public void FinishOnNext()
        {
            m_finishSubject.OnNext(Unit.Default);
        }
    }
}
