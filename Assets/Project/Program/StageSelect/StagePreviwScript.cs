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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void previewDisplay(string stageName, string summary, Sprite image, SceneObject scene)
    {
        
        nameText.text = stageName;
        summaryText.text = summary;
        stageImage.GetComponent <Image>().sprite = image;
        gameScene = scene;
        this.gameObject.SetActive(true);
    }

    public void OnClickClose()
    {
        stageSelectMng.selectState();
        this.gameObject.SetActive(false);
    }

    public void OnClickStart()
    {
        Debug.Log("ゲームスタート");
        SceneManager.LoadScene(gameScene);
    }

}
