using UnityEngine;
using SamuraiSoccer;

namespace SamuraiSoccer.UI
{
    public class Credit : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_creditObject;

        public void ShowCreditObject()
        {
            SoundMaster.Instance.PlaySE(0);
            m_creditObject.SetActive(true);
        }

        public void CloseCreditObject()
        {
            SoundMaster.Instance.PlaySE(0);
            m_creditObject.SetActive(false);
        }
    }
}
