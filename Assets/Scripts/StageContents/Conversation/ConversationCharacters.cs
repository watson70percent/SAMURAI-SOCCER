using SamuraiSoccer.StageContents.Conversation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ConversationCharacters : ScriptableObject
{
    public List<ConversationCharacter> m_conversationCharacters = new List<ConversationCharacter>();
}

[System.Serializable]
public class ConversationCharacter
{
    public CharacterName m_characterName;
    public Sprite m_image;
}
