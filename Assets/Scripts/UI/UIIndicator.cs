using UnityEngine;

namespace SamuraiSoccer.UI
{
    public class UIIndicator : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_uiObject;

        public void ShowUIObject()
        {
            SoundMaster.Instance.PlaySE(0);
            m_uiObject.SetActive(true);
        }

        public void CloseUIObject()
        {
            SoundMaster.Instance.PlaySE(0);
            m_uiObject.SetActive(false);
        }
    }
}
