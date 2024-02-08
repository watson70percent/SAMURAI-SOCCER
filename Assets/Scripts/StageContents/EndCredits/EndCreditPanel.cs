using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiSoccer.StageContents.EndCredits
{
    public class EndCreditPanel : MonoBehaviour
    {

        [SerializeField]
        private int m_invincibleTime = 2;

        // Start is called before the first frame update
        void Start()
        {
            GetComponent<MeshCollider>().isTrigger = true;
            Invoke("CancelInvincible", m_invincibleTime);
        }

        void CancelInvincible()
        {
            GetComponent<MeshCollider>().isTrigger = false;
        }
    }
}
