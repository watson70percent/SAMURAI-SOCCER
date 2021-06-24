using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(ResultManager))]
public class LoadRetryScene : MonoBehaviour
{
    public StageSelectData stageSelectData;
    BaseStageData baseStageData;
    private ResultManager _resultManager;
    private void Start()
    {
        _resultManager = GetComponent<ResultManager>();
    }
    /// <summary>
    /// 今遊んだSceneをStageSelectDataから取得しそのSceneへ移動
    /// </summary>
    public void LoadScene()
    {
        if (_resultManager.NowStageData == null) return;
        baseStageData = _resultManager.NowStageData;
        string retrySceneName = stageSelectData.stageSelectList[(int)baseStageData.WorldName * 3 + baseStageData.StageNumber].gameScene;
        SceneManager.sceneLoaded += GameSceneLoaded;
        SceneManager.LoadScene(retrySceneName);
    }

    void GameSceneLoaded(Scene next, LoadSceneMode mode)
    {
        GameObject.Find("DefaultStage").GetComponent<StageDataHolder>().SetStageData(baseStageData);

        SceneManager.sceneLoaded -= GameSceneLoaded;
    }
}
