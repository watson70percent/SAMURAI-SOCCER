using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageIconButton : MonoBehaviour
{
    public string stageName;
    public string stageSummary;
    public GameObject preview;
    public StageSelectManager stageSelectMng;
    public Sprite stImage;
    SceneObject gameScene;
    public int indexOfButton;

    // Start is called before the first frame update
    void Start()
    {
        (string stName, string summary, Sprite stagePreview, SceneObject scene)buttonData = stageSelectMng.ButtonDataSet(indexOfButton);
        stageName = buttonData.stName;
        stageSummary = buttonData.summary;
        stImage = buttonData.stagePreview;
        gameScene = buttonData.scene;
    }

    // Update is called once per frame
    void Update()
    {
        if (stageSelectMng.state == StageSelectManager.StageSelectState.preview)
        {
            GetComponent<Button>().interactable = false;
        }
        else
        {
            GetComponent<Button>().interactable = true;
        }
    }

    public void OnClick()
    {
        stageSelectMng.previewState(stageName, stageSummary, stImage, gameScene);

    }
}
