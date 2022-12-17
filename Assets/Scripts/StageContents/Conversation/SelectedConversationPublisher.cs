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
            // ��x��b�C�x���g������������V�[�����؂�ւ��Ȃ�����ēx��b���������Ȃ��悤�ɂ���
            if (!m_finishedConversation)
            {
                InFileTransmitClient<SaveData> fileTransitClient = new InFileTransmitClient<SaveData>();
                int clearNumber = fileTransitClient.Get(StorageKey.KEY_STAGENUMBER).m_stageData;
                // �Y����b�ԍ� == �N���A�ԍ�(������-1)��3�Ŋ������]��*4
                if (m_conversationNum != clearNumber%3*4)
                {
                    return;
                }
                _ = m_conversationManager.PlayConversation(m_conversationNum);
                m_finishedConversation = true;
            }
        }
    }
}