using SamuraiSoccer.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SamuraiSoccer.StageContents.StageSelect
{
    /// <summary>
    /// ステージの状態
    /// NotPlayable:プレイ不可
    /// Playable:プレイ可能
    /// Cleared:クリア済み
    /// </summary>
    public enum StageState
    {
        NotPlayable,
        Playable,
        Cleared
    }

    public class StageIcon : MonoBehaviour
    {
        [SerializeField]
        private int m_stageNumber;

        [SerializeField]
        private Image m_image;

        [SerializeField]
        private Sprite m_monoImage;

        /// <summary>
        ///  Inspectorから設定するこのステージの番号
        /// </summary>
        public int StageNumber
        {
            get { return m_stageNumber; }
        }

        /// <summary>
        /// このステージの状態
        /// </summary>
        public StageState StageState
        {
            get;
            set;
        }

        private void Start()
        {
            //状態によってステージのアイコンを変更
            if (StageState == StageState.NotPlayable)
            {
                m_image.sprite = m_monoImage;
            }
        }

        public void OnClick()
        {
            StageSelectEvent.PreviewOnNext(m_stageNumber);
        }
    }
}