using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiSoccer.SoccerGame
{
    /// <summary>
    /// ボールを操作するコマンドの元．
    /// </summary>
    public abstract class BallActionCommand
    {

    }

    /// <summary>
    /// ドリブルをする時のコマンド．
    /// </summary>
    public class DribbleCommand : BallActionCommand
    {
        public Transform m_owner;
        public PersonalStatus m_status;

        /// <summary>
        /// ドリブルをする時のコマンド．
        /// </summary>
        /// <param name="owner">ドリブル主．</param>
        /// <param name="status">ドリブル主のステータス．</param>
        public DribbleCommand(Transform owner, PersonalStatus status)
        {
            m_owner = owner;
            m_status = status;
        }
    }

    /// <summary>
    /// トラップする時のコマンド．
    /// </summary>
    public class TrapCommand : BallActionCommand
    {
        public Transform m_recever;
        public PersonalStatus m_status;

        /// <summary>
        /// トラップする時のコマンド．
        /// </summary>
        /// <param name="recever">トラップ主．</param>
        /// <param name="status">トラップ主のステータス．</param>
        public TrapCommand(Transform recever, PersonalStatus status)
        {
            m_recever = recever;
            m_status = status;
        }
    }

    /// <summary>
    /// パスの高さ．
    /// </summary>
    public enum PassHeight
    {
        Low,
        Middle,
        High
    }

    /// <summary>
    /// パスをする時のコマンド．
    /// </summary>
    public class PassCommand : BallActionCommand
    {
        public Vector2 m_sender;
        public Vector2 m_recever;
        public PassHeight m_passHeight;
        public PersonalStatus m_status;

        /// <summary>
        /// パスをする時のコマンド．
        /// </summary>
        /// <param name="sender">パス主．</param>
        /// <param name="recever">受け取り主．</param>
        /// <param name="passHeight">パスの高さ．</param>
        /// <param name="status">パス主のステータス．</param>
        public PassCommand(Vector2 sender, Vector2 recever, PassHeight passHeight, PersonalStatus status)
        {
            m_sender = sender;
            m_recever = recever;
            m_passHeight = passHeight;
            m_status = status;
        }
    }

    /// <summary>
    /// シュートする時のコマンド．
    /// </summary>
    public class ShootCommand : BallActionCommand
    {
        public Transform m_sender;
        public PersonalStatus m_status;

        /// <summary>
        /// シュートする時のコマンド．
        /// </summary>
        /// <param name="sender">シュート主．</param>
        /// <param name="status">シュート主のステータス．</param>
        public ShootCommand(Transform sender, PersonalStatus status)
        {
            m_sender = sender;
            m_status = status;
        }
    }
}
