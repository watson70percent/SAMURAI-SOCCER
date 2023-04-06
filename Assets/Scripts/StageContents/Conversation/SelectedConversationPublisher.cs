using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SamuraiSoccer;

namespace SamuraiSoccer.StageContents.Conversation
{
    public class SelectedConversationPublisher : MonoBehaviour
    {
        [SerializeField]
        private int m_conversationNum;

        [SerializeField]
        private ConversationManager m_conversationManager;

        private bool m_finishedConversation = false;

        public void Onclick()
        {
            // 一度会話イベントが発生したらシーンが切り替わらない限り再度会話が発生しないようにする
            if (!m_finishedConversation)
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
                // 該当会話番号 == クリア番号(初期は-1)を3で割った余り*4
                if (m_conversationNum != (clearNumber / 3 + clearNumber % 3) * 4)
                {
                    return;
                }
                _ = m_conversationManager.PlayConversation(m_conversationNum);
                m_finishedConversation = true;
            }
        }
    }
}
