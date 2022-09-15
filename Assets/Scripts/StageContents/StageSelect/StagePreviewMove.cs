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
        private StagePreviewDatas m_stagePreviewDatas; //�S�X�e�[�W�̏ڍ׏�񂪓�����ScriptableObject
        [SerializeField]
        private GameObject m_previewObject; //Preview��\��������Object
        [SerializeField]
        private Text m_stageName; //Preview�̃X�e�[�W��
        [SerializeField]
        private Text m_stageSummary; //Preview�̃X�e�[�W���
        [SerializeField]
        private Image m_stageImage; //Preview�̃X�e�[�W�摜

        private StagePreviewData m_currentStagePreviewData; //���ݕ\�����Ă���X�e�[�W�f�[�^

        private int m_slashSE = 0;

        private void Start()
        {
            StageSelectEvent.Preview.Subscribe(x =>
            {
                //ScriptableObject���獡��̃X�e�[�W���擾���A�\����֑��
                m_currentStagePreviewData = m_stagePreviewDatas.stageSelectList.Where(num => num.stageNumber == x).First();
                m_stageName.text = m_currentStagePreviewData.previewName;
                m_stageSummary.text = m_currentStagePreviewData.summary;
                m_stageImage.sprite = m_currentStagePreviewData.stageImage;
                m_previewObject.SetActive(true);
            });
        }

        /// <summary>
        /// �߂�{�^����Preview�\�������
        /// </summary>
        public void Close()
        {
            SoundMaster.Instance.PlaySE(m_slashSE);
            m_previewObject.SetActive(false);
        }

        /// <summary>
        /// �J��{�^���Ŏ���Scene�֑J��
        /// </summary>
        public void StartGame()
        {
            SoundMaster.Instance.PlaySE(m_slashSE);
            //����Scene�ɕK�v�f�[�^�̓]��
            InMemoryDataTransitClient<int> fieldNumberTransitClient = new InMemoryDataTransitClient<int>();
            fieldNumberTransitClient.Set(StorageKey.KEY_FIELDNUMBER, m_currentStagePreviewData.fieldNumber);
            fieldNumberTransitClient.Set(StorageKey.KEY_STAGENUMBER, m_currentStagePreviewData.stageNumber);
            InMemoryDataTransitClient<string> opponentNameTransmitClient = new InMemoryDataTransitClient<string>();
            opponentNameTransmitClient.Set(StorageKey.KEY_OPPONENT_TYPE, m_currentStagePreviewData.opponentType);
            SceneManager.LoadScene(m_currentStagePreviewData.gameScene);
        }
    }
}
