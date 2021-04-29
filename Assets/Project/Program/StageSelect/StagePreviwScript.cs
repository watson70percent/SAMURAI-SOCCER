using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StagePreviwScript : MonoBehaviour
{
    public Text nameText;
    public Text summaryText;
    public StageSelectManager stageSelectMng;
    public GameObject stageImage;
    SceneObject gameScene;

    bool isPreview;

    BaseStageData baseStageData;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //プレビューを表示する
    public void previewDisplay(string stageName, string summary, Sprite image, SceneObject scene,BaseStageData baseStageData)
    {
        
        nameText.text = stageName;
        summaryText.text = summary;
        stageImage.GetComponent <Image>().sprite = image;
        gameScene = scene;
        this.gameObject.SetActive(true);

        this.baseStageData = baseStageData;
    }

    //x押されたら閉じる
    public void OnClickClose()
    {
        stageSelectMng.selectState();
        this.gameObject.SetActive(false);
    }

    //スタート押されたらシーン呼び出す
    public void OnClickStart()
    {
        Debug.Log("ゲームスタート");

        SceneManager.sceneLoaded += GameSceneLoaded;

        SceneManager.LoadScene(gameScene);
    }

    void GameSceneLoaded(Scene next, LoadSceneMode mode)
    {
        GameObject.Find("DefaultStage").GetComponent<StageDataHolder>().SetStageData(baseStageData);

        SceneManager.sceneLoaded -= GameSceneLoaded;
    }


}
