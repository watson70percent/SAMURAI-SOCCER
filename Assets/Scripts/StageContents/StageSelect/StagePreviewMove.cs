using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SamuraiSoccer.Event;
using UniRx;

namespace SamuraiSoccer.StageContents.StageSelect
{
    public class StagePreviewMove : MonoBehaviour
    {
        [SerializeField]
        private StagePreviewDatas m_stagePreviewDatas; //全ステージの詳細情報が入ったScriptableObject
        [SerializeField]
        private GameObject m_previewObject; //Previewを表示させるObject
        [SerializeField]
        private Text m_stageName; //Previewのステージ名
        [SerializeField]
        private Text m_stageSummary; //Previewのステージ情報
        [SerializeField]
        private Image m_stageImage; //Previewのステージ画像

        private StagePreviewData m_currentStagePreviewData; //現在表示しているステージデータ

        private int m_slashSE = 0;

        private void Start()
        {
            StageSelectEvent.Preview.Subscribe(x =>
            {
                //ScriptableObjectから今回のステージを取得し、表示器へ代入
                m_currentStagePreviewData = m_stagePreviewDatas.stageSelectList.Where(num => num.stageNumber == x).First();
                m_stageName.text = m_currentStagePreviewData.previewName;
                m_stageSummary.text = m_currentStagePreviewData.summary;
                m_stageImage.sprite = m_currentStagePreviewData.stageImage;
                m_previewObject.SetActive(true);
            });
        }

        /// <summary>
        /// 戻るボタンでPreview表示を閉じる
        /// </summary>
        public void Close()
        {
            SoundMaster.Instance.PlaySE(m_slashSE);
            m_previewObject.SetActive(false);
        }

        /// <summary>
        /// 開戦ボタンで試合Sceneへ遷移
        /// </summary>
        public void StartGame()
        {
            SoundMaster.Instance.PlaySE(m_slashSE);
            //試合Sceneに必要データの転送
            InMemoryDataTransitClient<int> fieldNumberTransitClient = new InMemoryDataTransitClient<int>();
            fieldNumberTransitClient.Set(StorageKey.KEY_FIELDNUMBER, m_currentStagePreviewData.fieldNumber);
            fieldNumberTransitClient.Set(StorageKey.KEY_STAGENUMBER, m_currentStagePreviewData.stageNumber);
            InMemoryDataTransitClient<string> opponentNameTransmitClient = new InMemoryDataTransitClient<string>();
            opponentNameTransmitClient.Set(StorageKey.KEY_OPPONENT_TYPE, m_currentStagePreviewData.opponentType);
            SceneManager.LoadScene(m_currentStagePreviewData.gameScene);
        }
    }
}
