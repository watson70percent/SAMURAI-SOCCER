using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;
using SamuraiSoccer.UI;
using System.Linq;
using System.Threading;

namespace SamuraiSoccer.StageContents.Conversation
{
    public class ConversationManager : MonoBehaviour
    {
        [SerializeField, Tooltip("��b����\������R���e���c�S��")]
        private GameObject m_conversationContents;

        [SerializeField, Tooltip("��b�L�����N�^�[�̉摜(��:0, �E:1)")]
        private Image[] m_characterImages = new Image[2];

        [SerializeField, Tooltip("��b�L�����N�^�[�̖��O(��:0, �E:1)")]
        private Text[] m_characterNameTexts = new Text[2];

        [SerializeField]
        private Text m_conversationText; //��b�e�L�X�g

        [SerializeField]
        private GameObject m_brushPen; //��b�ҋ@���̕M�y��
        [SerializeField]
        private Button m_optionYes; //�͂��̑I����

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
            StageConversationData stageConversationData = m_conversationDatas.ConversationDatas[conversatioNum];
            // ��b���̍Đ�
            for (int i = 0; i < stageConversationData.m_conversationTexts.Count; i++)
            {
                int speakerNum = 0;
                // ��b���Ă���L�����N�^�[�̕\���ω�������
                if (stageConversationData.m_conversationTexts[i].m_characterName == stageConversationData.m_leftCharacterName)
                {
                    speakerNum = 0;
                    ChangeCharacterEmotion(m_characterImages[0], stageConversationData.m_conversationTexts[i].m_characterName, stageConversationData.m_conversationTexts[i].m_motionType);
                }
                else if (stageConversationData.m_conversationTexts[i].m_characterName == stageConversationData.m_rightCharacterName)
                {
                    speakerNum = 1;
                    ChangeCharacterEmotion(m_characterImages[1], stageConversationData.m_conversationTexts[i].m_characterName, stageConversationData.m_conversationTexts[i].m_motionType);
                }
                else
                {
                    Debug.LogError("��b���Ă���L�����N�^�[�ȊO����b�̎�Ƃ��đI������Ă����I");
                }
                Vector3 initPos = m_characterImages[speakerNum].transform.localPosition;
                CancellationTokenSource cts = new CancellationTokenSource();
                _ = HopImage(m_characterImages[speakerNum], initPos, cts.Token);
                await m_textScroller.ShowText(stageConversationData.m_conversationTexts[i].m_text);
                //�I�����̗L���ɂ�镪��
                if (stageConversationData.m_conversationTexts[i].is_option)
                {
                    m_optionYes.transform.parent.gameObject.SetActive(true);
                    await m_optionYes.OnClickAsObservable().ToUniTask(useFirstValue:true);
                    m_optionYes.transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    m_brushPen.SetActive(true);
                    m_isTouched = false;
                    while (!m_isTouched)
                    {
                        await UniTask.Yield();
                    }
                }
                cts.Cancel();
                m_characterImages[speakerNum].transform.localPosition = initPos;
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
            m_characterNameTexts[0].text = leftCharacterName.ToString();
            m_characterImages[0].sprite = m_conversationCharacters.m_conversationCharacters.Where(x => x.m_characterName == leftCharacterName).First().m_imageSilhouette;
            CharacterName rightCharacterName = m_conversationDatas.ConversationDatas[conversationNum].m_rightCharacterName;
            m_characterNameTexts[1].text = rightCharacterName.ToString();
            m_characterImages[1].sprite = m_conversationCharacters.m_conversationCharacters.Where(x => x.m_characterName == rightCharacterName).First().m_imageSilhouette;
        }

        /// <summary>
        /// ��b�v���n�u�̃e�L�X�g������SetActive���ꊇ�Ŏ��{����
        /// </summary>
        /// <param name="setActive"></param>
        private void ActiveTextUI(bool setActive)
        {
            m_conversationText.gameObject.SetActive(setActive);
            m_characterNameTexts[0].gameObject.SetActive(setActive);
            m_characterNameTexts[1].gameObject.SetActive(setActive);
        }

        /// <summary>
        /// ��b�L�����N�^�[�̉摜��K�؂Ȋ���̂��̂ɐݒ肷��
        /// </summary>
        /// <param name="image"></param>
        /// <param name="name"></param>
        /// <param name="emotionType"></param>
        private void ChangeCharacterEmotion(Image image, CharacterName name, EmotionType emotionType)
        {
            ConversationCharacter character = m_conversationCharacters.m_conversationCharacters.Where(x => x.m_characterName == name).First();
            switch (emotionType)
            {
                case EmotionType.Normal:
                    image.sprite = character.m_imageNormal;
                    break;
                case EmotionType.Silhouette:
                    image.sprite = character.m_imageSilhouette;
                    break;
                case EmotionType.Funny:
                    image.sprite = character.m_imageFunny;
                    break;
                case EmotionType.Angry:
                    image.sprite = character.m_imageAngry;
                    break;
                case EmotionType.Sad:
                    image.sprite = character.m_imageSad;
                    break;
                default:
                    image.sprite = character.m_imageNormal;
                    break;
            }
        }

        private async UniTask HopImage(Image image, Vector3 initPos, CancellationToken cancellationToken = default)
        {
            float elapsedtime = 0;
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                elapsedtime += Time.deltaTime;
                image.transform.localPosition = new Vector3(initPos.x, initPos.y + 5 * (1 + Mathf.Sin(3 * Mathf.PI * elapsedtime)), initPos.z);
                await UniTask.Yield();
            }
        }
    }
}
