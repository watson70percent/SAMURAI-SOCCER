using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiSoccer.StageContents.StageSelect
{
    public class StageSelectInit : MonoBehaviour
    {
        [SerializeField]
        private StageIcon[] m_stageIcon;
        private void Awake()
        {
            InFileTransmitClient<SaveData> fileTransitClient = new InFileTransmitClient<SaveData>();
            int clearNumber = fileTransitClient.Get(StorageKey.KEY_STAGENUMBER).m_stageData;
            foreach (StageIcon icon in m_stageIcon)
            {
                if (icon.StageNumber < clearNumber)
                {
                    icon.StageState = StageState.Cleared;
                }
                else if (icon.StageNumber == clearNumber)
                {
                    icon.StageState = StageState.Playable;
                }
                else
                {
                    icon.StageState = StageState.NotPlayable;
                }
            }
        }
    }
}
