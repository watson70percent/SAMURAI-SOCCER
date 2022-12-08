using UnityEngine;
using Cysharp.Threading.Tasks;
using SamuraiSoccer.Event;
using SamuraiSoccer.UI;

namespace SamuraiSoccer.StageContents.StageSelect
{
    public class ScrollMove : MonoBehaviour
    {
        [SerializeField]
        private Stage m_stage;
        [SerializeField]
        private ScrollScript m_scrollScript;
        [SerializeField]
        private HidePanel m_hidePanel;
        [SerializeField]
        private GameObject m_scrollObjects;

        public Stage Stage { get { return m_stage; } }

        public async UniTask Move()
        {
            m_scrollObjects.SetActive(true);
            await m_scrollScript.ScrollSlide();
        }

        public void ResetMove()
        {
            m_scrollScript.ResetObject();
            m_hidePanel.Update();
            m_scrollObjects.SetActive(false);
        }
    }
}
