using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SamuraiSoccer.StageContents;

namespace SamuraiSoccer.StageContents.Result
{
    /// <summary>
    /// ゲームの結果を取得・表示
    /// </summary>
    public class ResultManager : MonoBehaviour
    {
        public GameResult ResultState { get; private set; } = GameResult.Undefined;
        [SerializeField] Text result;
        public string resultText;
        [SerializeField]
        Text samuraiPhrase;
        public SamuraiWordBase samuraiWordBase;

        public List<Text> texts;
        public Camera background;

        [SerializeField]
        private RectTransform retryButtonTransform;
        [SerializeField]
        private RectTransform nextButtonTransform;



        // Start is called before the first frame update
        void Start()
        {
            InMemoryDataTransitClient<GameResult> inMemoryDataTransitClient = new InMemoryDataTransitClient<GameResult>();
            ResultState = inMemoryDataTransitClient.Get(StorageKey.KEY_WINORLOSE);
            //勝敗に応じてテキスト変更
            switch (ResultState)
            {
                case GameResult.Win:
                    result.text = "勝利";
                    foreach (var txt in texts)
                    {
                        txt.color = Color.black;
                    }
                    background.backgroundColor = Color.white;
                    break;
                case GameResult.Lose:
                    result.text = "敗北";
                    //負けたときはボタンの位置を反転する(左右対称を仮定)
                    retryButtonTransform.anchoredPosition = new Vector2(-retryButtonTransform.anchoredPosition.x, retryButtonTransform.anchoredPosition.y);
                    nextButtonTransform.anchoredPosition = new Vector2(-nextButtonTransform.anchoredPosition.x, nextButtonTransform.anchoredPosition.y);
                    foreach (var txt in texts)
                    {
                        txt.color = Color.white;
                    }
                    background.backgroundColor = Color.black;
                    break;
                case GameResult.Draw:
                    result.text = "引分";
                    break;
            }

            //今日のひとこと
            samuraiPhrase.text = samuraiWordBase.samuraiwords[Random.Range(0, samuraiWordBase.samuraiwords.Count)];
        }
    }

}