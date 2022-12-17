using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiSoccer.StageContents.StageSelect
{
    public class StageStateInit : MonoBehaviour
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
                    icon.State = StageState.Cleared;
                }
                else if (icon.StageNumber == clearNumber)
                {
                    icon.State = StageState.Playable;
                }
                else
                {
                    icon.State = StageState.NotPlayable;
                }
            }
        }
    }
}
