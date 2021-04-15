using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface StageDataReceiver
{
    void StageDataReceive(BaseStageData stageData);
}

public class StageDataHolder : MonoBehaviour
{

    static BaseStageData nowStageData;
    public void SetStageData(BaseStageData stageData)
    {
        nowStageData = stageData;
        SceneManager.sceneLoaded += GameSceneLoaded;
    }


    void GameSceneLoaded(Scene next, LoadSceneMode mode)
    {
        var resultManager=GameObject.Find("ResultManager");
        resultManager.GetComponent<StageDataReceiver>().StageDataReceive(nowStageData);

        SceneManager.sceneLoaded -= GameSceneLoaded;
    }

}
