using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SamuraiSoccer.Event;
using SamuraiSoccer.StageContents.Conversation;
using SamuraiSoccer.UI;

namespace SamuraiSoccer.StageContents.StageSelect
{
    public class StageScroll : MonoBehaviour
    {
        [SerializeField]
        Stage m_stage;

        [SerializeField]
        RectTransform m_viewPort;

        [SerializeField] Image m_image;
        [SerializeField] RectTransform m_imageRect;

        [SerializeField]
        private ScrollIcon m_scrollIcon;
        [SerializeField]
        private SelectedStagePublisher m_publisher;
        [SerializeField]
        private SelectedConversationPublisher m_conversationPublisher;

        public Stage Stage
        {
            get { return m_stage; }
        }


        private void Update()
        {
            float xpos = GetComponent<RectTransform>().position.x;
            float centrality=Mathf.Abs(xpos-m_viewPort.position.x);
            float coef = Mathf.Exp(-centrality * centrality / 600000.0f);
            
            m_imageRect.localScale = Vector3.one * Mathf.Max(coef,0.5f);
            
        }

        public void OnClick()
        {
            if (m_scrollIcon.State == StageState.NotPlayable || Stage != StageScrollScroller.SelectedStage.Value)
            {
                return;
            }
            m_publisher.OnClick();
            if (m_conversationPublisher!=null)
            {
                m_conversationPublisher.Onclick();
            }
        }
    }
}
