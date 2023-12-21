using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiSoccer.SoccerGame.AI
{
    public class FatEnemySlashed : EnemySlashed
    {
        /// <summary>
        /// a‚ç‚ê‚½‚Æ‚«‚Ìˆ—
        /// </summary>
        /// <param name="dir">”ò‚ñ‚Å‚¢‚­•ûŒü</param>
        public override void Slashed(Vector3 dir)
        {
            if (m_easyCPU.status.hp <= 1)
            {
                m_rigidbody.AddForce(dir * 1000, ForceMode.Impulse);
                if (!m_isKilled)
                {
                    GetComponent<Collider>().enabled = false;                 
                    _ = EasyCPUManager.Kill(this.gameObject);
                    m_isKilled = true;
                }
            }
            else
            {
                m_rigidbody.AddForce(dir * 20, ForceMode.Impulse);
            }
            _ = SoundMaster.Instance.PlaySE(3); // a‚ç‚ê‚½SE
            m_easyCPU.Attacked();
        }
    }
}
