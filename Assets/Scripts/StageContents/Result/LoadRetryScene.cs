using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SamuraiSoccer.StageContents.StageSelect;
using System.Linq;

namespace SamuraiSoccer.StageContents.Result
{
    [RequireComponent(typeof(ResultManager))]
    public class LoadRetryScene : MonoBehaviour
    {
        [SerializeField]
        private StagePreviewDatas m_stagePreviewDatas;
        /// <summary>
        /// 今遊んだSceneをStagePreviewDatasから取得しそのSceneへ移動
        /// </summary>
        public void LoadScene()
        {
            SoundMaster.Instance.PlaySE(0);
            InMemoryDataTransitClient<int> stageNumDataTransitClient = new InMemoryDataTransitClient<int>();
            int stageNum = stageNumDataTransitClient.Get(StorageKey.KEY_STAGENUMBER);
            string retrySceneName = m_stagePreviewDatas.stageSelectList.Where(stagedata => stagedata.stageNumber == stageNum).First().gameScene;
            stageNumDataTransitClient.Set(StorageKey.KEY_STAGENUMBER, stageNum);
            SceneManager.LoadScene(retrySceneName);
        }
    }
}

