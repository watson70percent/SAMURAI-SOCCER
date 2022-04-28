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
    public class InGameEvent
    {
        private Subject<Unit> m_resetSubject = new Subject<Unit>();

        /// <summary>
        /// Reset�C�x���g��Subscribe��
        /// </summary>
        public IObservable<Unit> Reset
        {
            get { return m_resetSubject; }
        }

        /// <summary>
        /// Reset�̃C�x���g�𔭍s
        /// </summary>
        public void ResetOnNext()
        {
            m_resetSubject.OnNext(Unit.Default);
        }

        private Subject<Unit> m_standbySubject = new Subject<Unit>();

        /// <summary>
        /// Standby�C�x���g��Subscribe��
        /// </summary>
        public IObservable<Unit> Standby
        {
            get { return m_standbySubject; }
        }

        /// <summary>
        /// Standby�C�x���g�𔭍s
        /// </summary>
        public void StandbyOnNext()
        {
            m_standbySubject.OnNext(Unit.Default);
        }

        private Subject<Unit> m_playSubject = new Subject<Unit>();

        /// <summary>
        /// Play�C�x���g��Subscribe��
        /// </summary>
        public IObservable<Unit> Play
        {
            get { return m_playSubject; }
        }

        /// <summary>
        /// Play�C�x���g�𔭍s
        /// </summary>
        public void PlayOnNext()
        {
            m_playSubject.OnNext(Unit.Default);
        }

        private Subject<Unit> m_goalSubject = new Subject<Unit>();

        /// <summary>
        /// Goal�C�x���g��Subscribe��
        /// </summary>
        public IObservable<Unit> Goal
        {
            get { return m_goalSubject; }
        }

        /// <summary>
        /// Goal�C�x���g�̔��s
        /// </summary>
        public void GoalOnNext()
        {
            m_goalSubject.OnNext(Unit.Default);
        }

        private Subject<bool> m_pauseSubject = new Subject<bool>();

        /// <summary>
        /// Pause�C�x���g��Subscribe�� true:�ꎞ��~, false:���� 
        /// </summary>
        public IObservable<bool> Pause
        {
            get { return m_pauseSubject; }
        }

        /// <summary>
        /// Pause�C�x���g�̔��s
        /// </summary>
        /// <param name="isPause">true:�ꎞ��~, false:����</param>
        public void PauseOnNext(bool isPause)
        {
            m_pauseSubject.OnNext(isPause);
        }

        private Subject<Unit> m_finishSubject = new Subject<Unit>();

        /// <summary>
        /// Finish�C�x���g��Subscribe��
        /// </summary>
        public IObservable<Unit> Finish
        {
            get { return m_finishSubject; }
        }

        /// <summary>
        /// Finish�C�x���g�̔��s
        /// </summary>
        public void FinishOnNext()
        {
            m_finishSubject.OnNext(Unit.Default);
        }
    }
}
