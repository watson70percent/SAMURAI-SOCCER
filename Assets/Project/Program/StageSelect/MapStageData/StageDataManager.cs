using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 各ステージの名前と順番
/// </summary>
public enum WorldName
{
    UK = 0,
    China = 1,
    America = 2,
    Russia = 3,
    WholeMap = 99,
}

/// <summary>
/// 各ステージのワールドと番号
/// </summary>
public class BaseStageData
{
    public WorldName WorldName { get; set; }
    public int StageNumber { get; set; }
    public BaseStageData(WorldName worldName, int stageNumber)
    {
        WorldName = worldName;
        StageNumber = stageNumber;
    }
}


public class StageDataManager : MonoBehaviour
{
    private GameObject[] TargetObjects;
    /// <summary>
    /// すべてのステージを検索し、状態管理する
    /// </summary>
    private void Start()
    {
        TargetObjects = GameObject.FindGameObjectsWithTag("TargetStage");
        BaseStageData currentStageData = LoadStageData();
        int currentWorldCount = 0;
        //現在いるワールドのステージ数を検索
        for (int i = 0; i < TargetObjects.Length; i++)
        {
            if (TargetObjects[i].GetComponent<StageData>().WorldName == currentStageData.WorldName)
            {
                currentWorldCount++;
            }
        }
        //条件によってステージの状態を設定
        for (int i = 0; i < TargetObjects.Length; i++)
        {
            if (TargetObjects[i].GetComponent<StageData>().WorldName < currentStageData.WorldName)
            {
                TargetObjects[i].GetComponent<StageData>().StageState = StageState.Cleared;
            }
            else if (TargetObjects[i].GetComponent<StageData>().WorldName == currentStageData.WorldName && (int)TargetObjects[i].GetComponent<StageData>().StageNumber <= currentStageData.StageNumber)
            {
                TargetObjects[i].GetComponent<StageData>().StageState = StageState.Cleared;
            }
            else if (TargetObjects[i].GetComponent<StageData>().WorldName == currentStageData.WorldName && (int)TargetObjects[i].GetComponent<StageData>().StageNumber == currentStageData.StageNumber + 1)
            {
                TargetObjects[i].GetComponent<StageData>().StageState = StageState.Playable;
            }
            else if (currentStageData.StageNumber == currentWorldCount - 1 && TargetObjects[i].GetComponent<StageData>().WorldName == currentStageData.WorldName + 1 && (int)TargetObjects[i].GetComponent<StageData>().StageNumber == 0)
            {
                TargetObjects[i].GetComponent<StageData>().StageState = StageState.Playable;
            }
            else
            {
                TargetObjects[i].GetComponent<StageData>().StageState = StageState.NotPlayable;
            }
        }
    }
    /// <summary>
    /// ステージクリア時などにクリアしたステージ番号をセーブする(ただし新しいステージをクリアしたときのみ)
    /// </summary>
    /// <param name="stageData">SaveするStageData</param>
    public static void SaveStageData(BaseStageData stageData)
    {
        int pastWorldName = PlayerPrefs.GetInt("worldName", 0);
        int pastStageNumber = PlayerPrefs.GetInt("stageNumber", -1);
        if ((int)stageData.WorldName > pastWorldName)
        {
            PlayerPrefs.SetInt("worldName", (int)stageData.WorldName);
            PlayerPrefs.SetInt("stageNumber", stageData.StageNumber);
            Debug.Log("Save" + stageData);
        }
        else if ((int)stageData.WorldName == pastWorldName && stageData.StageNumber > pastStageNumber)
        {
            PlayerPrefs.SetInt("worldName", (int)stageData.WorldName);
            PlayerPrefs.SetInt("stageNumber", stageData.StageNumber);
            Debug.Log("Save" + stageData);
        }
    }

    /// <summary>
    /// どのワールドの何番目のステージまでクリアしたのかを取得し、それをStageData型で返す
    /// </summary>
    /// <returns></returns>
    public static BaseStageData LoadStageData()
    {
        BaseStageData stageData = new BaseStageData((WorldName)PlayerPrefs.GetInt("worldName", 0), PlayerPrefs.GetInt("stageNumber", -1));
        Debug.Log("Load" + stageData);
        return stageData;
    }
}
