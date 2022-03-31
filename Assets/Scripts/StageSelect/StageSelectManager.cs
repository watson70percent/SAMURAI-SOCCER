using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class StageSelectManager : MonoBehaviour
{

    /// <summary>
    /// いまはStageSelectManagerにつけてるけど、適当なオブジェクトにつけて、オブジェクトを適切につければ動くよ
    /// </summary>
    public enum StageSelectState
    {
        select,
        preview
    }

    public StageSelectState state;
    public GameObject canvas;
    public GameObject stagePreview;
    StagePreviwScript stagePreviwScript;
    public GameObject prefabButton;

    //stageselectのデータ(public)
    public StageSelectData stageSelectData;


    // Start is called before the first frame update
    void Start()
    {
        stagePreviwScript = stagePreview.GetComponent<StagePreviwScript>();
    }

    public void previewState(string stageName, string summary, Sprite stImage, SceneObject gameScene,BaseStageData baseStageData)
    {
        //previewを見せる
        state = StageSelectState.preview;
        stagePreviwScript.previewDisplay(stageName, summary, stImage, gameScene,baseStageData);
    }

    public void selectState()
    {
        //ステージを選ぶ
        state = StageSelectState.select;
        
    }

    public (string stName, string summary, Sprite stagePreview, SceneObject scene)ButtonDataSet(int index)
    {
        StageSelect item = new StageSelect();
        //押されたボタンの番号のデータを返す
        foreach (StageSelect stageSelect in stageSelectData.stageSelectList)
        {
            if (stageSelect.stagenumber == index)
            {
                item = stageSelect;
            }
        }
        //相手選手のデータ、ステージ名をstaticにセットしておく
        FieldNumber.no = item.fieldNumber;
        OpponentName.name = item.oppnentName;
        return (stName:item.previewName, summary:item.summary, stagePreview:item.stageImage, scene:item.gameScene);


    }

}
