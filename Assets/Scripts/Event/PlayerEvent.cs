using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace SamuraiSoccer.Event
{
    /// <summary>
    /// �v���C���[�̃C�x���g���Ǘ�
    /// </summary>
    public static class PlayerEvent
    {

        private static ReactiveProperty<Vector3> m_stickDir = new ReactiveProperty<Vector3>(Vector3.zero);
        public static IReadOnlyReactiveProperty<Vector3> StickDir
        {
            get { return m_stickDir; }
        }

        private static Subject<Unit> m_attackSubject = new Subject<Unit>();
        private static IObservable<Unit> m_attackShareObservable = m_attackSubject.Share();

        /// <summary>
        /// Attack�C�x���g��Subscribe��
        /// </summary>
        public static IObservable<Unit> Attack
        {
            get { return m_attackShareObservable; }
        }

        //ReactiveProperty���`���čU���ł��邩�ǂ����̃t���O������
        private static IReadOnlyReactiveProperty<bool> m_isEnableAttack = Observable.Merge(
                InGameEvent.Play.Select(_ => true), //Play�̂Ƃ�����true��
                InGameEvent.Goal.Select(_ => false),
                InGameEvent.Finish.Select(_ => false)
            ).ToReactiveProperty<bool>(false);

        private static IReadOnlyReactiveProperty<bool> m_getPenalty = Observable.Merge(
                InGameEvent.Penalty.Select(_ => false),
                InGameEvent.Penalty.Delay(TimeSpan.FromSeconds(1.0)).Select(_ => true) // 1�b�o������y�i���e�B�Ŏa��Ȃ����鏈��������
            ).ToReactiveProperty<bool>(true);

        /// <summary>
        /// Attack�̃C�x���g�𔭍s
        /// </summary>
        public static void AttackOnNext()
        {
            if(m_isEnableAttack.Value && m_getPenalty.Value) m_attackSubject.OnNext(Unit.Default);
        }

        //�R�����t�@�[���`�F�b�N����g���K�[
        private static Subject<Unit> m_faulCheckSubject = new Subject<Unit>();
        private static IObservable<Unit> m_faulCheckShareObservable = m_faulCheckSubject.Share();

        public static IObservable<Unit> FaulCheck
        {
            get { return m_faulCheckShareObservable; }
        }
        public static void FaulCheckOnNext()
        {
            m_faulCheckSubject.OnNext(Unit.Default);
        }



        private static Subject<Vector3> m_stickInputSubject = new Subject<Vector3>();
        private static IObservable<Vector3> m_stickInputShareObservable = m_stickInputSubject.Share();

        /// <summary>
        /// �ړ��C�x���g�𔭍s
        /// </summary>
        public static IObservable<Vector3> StickInput
        {
            get { return m_stickInputShareObservable; }
        }

        public static void StickControllerOnNext(Vector3 dir)
        {
            m_stickDir.Value = dir;
            m_stickInputSubject.OnNext(dir);
        }

        private static ReactiveProperty<bool> m_isInChargeAttack = new ReactiveProperty<bool>(false);
        /// <summary>
        /// ���ߍU�������ǂ���
        /// </summary>
        public static IReadOnlyReactiveProperty<bool> IsInChargeAttack
        {
            get { return m_isInChargeAttack; }
        }
        public static void SetIsInChargeAtack(bool flag)
        {
            if (!flag)
            {
                m_isInChargeAttack.Value = flag;
                return;
            }
            if (m_isEnableAttack.Value && m_getPenalty.Value) m_isInChargeAttack.Value = flag;
        }

        private static ReactiveProperty<bool> m_isEnableChargeAttack = new ReactiveProperty<bool>(false);
        /// <summary>
        /// ���ߍU�����\���ǂ���
        /// </summary>
        public static IReadOnlyReactiveProperty<bool> IsEnableChargeAttack
        {
            get { return m_isEnableChargeAttack; }
        }
        public static void SetIsEnableChargeAtack(bool flag)
        {
            if (m_lockChargeAttack.Value) return; // ���ߎa����g���Ȃ����Ă���Ƃ��͒l��ς��Ȃ�
            m_isEnableChargeAttack.Value = flag;
        }

        private static ReactiveProperty<bool> m_lockChargeAttack = new ReactiveProperty<bool>(false);
        /// <summary>
        /// ���ߍU�����g���Ȃ����邩�ǂ���
        /// </summary>
        public static IReadOnlyReactiveProperty <bool> IsLockChargeAttack
        {
            get { return m_lockChargeAttack; }
        }

        /// <summary>
        /// ���ߍU���̎g�p�\��Ԃ�ݒ肷��
        /// </summary>
        /// <param name="value">true�F���ߍU���ł��Ȃ�</param>
        public static void SetLockChargeAttack(bool value)
        {
            m_lockChargeAttack.Value = value;
        }
    }
}
