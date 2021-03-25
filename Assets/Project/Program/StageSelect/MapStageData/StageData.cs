using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StageState 
{
    NotPlayable,
    Playable,
    Cleared
}

public class StageData : MonoBehaviour
{
    [SerializeField]
    private WorldName _worldName;
    public WorldName WorldName
    {
        get { return _worldName; }
        private set { _worldName = value; }
    }
    [SerializeField]
    private int _stageNumber;
    public int StageNumber
    {
        get { return _stageNumber; }
        private set { _stageNumber = value; }
    }
    [SerializeField]
    private StageState _stageState;
    public StageState StageState { get { return _stageState; } set { _stageState = value; } }
}
