using System;
using UnityEngine;

namespace SamuraiSoccer.SoccerGame
{
    /// <summary>
    /// �o�����p�X�Ɋւ�����B
    /// </summary>
    public sealed class PassInfo
    {
        public readonly GameObject m_recever;
        public readonly Vector2 m_recevePos;
        public readonly DateTime m_start;
        public readonly DateTime m_end;

        public PassInfo(GameObject recever, Vector2 recevePos, DateTime start, DateTime end)
        {
            m_recever = recever;
            m_recevePos = recevePos;
            m_start = start;
            m_end = end;
        }
    }
}
