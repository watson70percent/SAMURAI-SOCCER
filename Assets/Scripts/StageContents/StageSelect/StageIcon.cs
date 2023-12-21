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

        private int m_slashSE = 0; //斬撃SEの番号

        /// <summary>
        ///  Inspectorから設定するこのステージの番号
        /// </summary>
        public int StageNumber
        {
            get { return m_stageNumber; }
        }

        public StageState State { get; set; }

        private void Start()
        {
            //状態によってステージのアイコンを変更
            if (State == StageState.NotPlayable)
            {
                gameObject.SetActive(false);
            }
        }

        public void OnClick()
        {
            if (State == StageState.NotPlayable) { return; }
            SoundMaster.Instance.PlaySE(m_slashSE);
            StageSelectEvent.PreviewOnNext(m_stageNumber);
        }
    }
}