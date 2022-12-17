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
        public CharacterName m_leftCharacterName;
        public CharacterName m_rightCharacterName;
        public List<ConversationText> m_conversationTexts = new List<ConversationText>();
    }

    [System.Serializable]
    public class ConversationText 
    {
        [Tooltip("�b���l�̖��O")]
        public CharacterName m_characterName;
        [Tooltip("�b����̊���")]
        public EmotionType m_motionType;
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

    [System.Serializable]
    public enum EmotionType
    {
        Normal,
        Silhouette,
        Funny,
        Angry,
        Sad,
    }
}
