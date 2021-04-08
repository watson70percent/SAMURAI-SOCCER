using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageDataSave : MonoBehaviour,StageDataReceiver
{
    public void StageDataReceive(BaseStageData stageData)
    {
        StartCoroutine(Save(stageData));
    }

    IEnumerator Save(BaseStageData stageData)
    {
        Result result;
        while ((result=GetComponent<ResultManager>().ResultState) == Result.Undefined)
        {
            yield return null;
        }

        if (result == Result.Win)
        {
            StageDataManager.SaveStageData(stageData);
        }
        //print(result.ToString() + "," + stageData.WorldName);
    }
}
