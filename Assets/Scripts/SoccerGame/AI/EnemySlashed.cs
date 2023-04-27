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
        /// a‚ç‚ê‚½‚Æ‚«‚Ìˆ—
        /// </summary>
        /// <param name="dir">”ò‚ñ‚Å‚¢‚­•ûŒü</param>
        public void Slashed(Vector3 dir)
        {
            m_rigidbody.AddForce(dir * 1000, ForceMode.Impulse);
            m_easyCPU.Attacked();
            if (!m_isKilled)
            {
                GetComponent<Collider>().enabled = false;
                _ = SoundMaster.Instance.PlaySE(3); // a‚ç‚ê‚½SE
                _ = EasyCPUManager.Kill(this.gameObject);
                m_isKilled = true;
            }
        }
    }
}
