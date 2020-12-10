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
    public StageSelectData stageSelectData;// = Resources.Load<StageSelectData>("Project/Program/StageSelect/StageSelectData1");
    StageSelect stageSelect;// = stageSelectData.stageSelectList[0];

    //タップした先にあるものを入れるリスト。
    // 今回はコストが気になるのでフィールドにするよ
    // あんまり頻繁に判定しないならメソッドの中に入れてもいいよ
    //private List<RaycastResult> raycastResults = new List<RaycastResult>();

    //public bool IsPointerOnUGUI(Vector2 screenPosition)
    //{
    //    // EventSystemがない = uGUIがないときは遮るものがないので処理そのものをさせないよ
    //    if (EventSystem.current == null) { return false; }

    //    // EventSystemにタップした座標を設定するよ
    //    PointerEventData eventDataCurrent = new PointerEventData(EventSystem.current);
    //    eventDataCurrent.position = screenPosition;

    //    // タップ地点の先にあるものを調べるためにRayCastするよ
    //    EventSystem.current.RaycastAll(eventDataCurrent, raycastResults);
    //    bool result=false;
    //    foreach (var item in raycastResults)
    //    {
    //        result = result ||(item.gameObject.tag == "PreviewWindow");
    //    }
        

    //    raycastResults.Clear(); //リセットしておくよ
    //    return result;
    //}


    // Start is called before the first frame update
    void Start()
    {
        stagePreviwScript = stagePreview.GetComponent<StagePreviwScript>();
        
       

    }

    // Update is called once per frame
    void Update()
    {
        //ウィンドウの外タップで閉じる機能。いまのところは使わない方向
        //switch (state)
        //{
        //    case StageSelectState.preview:
        //        if (Input.GetTouch)
        //        {

        //            if (!IsPointerOnUGUI(Input.mousePosition))
        //            {
        //                stagePreviwScript.OnClickClose();
        //                state = StageSelectState.select;
        //            }

        //        }
        //        break;
               
        //    default:
        //        break;
        //}
    }

    public void previewState(string stageName, string summary, Sprite stImage, SceneObject gameScene)
    {
        //previewを見せる
        state = StageSelectState.preview;
        stagePreviwScript.previewDisplay(stageName, summary, stImage, gameScene);
    }

    public void selectState()
    {
        //ステージを選ぶ
        state = StageSelectState.select;
        
    }

    public (string stName, string summary, Sprite stagePreview, SceneObject scene)ButtonDataSet(int index)
    {
        //押されたボタンの番号のデータを返す
        StageSelect item = stageSelectData.stageSelectList[index];
        //相手選手のデータ、ステージ名をstaticにセットしておく
        FieldNumber.no = item.fieldNumber;
        OpponentName.name = item.oppnentName;
        StandbyStateProcess.OpponentInfo=item.opponentInfo;
        return (stName:item.previewName, summary:item.summary, stagePreview:item.stageImage, scene:item.gameScene);


    }

}
