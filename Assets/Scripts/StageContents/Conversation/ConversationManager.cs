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
        [SerializeField, Tooltip("��b����\������R���e���c�S��")]
        private GameObject m_conversationContents;

        [SerializeField]
        private Image m_rightCharacterImage; //�E��b�L�����N�^�[�̉摜

        [SerializeField]
        private Image m_leftCharacterImage; //����b�L�����N�^�[�̉摜

        [SerializeField]
        private Text m_rightCharacterNameText; //�E��b�L�����N�^�[�̖��O�\���e�L�X�g

        [SerializeField]
        private Text m_leftCharacterNameText; //����b�L�����N�^�[�̖��O�\���e�L�X�g

        [SerializeField]
        private Text m_conversationText; //��b�e�L�X�g

        [SerializeField]
        private GameObject m_brushPen; //��b�ҋ@���̕M�y��

        [SerializeField]
        private GameObject m_scrollObject; //��b�p����

        private Vector3 m_initPos; //��b�p�����̏����ʒu

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

        private bool m_isTouched = false; //��ʂɐG�ꂽ���ǂ���

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
        /// ��b�R���e���c�̋N��
        /// </summary>
        /// <param name="conversationNum">��b�ԍ�</param>
        /// <returns></returns>
        public async UniTask PlayConversation(int conversationNum)
        {
            if (conversationNum > m_conversationDatas.ConversationDatas.Count)
            {
                Debug.LogError("��b�ԍ����Ԉ���Ă��邽�߉�b���Đ��ł��܂���");
                return;
            }
            m_conversationContents.SetActive(true);
            await ConversationProcess(conversationNum);
            m_conversationContents.SetActive(false);
        }

        /// <summary>
        /// ��b���̕\���ƍĐ�
        /// </summary>
        /// <param name="conversatioNum">�Đ������b�ԍ�</param>
        /// <returns></returns>
        private async UniTask ConversationProcess(int conversatioNum)
        {
            await UniTask.Delay(1000);
            await m_scrollScript.ScrollSlide(m_initPos.x, -m_initPos.x, m_initPos.y, 1.0f);
            SetCharacterInfo(conversatioNum);
            await m_uiFade.FadeInUI();
            ActiveTextUI(true);
            // ��b���̍Đ�
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
            // �����̈ړ�
        }

        /// <summary>
        /// ��b�L�����N�^�[���̒ǉ�
        /// </summary>
        /// <param name="conversationNum">��b�ԍ�</param>
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
        /// ��b�v���n�u�̃e�L�X�g������SetActive���ꊇ�Ŏ��{����
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
