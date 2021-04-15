using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class StageSelectData : ScriptableObject
{
    public List<StageSelect> stageSelectList = new List<StageSelect>();
}

[System.Serializable]
public class StageSelect
{
    public string name;
    public string previewName;
    [TextArea(1, 5)]
    public string summary;
    public Sprite stageImage;
    public SceneObject gameScene;

    public int fieldNumber;
    public string oppnentName;
}