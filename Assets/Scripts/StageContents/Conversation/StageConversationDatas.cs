using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiSoccer.StageContents.Conversation
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
        [Tooltip("話す人の名前")]
        public CharacterName m_characterName;
        [Tooltip("話す内容"), TextArea(1,5)]
        public string m_text;
    }

    [System.Serializable]
    public enum CharacterName
    {
        ショーグン,
        コクオー,
        ソーショキ,
        ダイトーリョー,
        シドーシャ,
    }
}
