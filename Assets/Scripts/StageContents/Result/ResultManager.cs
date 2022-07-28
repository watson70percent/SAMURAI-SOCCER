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
    public class ResultManager : MonoBehaviour, StageDataReceiver
    {
        public BaseStageData NowStageData { get; private set; } = null;
        public void StageDataReceive(BaseStageData stageData)
        {
            NowStageData = stageData;
            Debug.Log("Set stage data : worldName = " + NowStageData.WorldName + ", StageNumber = " + NowStageData.StageNumber);
        }

        public GameResult ResultState { get; private set; } = Result.Undefined;
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
            //勝敗に応じてテキスト変更
            switch (ResultState)
            {
                case GameResult.Win:
                    result.text = "勝利";
                    break;
                case GameResult.Lose:
                    result.text = "敗北";
                    //負けたときはボタンの位置を反転する(左右対称を仮定)
                    retryButtonTransform.anchoredPosition = new Vector2(-retryButtonTransform.anchoredPosition.x, retryButtonTransform.anchoredPosition.y);
                    nextButtonTransform.anchoredPosition = new Vector2(-nextButtonTransform.anchoredPosition.x, nextButtonTransform.anchoredPosition.y);
                    break;
                case GameResult.Draw:
                    result.text = "引分";
                    break;
            }

            //今日のひとこと
            samuraiPhrase.text = samuraiWordBase.samuraiwords[Random.Range(0, samuraiWordBase.samuraiwords.Count)];
            Debug.Log("Now data : worldName = " + NowStageData.WorldName + ", StageNumber = " + NowStageData.StageNumber + ", Result = " + ResultState);
        }

        public void SetResult(GameResult resultState, string resultText)
        {
            this.ResultState = resultState;
            this.resultText = resultText;
            if (resultState == GameResult.Win)
            {
                foreach (var txt in texts)
                {
                    txt.color = Color.black;
                }
                background.backgroundColor = Color.white;
            }
            else
            {
                foreach (var txt in texts)
                {
                    txt.color = Color.white;
                }
                background.backgroundColor = Color.black;
            }
        }
    }

}