using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(StageData))]
public class StageIconButton : MonoBehaviour
{
    string stageName;
    string stageSummary;
    public GameObject preview;
    public StageSelectManager stageSelectMng;
    Sprite stImage;
    SceneObject gameScene;
    public int indexOfButton;

    private StageData _stageData;

    // Start is called before the first frame update
    void Start()
    {
        _stageData = GetComponent<StageData>();
        if (_stageData.StageState == StageState.NotPlayable)
        {
            _stageData.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,0.5f);
        }
    }

    private void DataSet()
    {
        (string stName, string summary, Sprite stagePreview, SceneObject scene) buttonData = stageSelectMng.ButtonDataSet(indexOfButton);
        stageName = buttonData.stName;
        stageSummary = buttonData.summary;
        stImage = buttonData.stagePreview;
        gameScene = buttonData.scene;
    }

    public void OnClick()
    {
        DataSet();
        BaseStageData basestageData = GetComponent<StageData>().ToBaseStageData();
        stageSelectMng.previewState(stageName, stageSummary, stImage, gameScene,basestageData);

    }
}
