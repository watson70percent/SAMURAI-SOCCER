using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiSoccer.SoccerGame.AI
{
    public class FatEnemySlashed : EnemySlashed
    {
        /// <summary>
        /// 斬られたときの処理
        /// </summary>
        /// <param name="dir">飛んでいく方向</param>
        public override void Slashed(Vector3 dir)
        {
            if (m_easyCPU.status.hp == 1)
            {
                m_rigidbody.AddForce(dir * 1000, ForceMode.Impulse);
                if (!m_isKilled)
                {
                    GetComponent<Collider>().enabled = false;                 
                    _ = EasyCPUManager.Kill(this.gameObject);
                    m_isKilled = true;
                }
            }
            _ = SoundMaster.Instance.PlaySE(3); // 斬られたSE
            m_easyCPU.Attacked();
        }
    }
}
