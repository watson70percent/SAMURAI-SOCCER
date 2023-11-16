using UnityEngine;
using System.Linq;
using System;
using Cysharp.Threading.Tasks;

namespace SamuraiSoccer.StageContents.BattleDome
{
    public class Indicator : MonoBehaviour
    {
        public GameObject m_indicatorPlane;
        public AudioSource m_bgm;

        private Material[] m_objs;
        private BSA bgmData;
        private DateTime m_start_time;
        private double m_levelNormalizer;

        private readonly Vector3 m_center = new Vector3(61, 3.65f, 49);
        private readonly float m_width = 2.0f;
        private readonly string m_fileName = "samurai-full-flavor.bsa";

        // Start is called before the first frame update
        async UniTask Start()
        {
            var time = DateTime.Now;
            bgmData = new BSA();
            await bgmData.Load(m_fileName);
            m_levelNormalizer = bgmData.LevelUnit;
            var dt = DateTime.Now - time;
            time = DateTime.Now;
            Debug.Log("t1 is:" + dt.TotalMilliseconds);

            var w_start = m_center.z - m_width * bgmData.FCount / 2 + m_width / 2;
            var grid = Enumerable.Range(0, bgmData.FCount).Select(i => new Vector3(m_center.x, m_center.y, w_start + i * m_width)).Reverse().ToArray();

            dt = DateTime.Now - time;
            time = DateTime.Now;
            Debug.Log("t2 is:" + dt.TotalMilliseconds);

            m_objs = grid.Select(g =>
            {
                var o = Instantiate(m_indicatorPlane, g, Quaternion.identity);
                var m = o.GetComponentInChildren<Renderer>().material;
                m.SetFloat("_Level", 0.0f);
                return m;
            }).ToArray();

            dt = DateTime.Now - time;
            time = DateTime.Now;
            Debug.Log("t3 is:" + dt.TotalMilliseconds);

            m_start_time = DateTime.Now.AddSeconds(5.0);
        }

        private void Update()
        {
            var t = DateTime.Now - m_start_time;

            if (t.TotalMilliseconds < 0)
            {
                return;
            }

            if (m_bgm.time >= m_bgm.clip.length)
            {
                foreach (var m in m_objs)
                {
                    m.SetFloat("_Level", 0.0f);
                }
                return;
            }

            if (!m_bgm.isPlaying)
            {
                m_bgm.Play();
            }

            var t_idx = Mathf.FloorToInt(m_bgm.time / bgmData.StrideTime);

            if (t_idx >= bgmData.SampleCount)
            {
                return;
            }

            var levels = bgmData.LevelData(t_idx);

            for (int i = 0; i < bgmData.FCount; i++)
            {
                m_objs[i].SetFloat("_Level", (float)(levels[i] * m_levelNormalizer));
            }
        }
    }
}
