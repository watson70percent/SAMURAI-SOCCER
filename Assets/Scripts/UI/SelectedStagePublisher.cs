using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SamuraiSoccer.Event;

namespace SamuraiSoccer.UI
{
    public class SelectedStagePublisher : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("選択したステージの国がどこかを設定")]
        private Stage m_stage;

        /// <summary>
        /// 選択したステージをもとに状態遷移を発行
        /// </summary>
        public void OnClick()
        {
            StageSelectEvent.StageOnNext(m_stage);
        }
    }
}
