using System;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiSoccer.StageContents.StageSelect
{
    /// <summary>
    /// ����_�O�_�̓񎟃x�W�F�Ȑ��쐻�N���X�D
    /// </summary>
    public sealed class Bezier2Order3Points
    {
        private List<Vector2> m_points = new List<Vector2>();
        private List<float> m_lnCombi = new List<float>();

        /// <summary>
        /// �x�W�F�Ȑ��̍쐻�D
        /// </summary>
        /// <param name="start">�n�_</param>
        /// <param name="medium">����_</param>
        /// <param name="end">�I�_</param>
        public Bezier2Order3Points(Vector2 start, Vector2 medium, Vector2 end) {
            m_points.Add(start);
            m_points.Add(medium);
            m_points.Add(end);
            m_lnCombi.Add(Mathf.Log(1));
            m_lnCombi.Add(Mathf.Log(2));
            m_lnCombi.Add(Mathf.Log(1));
        }

        /// <summary>
        /// �x�W�F�Ȑ��̓_���擾�D
        /// </summary>
        /// <param name="t">�n�_</param>
        /// <returns>�_</returns>
        /// <exception cref="ArgumentException">�͈͊O��t����ꂽ�Ƃ���������D</exception>
        public Vector2 GetPoint(float t)
        {
            if (t < 0)
            {
                throw new ArgumentException("t < 0 is not allow.");
            }

            if (t > 1)
            {
                throw new ArgumentException("t > 1 is not allow.");
            }

            if (t == 0)
            {
                return m_points[0];
            }

            if (t == 1)
            {
                return m_points[2];
            }

            Vector2 ret = Vector2.zero;
            var ln_t = Mathf.Log(t);
            var ln_t_1 = Mathf.Log(1 - t);

            for (var i = 0; i < 3; i++)
            {
                ret += m_points[i] * Mathf.Exp(m_lnCombi[i] + i * ln_t + (2 - i) * ln_t_1);
            }

            return ret;
        }
    }
}
