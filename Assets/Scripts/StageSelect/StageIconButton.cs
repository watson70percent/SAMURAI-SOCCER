using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(StageData))]
public class StageIconButton : MonoBehaviour
{
    string stageName;
    string stageSummary;
    public GameObject Preview;
    public StageSelectManager StageSelectMng;
    Sprite stImage;
    SceneObject gameScene;
    public int IndexOfButton;

    private StageData _stageData;
    public SpriteRenderer SpriteRenderer;
    public Sprite MonoImage;

    // Start is called before the first frame update
    void Start()
    {
        _stageData = GetComponent<StageData>();
        if (_stageData.StageState == StageState.NotPlayable)
        {
            SpriteRenderer.sprite = MonoImage;
        }
    }

    private void DataSet()
    {
        (string stName, string summary, Sprite stagePreview, SceneObject scene) buttonData = StageSelectMng.ButtonDataSet(IndexOfButton);
        stageName = buttonData.stName;
        stageSummary = buttonData.summary;
        stImage = buttonData.stagePreview;
        gameScene = buttonData.scene;
    }

    public void OnClick()
    {
        DataSet();
        BaseStageData basestageData = GetComponent<StageData>().ToBaseStageData();
        StageSelectMng.previewState(stageName, stageSummary, stImage, gameScene,basestageData);

    }
}
