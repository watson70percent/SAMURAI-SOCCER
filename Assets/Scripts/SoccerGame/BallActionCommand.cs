using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiSoccer.SoccerGame
{
    /// <summary>
    /// �{�[���𑀍삷��R�}���h�̌��D
    /// </summary>
    public abstract class BallActionCommand
    {

    }

    /// <summary>
    /// �h���u�������鎞�̃R�}���h�D
    /// </summary>
    public class DribbleCommand : BallActionCommand
    {
        public Transform m_owner;
        public PersonalStatus m_status;

        /// <summary>
        /// �h���u�������鎞�̃R�}���h�D
        /// </summary>
        /// <param name="owner">�h���u����D</param>
        /// <param name="status">�h���u����̃X�e�[�^�X�D</param>
        public DribbleCommand(Transform owner, PersonalStatus status)
        {
            m_owner = owner;
            m_status = status;
        }
    }

    /// <summary>
    /// �g���b�v���鎞�̃R�}���h�D
    /// </summary>
    public class TrapCommand : BallActionCommand
    {
        public Transform m_recever;
        public PersonalStatus m_status;

        /// <summary>
        /// �g���b�v���鎞�̃R�}���h�D
        /// </summary>
        /// <param name="recever">�g���b�v��D</param>
        /// <param name="status">�g���b�v��̃X�e�[�^�X�D</param>
        public TrapCommand(Transform recever, PersonalStatus status)
        {
            m_recever = recever;
            m_status = status;
        }
    }

    /// <summary>
    /// �p�X�̍����D
    /// </summary>
    public enum PassHeight
    {
        Low,
        Middle,
        High
    }

    /// <summary>
    /// �p�X�����鎞�̃R�}���h�D
    /// </summary>
    public class PassCommand : BallActionCommand
    {
        public Vector2 m_sender;
        public Vector2 m_recever;
        public PassHeight m_passHeight;
        public PersonalStatus m_status;

        /// <summary>
        /// �p�X�����鎞�̃R�}���h�D
        /// </summary>
        /// <param name="sender">�p�X��D</param>
        /// <param name="recever">�󂯎���D</param>
        /// <param name="passHeight">�p�X�̍����D</param>
        /// <param name="status">�p�X��̃X�e�[�^�X�D</param>
        public PassCommand(Vector2 sender, Vector2 recever, PassHeight passHeight, PersonalStatus status)
        {
            m_sender = sender;
            m_recever = recever;
            m_passHeight = passHeight;
            m_status = status;
        }
    }

    /// <summary>
    /// �V���[�g���鎞�̃R�}���h�D
    /// </summary>
    public class ShootCommand : BallActionCommand
    {
        public Transform m_sender;
        public PersonalStatus m_status;

        /// <summary>
        /// �V���[�g���鎞�̃R�}���h�D
        /// </summary>
        /// <param name="sender">�V���[�g��D</param>
        /// <param name="status">�V���[�g��̃X�e�[�^�X�D</param>
        public ShootCommand(Transform sender, PersonalStatus status)
        {
            m_sender = sender;
            m_status = status;
        }
    }
}
