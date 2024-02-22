using UnityEngine;
using SamuraiSoccer.StageContents.Conversation;

namespace SamuraiSoccer.StageContents.StageSelect
{
    public class LastStageInvitation : MonoBehaviour
    {
        [SerializeField]
        private int m_lastStageSaveNumber;

        [SerializeField]
        private int m_lastStageConversationNumber;

        [SerializeField]
        private ConversationManager m_conversationManager;

        // Start is called before the first frame update
        private void Start()
        {
            InFileTransmitClient<SaveData> fileTransitClient = new InFileTransmitClient<SaveData>();
            if (fileTransitClient.TryGet(StorageKey.KEY_STAGENUMBER, out var save))
            {
                if (save.m_stageData == m_lastStageSaveNumber)
                {
                    _ = m_conversationManager.PlayConversation(m_lastStageConversationNumber, () => { });
                }
            }
        }
    }
}
