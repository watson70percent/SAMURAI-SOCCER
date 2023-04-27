using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiSoccer.SoccerGame
{
    public class BallSlashed : MonoBehaviour, ISlashed
    {
        [SerializeField]
        private Rigidbody m_rigidbody;

        public void Slashed(Vector3 dir)
        {
            dir = dir.normalized;
            dir = new Vector3(dir.x, 0.3f , dir.z);
            _ = SoundMaster.Instance.PlaySE(3); // Ža‚ç‚ê‚½SE
            m_rigidbody.AddForce(dir * 1000 / 60 , ForceMode.Impulse);
        }
    }
}
