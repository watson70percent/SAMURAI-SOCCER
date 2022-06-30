using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace SamuraiSoccer.Event
{
    /// <summary>
    /// �������̃C�x���g���Ǘ�
    /// </summary>
    public static class InGameEvent
    {
        private static bool m_isPlaying = false; //Play��Ԃ��ǂ��� 

        private static Subject<Unit> m_resetSubject = new Subject<Unit>();
        private static IObservable<Unit> m_resetShareObservable = m_resetSubject.Share();

        /// <summary>
        /// Reset�C�x���g��Subscribe��
        /// </summary>
        public static IObservable<Unit> Reset
        {
            get { return m_resetShareObservable; }
        }

        /// <summary>
        /// Reset�̃C�x���g�𔭍s
        /// </summary>
        public static void ResetOnNext()
        {
            m_isPlaying = false;
            m_resetSubject.OnNext(Unit.Default);
        }

        private static Subject<Unit> m_standbySubject = new Subject<Unit>();
        private static IObservable<Unit> m_standbyShareObservable = m_standbySubject.Share();

        /// <summary>
        /// Standby�C�x���g��Subscribe��
        /// </summary>
        public static IObservable<Unit> Standby
        {
            get { return m_standbyShareObservable; }
        }

        /// <summary>
        /// Standby�C�x���g�𔭍s
        /// </summary>
        public static void StandbyOnNext()
        {
            m_isPlaying = false;
            m_standbySubject.OnNext(Unit.Default);
        }

        private static Subject<Unit> m_playSubject = new Subject<Unit>();
        private static IObservable<Unit> m_playShareObservable = m_playSubject.Share();

        /// <summary>
        /// Play�C�x���g��Subscribe��
        /// </summary>
        public static IObservable<Unit> Play
        {
            get { return m_playShareObservable; }
        }

        /// <summary>
        /// Play�C�x���g�𔭍s
        /// </summary>
        public static void PlayOnNext()
        {
            m_isPlaying = true;
            m_playSubject.OnNext(Unit.Default);
        }

        private static IObservable<long> m_updateDuringPlayObservable = Observable.EveryUpdate().Where(_ => m_isPlaying).Share();
        /// <summary>
        /// Play����Update�^�C�~���O�Ŕ��s���ꑱ����X�g���[��
        /// </summary>
        public static IObservable<long> UpdateDuringPlay
        {
            get { return m_updateDuringPlayObservable; }
        }

        private static Subject<Unit> m_goalSubject = new Subject<Unit>();
        private static IObservable<Unit> m_goalShareObservable = m_goalSubject.Share();

        /// <summary>
        /// Goal�C�x���g��Subscribe��
        /// </summary>
        public static IObservable<Unit> Goal
        {
            get { return m_goalShareObservable; }
        }

        /// <summary>
        /// Goal�C�x���g�̔��s
        /// </summary>
        public static void GoalOnNext()
        {
            m_isPlaying = false;
            m_goalSubject.OnNext(Unit.Default);
        }

        private static Subject<bool> m_pauseSubject = new Subject<bool>();
        private static IObservable<bool> m_pauseShareObservable = m_pauseSubject.Share();

        /// <summary>
        /// Pause�C�x���g��Subscribe�� true:�ꎞ��~, false:���� 
        /// </summary>
        public static IObservable<bool> Pause
        {
            get { return m_pauseShareObservable; }
        }

        /// <summary>
        /// Pause�C�x���g�̔��s
        /// </summary>
        /// <param name="isPause">true:�ꎞ��~, false:����</param>
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
        /// Finish�C�x���g��Subscribe��
        /// </summary>
        public static IObservable<Unit> Finish
        {
            get { return m_finishShareObservable; }
        }

        /// <summary>
        /// Finish�C�x���g�̔��s
        /// </summary>
        public static void FinishOnNext()
        {
            m_isPlaying = false;
            m_finishSubject.OnNext(Unit.Default);
        }
    }
}
