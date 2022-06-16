using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SamuraiSoccer.StageContents
{
    public interface StageDataReceiver
    {
        void StageDataReceive(BaseStageData stageData);
    }

    public class StageDataHolder : MonoBehaviour
    {

        static BaseStageData nowStageData;

        public static BaseStageData NowStageData { get => nowStageData; }
        public void SetStageData(BaseStageData stageData)
        {
            nowStageData = stageData;
            Debug.Log("Hold stage data : worldName = " + nowStageData.WorldName + ", StageNumber = " + nowStageData.StageNumber);
            SceneManager.sceneLoaded += GameSceneLoaded;
        }


        void GameSceneLoaded(Scene next, LoadSceneMode mode)
        {
            var resultManager = GameObject.Find("ResultManager");
            if (resultManager != null)
            {
                resultManager.GetComponent<StageDataReceiver>().StageDataReceive(nowStageData);
            }

            SceneManager.sceneLoaded -= GameSceneLoaded;
        }

    }
}
