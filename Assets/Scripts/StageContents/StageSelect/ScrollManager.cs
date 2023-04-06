using SamuraiSoccer.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace SamuraiSoccer.StageContents.StageSelect
{
    public class ScrollManager : MonoBehaviour
    {
        public List<ScrollMove> m_scrollMoves = new List<ScrollMove>();

        [SerializeField]
        private WorldMapMove m_worldMapMove;

        [SerializeField]
        private StageSelectBGM m_stageSelectBGM;

        private Stage m_currentStage;

        // Start is called before the first frame update
        void Start()
        {
            StageSelectEvent.Stage.Subscribe(stage =>
            {
                for (int i=0; i<m_scrollMoves.Count; i++)
                {
                    if (m_scrollMoves[i].Stage == stage)
                    {
                        _ = m_scrollMoves[i].Move();
                    }
                }
            }).AddTo(this);

            StageScrollScroller.SelectedStage.Where(x => x != m_currentStage).Subscribe(async stage =>
            {
                await m_worldMapMove.GoTo(stage);
                m_stageSelectBGM.ChangeBGM(stage, 1.0f);
            }).AddTo(this);
        }
    }
}
