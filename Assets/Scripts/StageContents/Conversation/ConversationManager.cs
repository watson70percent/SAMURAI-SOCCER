using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;
using SamuraiSoccer.UI;
using System.Linq;

namespace SamuraiSoccer.StageContents.Conversation
{
    public class ConversationManager : MonoBehaviour
    {
        [SerializeField, Tooltip("会話文を表示するコンテンツ全体")]
        private GameObject m_conversationContents;

        [SerializeField]
        private Image m_rightCharacterImage; //右会話キャラクターの画像

        [SerializeField]
        private Image m_leftCharacterImage; //左会話キャラクターの画像

        [SerializeField]
        private Text m_rightCharacterNameText; //右会話キャラクターの名前表示テキスト

        [SerializeField]
        private Text m_leftCharacterNameText; //左会話キャラクターの名前表示テキスト

        [SerializeField]
        private Text m_conversationText; //会話テキスト

        [SerializeField]
        private GameObject m_brushPen; //会話待機中の筆ペン

        [SerializeField]
        private GameObject m_scrollObject; //会話用巻物

        private Vector3 m_initPos; //会話用巻物の初期位置

        [SerializeField]
        private StageConversationDatas m_conversationDatas;

        [SerializeField]
        private ConversationCharacters m_conversationCharacters;

        [SerializeField]
        private UIFade m_uiFade;

        [SerializeField]
        private TouchProvider m_provider;

        [SerializeField]
        private TextScroller m_textScroller;

        [SerializeField]
        private ScrollScript m_scrollScript;

        private bool m_isTouched = false; //画面に触れたかどうか

        // Start is called before the first frame update
        public void Start()
        {
            m_conversationContents.SetActive(false);
            m_provider.IsTouchingReactiveProperty.Where(b => b).Subscribe(_ => 
            { 
                m_isTouched = true; 
            }).AddTo(this);

            m_initPos = m_scrollObject.transform.localPosition;
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
            await m_scrollScript.ScrollSlide(m_initPos.x, -m_initPos.x, m_initPos.y, 1.0f);
            SetCharacterInfo(conversatioNum);
            await m_uiFade.FadeInUI();
            ActiveTextUI(true);
            // 会話文の再生
            for (int i = 0; i < m_conversationDatas.ConversationDatas[conversatioNum].m_conversationTexts.Count; i++)
            {
                await m_textScroller.ShowText(m_conversationDatas.ConversationDatas[conversatioNum].m_conversationTexts[i].m_text);
                m_brushPen.SetActive(true);
                m_isTouched = false;
                while (!m_isTouched)
                {
                    await UniTask.Yield();
                }
                m_brushPen.SetActive(false);
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
