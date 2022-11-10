using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using SamuraiSoccer.UI;
using UnityEngine.UI;
using System.Linq;

namespace SamuraiSoccer.StageContents.Conversation
{
    public class ConversationManager : MonoBehaviour
    {
        [SerializeField, Tooltip("会話文を表示するコンテンツ全体")]
        private GameObject m_conversationContents;

        [SerializeField]
        private Image m_rightCharacterImage;

        [SerializeField]
        private Image m_leftCharacterImage;

        [SerializeField]
        private Text m_rightCharacterNameText;

        [SerializeField]
        private Text m_leftCharacterNameText;

        [SerializeField]
        private Text m_conversationText;

        [SerializeField]
        private StageConversationDatas m_conversationDatas;

        [SerializeField]
        private ConversationCharacters m_conversationCharacters;

        [SerializeField]
        private UIFade m_uiFade;

        [SerializeField]
        private TextScroller m_textScroller;

        [SerializeField]
        private ScrollScript m_scrollScript;

        // Start is called before the first frame update
        public async void Start()
        {
            m_conversationContents.SetActive(false);
            await PlayConversation(12);
        }

        /// <summary>
        /// 会話コンテンツの起動
        /// </summary>
        /// <param name="conversationNum">会話番号</param>
        /// <returns></returns>
        public async UniTask PlayConversation(int conversationNum)
        {
            if (conversationNum > m_conversationDatas.ConversationDatas.Count)
            {
                Debug.LogError("会話番号が間違っているため会話が再生できません");
                return;
            }
            m_conversationContents.SetActive(true);
            await ConversationProcess(conversationNum);
            m_conversationContents.SetActive(false);
        }

        /// <summary>
        /// 会話分の表示と再生
        /// </summary>
        /// <param name="conversatioNum">再生する会話番号</param>
        /// <returns></returns>
        private async UniTask ConversationProcess(int conversatioNum)
        {
            await UniTask.Delay(1000);
            await m_scrollScript.ScrollSlide();
            SetCharacterInfo(conversatioNum);
            await m_uiFade.FadeInUI();
            ActiveTextUI(true);
            // 会話文の再生
            for (int i = 0; i < m_conversationDatas.ConversationDatas[conversatioNum].m_conversationTexts.Count; i++)
            {
                await m_textScroller.ShowText(m_conversationDatas.ConversationDatas[conversatioNum].m_conversationTexts[i].m_text);
                await UniTask.Delay(4000);
            }
            ActiveTextUI(false);
            await m_uiFade.FadeOutUI();
            // 巻物の移動
        }

        /// <summary>
        /// 会話キャラクター情報の追加
        /// </summary>
        /// <param name="conversationNum">会話番号</param>
        private void SetCharacterInfo(int conversationNum)
        {
            CharacterName leftCharacterName = m_conversationDatas.ConversationDatas[conversationNum].m_leftCharacterName;
            m_leftCharacterNameText.text = leftCharacterName.ToString();
            m_leftCharacterImage.sprite = m_conversationCharacters.m_conversationCharacters.Where(x => x.m_characterName == leftCharacterName).First().m_image;
            CharacterName rightCharacterName = m_conversationDatas.ConversationDatas[conversationNum].m_rightCharacterName;
            m_rightCharacterNameText.text = rightCharacterName.ToString();
            m_rightCharacterImage.sprite = m_conversationCharacters.m_conversationCharacters.Where(x => x.m_characterName == rightCharacterName).First().m_image;
        }

        /// <summary>
        /// 会話プレハブのテキスト部分のSetActiveを一括で実施する
        /// </summary>
        /// <param name="setActive"></param>
        private void ActiveTextUI(bool setActive)
        {
            m_conversationText.gameObject.SetActive(setActive);
            m_leftCharacterNameText.gameObject.SetActive(setActive);
            m_rightCharacterNameText.gameObject.SetActive(setActive);
        }
    }
}
