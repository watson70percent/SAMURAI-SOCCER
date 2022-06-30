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
    public static class InGameEvent
    {
        private static bool m_isPlaying = false; //Play状態かどうか 

        private static Subject<Unit> m_resetSubject = new Subject<Unit>();
        private static IObservable<Unit> m_resetShareObservable = m_resetSubject.Share();

        /// <summary>
        /// ResetイベントのSubscribe先
        /// </summary>
        public static IObservable<Unit> Reset
        {
            get { return m_resetShareObservable; }
        }

        /// <summary>
        /// Resetのイベントを発行
        /// </summary>
        public static void ResetOnNext()
        {
            m_isPlaying = false;
            m_resetSubject.OnNext(Unit.Default);
        }

        private static Subject<Unit> m_standbySubject = new Subject<Unit>();
        private static IObservable<Unit> m_standbyShareObservable = m_standbySubject.Share();

        /// <summary>
        /// StandbyイベントのSubscribe先
        /// </summary>
        public static IObservable<Unit> Standby
        {
            get { return m_standbyShareObservable; }
        }

        /// <summary>
        /// Standbyイベントを発行
        /// </summary>
        public static void StandbyOnNext()
        {
            m_isPlaying = false;
            m_standbySubject.OnNext(Unit.Default);
        }

        private static Subject<Unit> m_playSubject = new Subject<Unit>();
        private static IObservable<Unit> m_playShareObservable = m_playSubject.Share();

        /// <summary>
        /// PlayイベントのSubscribe先
        /// </summary>
        public static IObservable<Unit> Play
        {
            get { return m_playShareObservable; }
        }

        /// <summary>
        /// Playイベントを発行
        /// </summary>
        public static void PlayOnNext()
        {
            m_isPlaying = true;
            m_playSubject.OnNext(Unit.Default);
        }

        private static IObservable<long> m_updateDuringPlayObservable = Observable.EveryUpdate().Where(_ => m_isPlaying).Share();
        /// <summary>
        /// Play中にUpdateタイミングで発行され続けるストリーム
        /// </summary>
        public static IObservable<long> UpdateDuringPlay
        {
            get { return m_updateDuringPlayObservable; }
        }

        private static Subject<Unit> m_goalSubject = new Subject<Unit>();
        private static IObservable<Unit> m_goalShareObservable = m_goalSubject.Share();

        /// <summary>
        /// GoalイベントのSubscribe先
        /// </summary>
        public static IObservable<Unit> Goal
        {
            get { return m_goalShareObservable; }
        }

        /// <summary>
        /// Goalイベントの発行
        /// </summary>
        public static void GoalOnNext()
        {
            m_isPlaying = false;
            m_goalSubject.OnNext(Unit.Default);
        }

        private static Subject<bool> m_pauseSubject = new Subject<bool>();
        private static IObservable<bool> m_pauseShareObservable = m_pauseSubject.Share();

        /// <summary>
        /// PauseイベントのSubscribe先 true:一時停止, false:解除 
        /// </summary>
        public static IObservable<bool> Pause
        {
            get { return m_pauseShareObservable; }
        }

        /// <summary>
        /// Pauseイベントの発行
        /// </summary>
        /// <param name="isPause">true:一時停止, false:解除</param>
        public static void PauseOnNext(bool isPause)
        {
            if (isPause)
            {
                m_isPlaying = false;
            }
            else
            {
                m_isPlaying = true;
            }
            m_pauseSubject.OnNext(isPause);
        }

        private static Subject<Unit> m_finishSubject = new Subject<Unit>();
        private static IObservable<Unit> m_finishShareObservable = m_finishSubject.Share();

        /// <summary>
        /// FinishイベントのSubscribe先
        /// </summary>
        public static IObservable<Unit> Finish
        {
            get { return m_finishShareObservable; }
        }

        /// <summary>
        /// Finishイベントの発行
        /// </summary>
        public static void FinishOnNext()
        {
            m_isPlaying = false;
            m_finishSubject.OnNext(Unit.Default);
        }
    }
}
