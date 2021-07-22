using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class IceStageManager : MonoBehaviour
{
    public StagePrefabManager stageManager;
    public Texture greenTexture;
    public Texture iceTexture;
    public Material greenMaterial;
    public Material iceMaterial;
    public GameObject eclanoplan;
    static public int no = 2;

    private void Start()
    {
        switch (StageDataHolder.NowStageData.StageNumber)
        {
            case 0:
                stageManager.groundTexture = greenTexture;
                stageManager.groundMaterial = greenMaterial;
                eclanoplan.SetActive(false);
                break;
            case 1:
                stageManager.groundTexture = iceTexture;
                stageManager.groundMaterial = iceMaterial;
                eclanoplan.SetActive(false);
                break;
            case 2:
                stageManager.groundTexture = iceTexture;
                stageManager.groundMaterial = iceMaterial;
                eclanoplan.SetActive(true);
                break;

        }
    }
}
