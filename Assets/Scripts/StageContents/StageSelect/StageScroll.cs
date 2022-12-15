using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SamuraiSoccer.Event;
using UnityEngine.UI;

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
        public Stage Stage
        {
            get { return m_stage; }
        }


        bool m_isPlayable = false;
        private void Start()
        {

            

            InFileTransmitClient<SaveData> stageNumberTransitionClient = new InFileTransmitClient<SaveData>();
            int stageNumber = stageNumberTransitionClient.Get(StorageKey.KEY_STAGENUMBER).m_stageData;

            
            m_isPlayable = stageNumber/3 + 1 >= (int)m_stage;
            if (m_stage == Stage.Japan) m_isPlayable = true;
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
            print("clicked!");
        }
    }
}
