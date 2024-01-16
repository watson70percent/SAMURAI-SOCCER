using System;
using UnityEngine;

namespace SamuraiSoccer.SoccerGame
{
    /// <summary>
    /// 出したパスに関する情報。
    /// </summary>
    public sealed class PassInfo
    {
        private static readonly float gravity = 9.8f;

        public readonly GameObject m_recever;
        private readonly Vector2 m_recevePos;
        private readonly Rigidbody m_rigidbody;
        public readonly DateTime m_start;
        private readonly DateTime m_end;

        public PassInfo(GameObject recever, Vector2 recevePos, Rigidbody rigidbody, DateTime start, DateTime end)
        {
            m_recever = recever;
            m_recevePos = recevePos;
            m_rigidbody = rigidbody;
            m_start = start;
            m_end = end;
        }

        public (bool valid, Vector2 destPos) GetInfo()
        {
            if (DateTime.Now < m_start || DateTime.Now > m_end + (m_end - m_start) * 0.2)
            {
                return (false, Vector2.zero);
            }

            var t = (m_rigidbody.velocity.y + Mathf.Sqrt(m_rigidbody.velocity.y * m_rigidbody.velocity.y + 2 * gravity * m_rigidbody.position.y)) / gravity;
            var p = m_rigidbody.position + m_rigidbody.velocity * t;
            var fall = new Vector2(p.x, p.z);
            var dt = DateTime.Now.AddSeconds(t) - m_start;
            return (true, Vector2.Lerp(m_recevePos, fall, Mathf.Min(1.0f, 0.25f + (float)((DateTime.Now - m_start) / dt))));
        }
    }
}
