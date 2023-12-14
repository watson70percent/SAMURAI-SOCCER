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

        private static ReactiveProperty<Vector3> m_stickDir = new ReactiveProperty<Vector3>(Vector3.zero);
        public static IReadOnlyReactiveProperty<Vector3> StickDir
        {
            get { return m_stickDir; }
        }

        private static Subject<Unit> m_attackSubject = new Subject<Unit>();
        private static IObservable<Unit> m_attackShareObservable = m_attackSubject.Share();

        /// <summary>
        /// AttackイベントのSubscribe先
        /// </summary>
        public static IObservable<Unit> Attack
        {
            get { return m_attackShareObservable; }
        }

        //ReactivePropertyを定義して攻撃できるかどうかのフラグをつくる
        private static IReadOnlyReactiveProperty<bool> m_isEnableAttack = Observable.Merge(
                InGameEvent.Play.Select(_ => true), //Playのときだけtrueに
                InGameEvent.Goal.Select(_ => false),
                InGameEvent.Finish.Select(_ => false)
            ).ToReactiveProperty<bool>(false);

        private static IReadOnlyReactiveProperty<bool> m_getPenalty = Observable.Merge(
                InGameEvent.Penalty.Select(_ => false),
                InGameEvent.Penalty.Delay(TimeSpan.FromSeconds(1.0)).Select(_ => true) // 1秒経ったらペナルティで斬れなくする処理を解除
            ).ToReactiveProperty<bool>(true);

        /// <summary>
        /// Attackのイベントを発行
        /// </summary>
        public static void AttackOnNext()
        {
            if(m_isEnableAttack.Value && m_getPenalty.Value) m_attackSubject.OnNext(Unit.Default);
        }

        //審判がファールチェックするトリガー
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
        /// 移動イベントを発行
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
        /// ため攻撃中かどうか
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
        /// ため攻撃が可能かどうか
        /// </summary>
        public static IReadOnlyReactiveProperty<bool> IsEnableChargeAttack
        {
            get { return m_isEnableChargeAttack; }
        }
        public static void SetIsEnableChargeAtack(bool flag)
        {
            if (m_lockChargeAttack.Value) return; // ため斬りを使えなくしているときは値を変えない
            m_isEnableChargeAttack.Value = flag;
        }

        private static ReactiveProperty<bool> m_lockChargeAttack = new ReactiveProperty<bool>(false);
        /// <summary>
        /// ため攻撃を使えなくするかどうか
        /// </summary>
        public static IReadOnlyReactiveProperty <bool> IsLockChargeAttack
        {
            get { return m_lockChargeAttack; }
        }

        /// <summary>
        /// ため攻撃の使用可能状態を設定する
        /// </summary>
        /// <param name="value">true：ため攻撃できない</param>
        public static void SetLockChargeAttack(bool value)
        {
            m_lockChargeAttack.Value = value;
        }
    }
}
