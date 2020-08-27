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
    private void OnGUI()
    {
        if (_foldinglist = EditorGUILayout.Foldout(_foldinglist, "StateDebugger"))
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

    }
}
