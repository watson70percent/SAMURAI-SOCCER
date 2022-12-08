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

        // Start is called before the first frame update
        void Start()
        {
            StageSelectEvent.Stage.Subscribe(async stage =>
            {
                for (int i=0; i<m_scrollMoves.Count; i++)
                {
                    if (m_scrollMoves[i].Stage == stage)
                    {
                        _ = m_scrollMoves[i].Move();
                    }
                }
            }).AddTo(this);
        }
    }
}
