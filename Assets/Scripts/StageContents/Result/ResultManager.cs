using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SamuraiSoccer.StageContents;
using Cysharp.Threading.Tasks;
using SamuraiSoccer.StageContents.Conversation;

namespace SamuraiSoccer.StageContents.Result
{
    /// <summary>
    /// ゲームの結果を取得・表示
    /// </summary>
    public class ResultManager : MonoBehaviour
    {
        public GameResult ResultState { get; private set; } = GameResult.Undefined;
        [SerializeField]
        private Text result;
        [SerializeField]
        private Text samuraiPhrase;
        [SerializeField]
        private ResultBGM resultBGM;
        [SerializeField]
        private SamuraiWordBase samuraiWordBase;
        [SerializeField]
        private StageDataSave stageDataSave;
        [SerializeField]
        private ConversationManager conversationManager;
        [SerializeField]
        private List<Text> texts;
        [SerializeField]
        private Camera mainCamera;

        [SerializeField]
        private int lastStageNum;
        [SerializeField]
        private RectTransform retryButtonTransform;
        [SerializeField]
        private RectTransform nextButtonTransform;
        [SerializeField]
        private RectTransform lastButtonTransform;

        // Start is called before the first frame update
        private void Start()
        {
            InMemoryDataTransitClient<GameResult> inMemoryDataTransitClient = new InMemoryDataTransitClient<GameResult>();
            ResultState = inMemoryDataTransitClient.Get(StorageKey.KEY_WINORLOSE);
            //勝敗に応じてテキスト変更
            switch (ResultState)
            {
                case GameResult.Win:
                    Win();
                    break;
                case GameResult.Lose:
                    Lose();
                    break;
                case GameResult.Draw:
                    result.text = "引分";
                    break;
            }
            //今日のひとこと
            samuraiPhrase.text = samuraiWordBase.samuraiwords[Random.Range(0, samuraiWordBase.samuraiwords.Count)];
        }

        /// <summary>
        /// 勝った時は白地に黒文字になってセーブ処理が発生する
        /// </summary>
        private void Win()
        {
            result.text = "勝利";
            foreach (var txt in texts)
            {
                txt.color = Color.black;
            }
            mainCamera.backgroundColor = Color.white;
            InMemoryDataTransitClient<int> stageNumberTransitionClient = new InMemoryDataTransitClient<int>();
            int clearNumber = stageNumberTransitionClient.Get(StorageKey.KEY_STAGENUMBER);
            if (stageDataSave.Save(clearNumber))
            {
                // ステージ番号に対応したお話を開始
                // お話の番号=クリアしたステージ番号を3で割った商×4+ステージ番号を3で割った余り+1
                conversationManager.PlayConversation((clearNumber / 3) * 4 + clearNumber % 3 + 1, () => resultBGM.PlayWinBGM().Forget()).Forget();
            }
            else
            {
                resultBGM.PlayWinBGM().Forget();
            }
            // 再戦用に再びKEY_STAGENUMBERをセットする
            stageNumberTransitionClient.Set(StorageKey.KEY_STAGENUMBER, clearNumber);
            if (clearNumber == lastStageNum)
            {
                retryButtonTransform.gameObject.SetActive(false);
                nextButtonTransform.gameObject.SetActive(false);
                lastButtonTransform.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 負けた時は黒地に白文字になる
        /// </summary>
        private void Lose()
        {
            result.text = "敗北";
            //負けたときはボタンの位置を反転する(左右対称を仮定)
            retryButtonTransform.anchoredPosition = new Vector2(-retryButtonTransform.anchoredPosition.x, retryButtonTransform.anchoredPosition.y);
            nextButtonTransform.anchoredPosition = new Vector2(-nextButtonTransform.anchoredPosition.x, nextButtonTransform.anchoredPosition.y);
            foreach (var txt in texts)
            {
                txt.color = Color.white;
            }
            mainCamera.backgroundColor = Color.black;
            resultBGM.PlayLoseBGM().Forget();
        }
    }
}