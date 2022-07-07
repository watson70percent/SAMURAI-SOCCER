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

        /// <summary>
        /// Attack�̃C�x���g�𔭍s
        /// </summary>
        public static void AttackOnNext()
        {
            m_attackSubject.OnNext(Unit.Default);
        }


        private static Subject<Vector3> m_stickControllerSubject = new Subject<Vector3>();
        private static IObservable<Vector3> m_stickControllerShareObservable = m_stickControllerSubject.Share();

        /// <summary>
        /// �ړ��C�x���g�𔭍s
        /// </summary>
        public static IObservable<Vector3> StickControllerSubject
        {
            get { return m_stickControllerShareObservable; }
        }

        public static void StickControllerOnNext(Vector3 dir)
        {
            m_stickControllerSubject.OnNext(dir);
        }
    }
}
