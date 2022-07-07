using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace SamuraiSoccer.Event
{
    /// <summary>
    /// プレイヤーのイベントを管理
    /// </summary>
    public static class PlayerEvent
    {
        private static Subject<Unit> m_attackSubject = new Subject<Unit>();
        private static IObservable<Unit> m_attackShareObservable = m_attackSubject.Share();

        /// <summary>
        /// AttackイベントのSubscribe先
        /// </summary>
        public static IObservable<Unit> Attack
        {
            get { return m_attackShareObservable; }
        }

        /// <summary>
        /// Attackのイベントを発行
        /// </summary>
        public static void AttackOnNext()
        {
            m_attackSubject.OnNext(Unit.Default);
        }


        private static Subject<Vector3> m_stickControllerSubject = new Subject<Vector3>();
        private static IObservable<Vector3> m_stickControllerShareObservable = m_stickControllerSubject.Share();

        /// <summary>
        /// 移動イベントを発行
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
