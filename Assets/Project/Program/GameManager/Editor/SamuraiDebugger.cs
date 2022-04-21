using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SamuraiDebugger : EditorWindow
{
    [MenuItem("Custom/SamuraiDebugger")]
    static void ShowWindow()
    {
        EditorWindow.GetWindow<SamuraiDebugger>();
    }

    bool _foldinglist = false;
    string _gameManagerName = "GameManager";
    GameState _gameState;
    GameManager _gameManager;
    WorldName _worldName;
    int _stageNumber;
    private void OnGUI()
    {
        if (_foldinglist = EditorGUILayout.Foldout(_foldinglist, "GameStateDebugger"))
        {
            EditorGUI.indentLevel++;

            _gameManagerName = EditorGUILayout.TextField(_gameManagerName);
            _gameState = (GameState)EditorGUILayout.EnumPopup("TargetState", _gameState);//enum値の表示

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("ChangeToTargetState"))
            {
                if (EditorApplication.isPlaying)
                {
                    if (!GameObject.Find(_gameManagerName))
                    {
                        Debug.LogError("Scene上にGameManagerオブジェクトが存在しません。もし、GameManagerスクリプトがGameManegerという名前以外のオブジェクトについている場合はこのスクリプトのgameManagerNameを変更してさい。");
                        return;
                    }
                    _gameManager = GameObject.Find(_gameManagerName).GetComponent<GameManager>();
                }
                _gameManager.CurrentStateChanger(_gameState);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("  CurrentState  "))
            {
                if (EditorApplication.isPlaying)
                {
                    if (!GameObject.Find(_gameManagerName))
                    {
                        Debug.LogError("Scene上にGameManagerオブジェクトが存在しません。もし、GameManagerスクリプトがGameManegerという名前以外のオブジェクトについている場合はこのスクリプトのgameManagerNameを変更してさい。");
                        return;
                    }
                    _gameManager = GameObject.Find(_gameManagerName).GetComponent<GameManager>();
                }
                _gameManager.CurrentStateResercher();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }
        if(_foldinglist = EditorGUILayout.Foldout(_foldinglist, "SaveDataDebugger"))
        {
            EditorGUI.indentLevel++;
            _worldName = (WorldName)EditorGUILayout.EnumPopup("TargetWorldName", _worldName);
            _stageNumber = EditorGUILayout.IntField("TargetWorldNumber", _stageNumber);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("ChanegeToSaveData"))
            {
                PlayerPrefs.SetInt("worldName", (int)_worldName);
                PlayerPrefs.SetInt("stageNumber", _stageNumber);
                Debug.Log("デバッグ関数の使用、("+_worldName+","+_stageNumber+")にSaveしました。");
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("  CurrentSaveData  "))
            {
                Debug.Log("現在のSaveDataは("+StageDataManager.LoadStageData().WorldName+","+StageDataManager.LoadStageData().StageNumber+")です");
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }
    }
}
