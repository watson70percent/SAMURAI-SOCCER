using System;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiSoccer.StageContents.StageSelect
{
    /// <summary>
    /// 制御点三点の二次ベジェ曲線作製クラス．
    /// </summary>
    public sealed class Bezier2Order3Points
    {
        private List<Vector2> m_points = new List<Vector2>();
        private List<float> m_lnCombi = new List<float>();

        /// <summary>
        /// ベジェ曲線の作製．
        /// </summary>
        /// <param name="start">始点</param>
        /// <param name="medium">制御点</param>
        /// <param name="end">終点</param>
        public Bezier2Order3Points(Vector2 start, Vector2 medium, Vector2 end) {
            m_points.Add(start);
            m_points.Add(medium);
            m_points.Add(end);
            m_lnCombi.Add(Mathf.Log(1));
            m_lnCombi.Add(Mathf.Log(2));
            m_lnCombi.Add(Mathf.Log(1));
        }

        /// <summary>
        /// ベジェ曲線の点を取得．
        /// </summary>
        /// <param name="t">地点</param>
        /// <returns>点</returns>
        /// <exception cref="ArgumentException">範囲外のtを入れたとき投げられる．</exception>
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
