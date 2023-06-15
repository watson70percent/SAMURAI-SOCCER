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

        /// <summary>
        /// Attack�̃C�x���g�𔭍s
        /// </summary>
        public static void AttackOnNext()
        {
            if(m_isEnableAttack.Value) m_attackSubject.OnNext(Unit.Default);
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
            m_isInChargeAttack.Value = flag;
        }
    }
}
