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
        [SerializeField]
        private GameObject m_scrollObject;
        [SerializeField]
        private float m_goalX;

        private Vector3 m_initPos;

        public Stage Stage { get { return m_stage; } }

        private void Start()
        {
            m_initPos = m_scrollObject.transform.localPosition;
        }

        public async UniTask Move()
        {
            m_scrollObjects.SetActive(true);
            await m_scrollScript.ScrollSlide(m_initPos.x, m_goalX, m_initPos.y, 1.0f);
        }

        public void ResetMove()
        {
            m_scrollScript.ResetObject(m_initPos.x);
            m_hidePanel.Update();
            m_scrollObjects.SetActive(false);
        }
    }
}
