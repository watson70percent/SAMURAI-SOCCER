using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiSoccer.StageContents
{
    [CreateAssetMenu]
    public class StageConversationDatas : ScriptableObject
    {
        public List<StageConversationData> ConversationDatas = new List<StageConversationData>();
    }

    [System.Serializable]
    public class StageConversationData
    {
        public List<ConversationText> m_conversationTexts = new List<ConversationText>();
    }

    [System.Serializable]
    public class ConversationText 
    {
        [Tooltip("�b���l�̖��O")]
        public string m_characterName;
        [Tooltip("�b�����e"), TextArea(1,5)]
        public string m_text;
    }

}
