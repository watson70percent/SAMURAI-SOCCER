using SamuraiSoccer.StageContents.Conversation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiSoccer.StageContents.Conversation
{
    [CreateAssetMenu]
    public class ConversationCharacters : ScriptableObject
    {
        public List<ConversationCharacter> m_conversationCharacters = new List<ConversationCharacter>();
    }

    [System.Serializable]
    public class ConversationCharacter
    {
        public CharacterName m_characterName;
        public Sprite m_imageNormal;
        public Sprite m_imageSilhouette;
        public Sprite m_imageFunny;
        public Sprite m_imageAngry;
        public Sprite m_imageSad;
    }
}


