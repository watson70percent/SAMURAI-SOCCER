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
        [SerializeField, Tooltip("会話文を表示するコンテンツ全体")]
        private GameObject m_conversationContents;

        [SerializeField, Tooltip("会話キャラクターの画像(左:0, 右:1)")]
        private Image[] m_characterImages = new Image[2];

        [SerializeField, Tooltip("会話キャラクターの名前(左:0, 右:1)")]
        private Text[] m_characterNameTexts = new Text[2];

        [SerializeField, Tooltip("会話キャラクターの名前が書かれたWindow(左:0, 右:1)")]
        private GameObject[] m_characterNameWindow = new GameObject[2];

        [SerializeField]
        private Text m_conversationText; //会話テキスト

        [SerializeField]
        private GameObject m_brushPen; //会話待機中の筆ペン
        [SerializeField]
        private List<Button> m_optionYesButtons; //はいの選択肢

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
        /// <param name="afterBGM">会話後に流すBGMの処理</param>
        /// <returns></returns>
        public async UniTask PlayConversation(int conversationNum, Action afterBGM)
        {
            if (conversationNum > m_conversationDatas.ConversationDatas.Count)
            {
                Debug.LogError("会話番号が間違っているため会話が再生できません");
                return;
            }
            m_conversationContents.SetActive(true);
            await ConversationProcess(conversationNum, afterBGM);
            m_conversationContents.SetActive(false);
        }

        /// <summary>
        /// 会話分の表示と再生
        /// </summary>
        /// <param name="conversationNum">再生する会話番号</param>
        /// <returns></returns>
        private async UniTask ConversationProcess(int conversationNum, Action afterBGM)
        {
            var beforeBGM = SoundMaster.Instance.BGMIndex;
            var conversationBGM = ConversationBGMMapping(conversationNum);
            if (conversationBGM != -1)
            {
                // 最終ステージのBGMでなければいったん止める。
                SoundMaster.Instance.StopSound();
            }
            await UniTask.Delay(1000);

            if (conversationBGM != -1)
            {
                SoundMaster.Instance.PlayBGM(ConversationBGMMapping(conversationNum));
            }
            // 巻物がスライドしてくる
            await m_scrollScript.ScrollSlide(m_initPos.x, -m_initPos.x, m_initPos.y, 1.0f);
            // 会話キャラの情報設定
            SetCharacterInfo(conversationNum);
            StageConversationData stageConversationData = m_conversationDatas.ConversationDatas[conversationNum];
            // 片方の名前Windowを表示
            if (stageConversationData.m_conversationTexts[0].m_characterName == stageConversationData.m_leftCharacterName)
            {
                // 左の名前だけ表示
                m_characterNameWindow[0].SetActive(true);
                m_characterNameWindow[1].SetActive(false);               
            }
            else if (stageConversationData.m_conversationTexts[0].m_characterName == stageConversationData.m_rightCharacterName)
            {
                // 右の名前だけ表示
                m_characterNameWindow[0].SetActive(false);
                m_characterNameWindow[1].SetActive(true);
            }
            else
            {
                Debug.LogError("会話しているキャラクター以外が会話の主として選択されているよ！");
            }
            // 会話UIが現れる
            await m_uiFade.FadeInUI();
            ActiveTextUI(true);
            // 会話文の再生
            for (int i = 0; i < stageConversationData.m_conversationTexts.Count; i++)
            {
                int speakerNum = 0;
                // 会話しているキャラクターの表情を変化させる
                if (stageConversationData.m_conversationTexts[i].m_characterName == stageConversationData.m_leftCharacterName)
                {
                    speakerNum = 0;
                    // 左の名前だけ表示
                    m_characterNameWindow[0].SetActive(true);
                    m_characterNameWindow[1].SetActive(false);
                    ChangeCharacterEmotion(m_characterImages[0], stageConversationData.m_conversationTexts[i].m_characterName, stageConversationData.m_conversationTexts[i].m_motionType);
                }
                else if (stageConversationData.m_conversationTexts[i].m_characterName == stageConversationData.m_rightCharacterName)
                {
                    speakerNum = 1;
                    // 右の名前だけ表示
                    m_characterNameWindow[0].SetActive(false);
                    m_characterNameWindow[1].SetActive(true);
                    ChangeCharacterEmotion(m_characterImages[1], stageConversationData.m_conversationTexts[i].m_characterName, stageConversationData.m_conversationTexts[i].m_motionType);
                }
                else
                {
                    Debug.LogError("会話しているキャラクター以外が会話の主として選択されているよ！");
                }
                Vector3 initPos = m_characterImages[speakerNum].transform.localPosition;
                CancellationTokenSource cts = new CancellationTokenSource();
                _ = HopImage(m_characterImages[speakerNum], initPos, cts.Token);
                await m_textScroller.ShowText(stageConversationData.m_conversationTexts[i].m_text);
                //選択肢の有無による分岐
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
                    // タイミング的にBGMが未設定またはステージセレクトのときはステージセレクト再生。
                    SoundMaster.Instance.PlayBGM(SoundMaster.STAGE_SELECT_BGM_INDEX);
                }
                // リザルトの時はここで上書き。
                afterBGM();
            }
            // 巻物の移動
        }

        /// <summary>
        /// 会話キャラクター情報の追加
        /// </summary>
        /// <param name="conversationNum">会話番号</param>
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
        /// 会話プレハブのテキスト部分のSetActiveを一括で実施する
        /// </summary>
        /// <param name="setActive"></param>
        private void ActiveTextUI(bool setActive)
        {
            m_conversationText.gameObject.SetActive(setActive);
            m_characterNameTexts[0].gameObject.SetActive(setActive);
            m_characterNameTexts[1].gameObject.SetActive(setActive);
        }

        /// <summary>
        /// 会話キャラクターの画像を適切な感情のものに設定する
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

            throw new ArgumentException("会話番号が想定されていないためBGMが再生できません");
        }
    }
}
