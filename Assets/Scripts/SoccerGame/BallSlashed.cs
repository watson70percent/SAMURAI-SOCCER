using UnityEngine;

namespace SamuraiSoccer.SoccerGame
{
    public class BallSlashed : MonoBehaviour, ISlashed
    {
        [SerializeField]
        private Rigidbody m_rigidbody;

        [SerializeField]
        private float m_power = 1.0f;

        public void Slashed(Vector3 dir)
        {
            dir = new Vector3(dir.x, 0.0f, dir.z).normalized;
            dir = new Vector3(dir.x, 0.3f , dir.z);
            _ = SoundMaster.Instance.PlaySE(3); // Ža‚ç‚ê‚½SE
            m_rigidbody.AddForce(dir * m_power * 1000 / 60 , ForceMode.Impulse);
        }
    }
}
