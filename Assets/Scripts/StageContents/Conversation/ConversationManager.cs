using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;
using SamuraiSoccer.UI;
using System.Linq;
using System.Threading;
using System;

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

        [SerializeField, Tooltip("��b�L�����N�^�[�̖��O�������ꂽWindow(��:0, �E:1)")]
        private GameObject[] m_characterNameWindow = new GameObject[2];

        [SerializeField]
        private Text m_conversationText; //��b�e�L�X�g

        [SerializeField]
        private GameObject m_brushPen; //��b�ҋ@���̕M�y��
        [SerializeField]
        private List<Button> m_optionYesButtons; //�͂��̑I����

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
        /// <param name="afterBGM">��b��ɗ���BGM�̏���</param>
        /// <returns></returns>
        public async UniTask PlayConversation(int conversationNum, Action afterBGM)
        {
            if (conversationNum > m_conversationDatas.ConversationDatas.Count)
            {
                Debug.LogError("��b�ԍ����Ԉ���Ă��邽�߉�b���Đ��ł��܂���");
                return;
            }
            m_conversationContents.SetActive(true);
            await ConversationProcess(conversationNum, afterBGM);
            m_conversationContents.SetActive(false);
        }

        /// <summary>
        /// ��b���̕\���ƍĐ�
        /// </summary>
        /// <param name="conversationNum">�Đ������b�ԍ�</param>
        /// <returns></returns>
        private async UniTask ConversationProcess(int conversationNum, Action afterBGM)
        {
            var beforeBGM = SoundMaster.Instance.BGMIndex;
            var conversationBGM = ConversationBGMMapping(conversationNum);
            if (conversationBGM != -1)
            {
                // �ŏI�X�e�[�W��BGM�łȂ���΂�������~�߂�B
                SoundMaster.Instance.StopSound();
            }
            await UniTask.Delay(1000);

            if (conversationBGM != -1)
            {
                SoundMaster.Instance.PlayBGM(ConversationBGMMapping(conversationNum));
            }
            // �������X���C�h���Ă���
            await m_scrollScript.ScrollSlide(m_initPos.x, -m_initPos.x, m_initPos.y, 1.0f);
            // ��b�L�����̏��ݒ�
            SetCharacterInfo(conversationNum);
            StageConversationData stageConversationData = m_conversationDatas.ConversationDatas[conversationNum];
            // �Е��̖��OWindow��\��
            if (stageConversationData.m_conversationTexts[0].m_characterName == stageConversationData.m_leftCharacterName)
            {
                // ���̖��O�����\��
                m_characterNameWindow[0].SetActive(true);
                m_characterNameWindow[1].SetActive(false);               
            }
            else if (stageConversationData.m_conversationTexts[0].m_characterName == stageConversationData.m_rightCharacterName)
            {
                // �E�̖��O�����\��
                m_characterNameWindow[0].SetActive(false);
                m_characterNameWindow[1].SetActive(true);
            }
            else
            {
                Debug.LogError("��b���Ă���L�����N�^�[�ȊO����b�̎�Ƃ��đI������Ă����I");
            }
            // ��bUI�������
            await m_uiFade.FadeInUI();
            ActiveTextUI(true);
            // ��b���̍Đ�
            for (int i = 0; i < stageConversationData.m_conversationTexts.Count; i++)
            {
                int speakerNum = 0;
                // ��b���Ă���L�����N�^�[�̕\���ω�������
                if (stageConversationData.m_conversationTexts[i].m_characterName == stageConversationData.m_leftCharacterName)
                {
                    speakerNum = 0;
                    // ���̖��O�����\��
                    m_characterNameWindow[0].SetActive(true);
                    m_characterNameWindow[1].SetActive(false);
                    ChangeCharacterEmotion(m_characterImages[0], stageConversationData.m_conversationTexts[i].m_characterName, stageConversationData.m_conversationTexts[i].m_motionType);
                }
                else if (stageConversationData.m_conversationTexts[i].m_characterName == stageConversationData.m_rightCharacterName)
                {
                    speakerNum = 1;
                    // �E�̖��O�����\��
                    m_characterNameWindow[0].SetActive(false);
                    m_characterNameWindow[1].SetActive(true);
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
                    m_optionYesButtons.ForEach(button => button.transform.parent.gameObject.SetActive(true));
                    var result = await Observable.Merge(
                        m_optionYesButtons.Select(button => button.OnClickAsObservable().Select(_ => button.name))
                    )
                    .ToUniTask(useFirstValue: true);
                    m_optionYesButtons.ForEach(button => button.transform.parent.gameObject.SetActive(false));
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
            await m_scrollScript.ScrollSlide(-m_initPos.x, m_initPos.x, m_initPos.y, 1.0f);
            if (conversationBGM != -1)
            {
                SoundMaster.Instance.StopSound();
            }
            await UniTask.Delay(500);

            if (conversationBGM != -1)
            {
                if (beforeBGM == -1 || beforeBGM == SoundMaster.STAGE_SELECT_BGM_INDEX)
                {
                    // �^�C�~���O�I��BGM�����ݒ�܂��̓X�e�[�W�Z���N�g�̂Ƃ��̓X�e�[�W�Z���N�g�Đ��B
                    SoundMaster.Instance.PlayBGM(SoundMaster.STAGE_SELECT_BGM_INDEX);
                }
                // ���U���g�̎��͂����ŏ㏑���B
                afterBGM();
            }
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
            if(rightCharacterName == CharacterName.xxx)
            {
                m_characterNameTexts[1].text = "";
            }
            else
            {
                m_characterNameTexts[1].text = rightCharacterName.ToString();
            }
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

        private int ConversationBGMMapping(int conversation)
        {
            if (conversation >= 0 && conversation <= 3)
            {
                return 15;
            }

            if (conversation >= 4 && conversation <= 7)
            {
                return 16;
            }

            if (conversation >= 8 && conversation <= 11)
            {
                return 17;
            }

            if (conversation >= 12 && conversation <= 15)
            {
                return 18;
            }

            if (conversation >= 16 && conversation <= 18)
            {
                return 19;
            }

            if (conversation >= 30 && conversation <= 31)
            {
                return -1;
            }

            throw new ArgumentException("��b�ԍ����z�肳��Ă��Ȃ�����BGM���Đ��ł��܂���");
        }
    }
}
