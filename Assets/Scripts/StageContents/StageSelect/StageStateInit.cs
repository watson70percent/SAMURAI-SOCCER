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
            int clearNumber;
            if (fileTransitClient.TryGet(StorageKey.KEY_STAGENUMBER, out var saveData))
            {
                clearNumber = saveData.m_stageData;
            }
            else
            {
                clearNumber = 0;
                SaveData data = new SaveData();
                data.m_stageData = clearNumber;
                fileTransitClient.Set(StorageKey.KEY_STAGENUMBER, data);
            }
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
