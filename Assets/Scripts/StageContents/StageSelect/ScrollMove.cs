using UnityEngine;
using Cysharp.Threading.Tasks;
using SamuraiSoccer.Event;
using SamuraiSoccer.UI;
using System.Threading;

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

        private CancellationTokenSource m_cts;

        public Stage Stage { get { return m_stage; } }

        private void Start()
        {
            m_initPos = m_scrollObject.transform.localPosition;
            m_cts = new CancellationTokenSource();
        }

        public async UniTask Move()
        {
            m_scrollObjects.SetActive(true);
            m_scrollScript.Start();
            m_scrollScript.ChangeMaterial(m_stage);
            await m_scrollScript.ScrollSlide(m_initPos.x, m_goalX, m_initPos.y, 1.0f, m_cts.Token);
            m_hidePanel.gameObject.SetActive(false);
        }

        public void ResetMove()
        {
            m_cts.Cancel();
            m_scrollScript.ResetObject(m_initPos.x);
            m_hidePanel.Update();
            m_hidePanel.gameObject.SetActive(true);
            m_scrollObjects.SetActive(false);
            m_cts = new CancellationTokenSource();
        }
    }
}
