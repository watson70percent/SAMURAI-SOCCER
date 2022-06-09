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
            InNetworkTransmitClient<int> inNetworkTransmitClient = new InNetworkTransmitClient<int>();
            int clearNumber = inNetworkTransmitClient.Get("clearNumber");
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
