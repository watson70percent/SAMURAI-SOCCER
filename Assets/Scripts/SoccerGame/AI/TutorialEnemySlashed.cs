using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiSoccer.SoccerGame.AI
{
    public class TutorialEnemySlashed : MonoBehaviour, ISlashed
    {
        [SerializeField]
        private Rigidbody m_rigidbody;

        public void Slashed(Vector3 dir)
        {
            m_rigidbody.AddForce(dir * 1000, ForceMode.Impulse);
            _ = SoundMaster.Instance.PlaySE(3); // Ža‚ç‚ê‚½SE
        }
    }
}
