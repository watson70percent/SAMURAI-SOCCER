using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiSoccer.SoccerGame.AI
{
    public class EnemySlashed : MonoBehaviour, ISlashed
    {
        [SerializeField]
        private EasyCPU m_easyCPU;

        [SerializeField]
        private Rigidbody m_rigidbody;

        public EasyCPUManager EasyCPUManager { get; set; }

        private bool m_isKilled = false;

        /// <summary>
        /// 斬られたときの処理
        /// </summary>
        /// <param name="dir">飛んでいく方向</param>
        public void Slashed(Vector3 dir)
        {
            m_rigidbody.AddForce(dir * 1000, ForceMode.Impulse);
            m_easyCPU.Attacked();
            if (!m_isKilled)
            {
                GetComponent<Collider>().enabled = false;
                _ = SoundMaster.Instance.PlaySE(3); // 斬られたSE
                _ = EasyCPUManager.Kill(this.gameObject);
                m_isKilled = true;
            }
        }
    }
}
