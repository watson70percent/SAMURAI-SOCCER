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
        [Tooltip("�b���l�̖��O")]
        public CharacterName m_characterName;
        [Tooltip("�b�����e"), TextArea(1,5)]
        public string m_text;
    }

    [System.Serializable]
    public enum CharacterName
    {
        �V���[�O��,
        �R�N�I�[,
        �\�[�V���L,
        �_�C�g�[�����[,
        �V�h�[�V��,
    }
}
