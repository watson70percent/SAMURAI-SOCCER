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

        private static ReplaySubject<Unit> m_resetSubject = new ReplaySubject<Unit>(1);

        /// <summary>
        /// Reset�C�x���g��Subscribe��
        /// </summary>
        public static IObservable<Unit> Reset
        {
            get { return m_resetSubject; }
        }

        /// <summary>
        /// Reset�̃C�x���g�𔭍s
        /// </summary>
        public static void ResetOnNext()
        {
            m_isPlaying = false;
            m_resetSubject.OnNext(Unit.Default);
        }

        public static void ResetResetSubject()
        {
            m_resetSubject.Dispose();
            m_resetSubject = new ReplaySubject<Unit>(1);
        }

        private static Subject<bool> m_standbySubject = new Subject<bool>();
        private static IObservable<bool> m_standbyShareObservable = m_standbySubject.Share();

        /// <summary>
        /// Standby�C�x���g��Subscribe��
        /// </summary>
        public static IObservable<bool> Standby
        {
            get { return m_standbyShareObservable; }
        }

        /// <summary>
        /// Standby�C�x���g�𔭍s
        /// </summary>
        /// <param name="isTeammateBall">
        /// ���`�[���{�[�����B
        /// </param>
        public static void StandbyOnNext(bool isTeammateBall = false)
        {
            m_isPlaying = false;
            m_standbySubject.OnNext(isTeammateBall);
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

        private static Subject<GoalEventType> m_goalSubject = new Subject<GoalEventType>();
        private static IObservable<GoalEventType> m_goalShareObservable = m_goalSubject.Share();

        /// <summary>
        /// Goal�C�x���g��Subscribe��
        /// </summary>
        public static IObservable<GoalEventType> Goal
        {
            get { return m_goalShareObservable; }
        }

        /// <summary>
        /// Goal�C�x���g�̔��s
        /// </summary>
        public static void GoalOnNext(GoalEventType type)
        {
            m_isPlaying = false;
            m_goalSubject.OnNext(type);
        }

        private static Subject<int> m_penaltySubject = new Subject<int>();
        private static IObservable<int> m_penaltySubjectObservable = m_penaltySubject.Share();

        /// <summary>
        /// Penalty�C�x���g��Subscribe�� 0:�x��, 1:�ޏ�
        /// </summary>
        public static IObservable<int> Penalty
        {
            get { return m_penaltySubjectObservable; }
        }

        /// <summary>
        /// Penalty�C�x���g�̔��s
        /// </summary>
        /// <param name="penaltycount">0:�x��, 1:�ޏ�</param>
        public static void PenaltyOnNext(int penaltycount)
        {
            m_penaltySubject.OnNext(penaltycount);
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

        private static ReactiveProperty<int> m_teammateScoreSubject = new ReactiveProperty<int>(0);

        /// <summary>
        /// TeammateScore�C�x���g��Subscribe��
        /// </summary>
        public static IObservable<int> TeammateScore
        {
            get { return m_teammateScoreSubject; }
        }

        /// <summary>
        /// TeammateScore�C�x���g�̔��s
        /// </summary>
        public static void TeammateScoreOnNext(int score)
        {
            m_teammateScoreSubject.Value = score;
            Debug.Log("Game score: " + m_teammateScoreSubject.Value + " : " + m_opponentScoreSubject.Value);
        }

        private static ReactiveProperty<int> m_opponentScoreSubject = new ReactiveProperty<int>(0);

        /// <summary>
        /// OpponentScore�C�x���g��Subscribe��
        /// </summary>
        public static IObservable<int> OpponentScore
        {
            get { return m_opponentScoreSubject; }
        }

        /// <summary>
        /// OpponentScore�C�x���g�̔��s
        /// </summary>
        public static void OpponentScoreOnNext(int score)
        {
            m_opponentScoreSubject.Value = score;
            Debug.Log("Game score: " + m_teammateScoreSubject.Value + " : " + m_opponentScoreSubject.Value);
        }
    }
}
