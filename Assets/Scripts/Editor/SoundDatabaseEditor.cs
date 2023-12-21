using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(SoundDatabase))]
public class SoundDatabaseEditor : Editor
{

    // Listの折りたたみ用の変数
    bool folding_list = false;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var list = serializedObject.FindProperty("soundDatas");
        List<int> soundIndexs = new List<int>();
        // Listを折りたたみ表示
        if (folding_list = EditorGUILayout.Foldout(folding_list, "SoundList"))
        {
            using (new EditorGUI.IndentLevelScope())
            {
                for (var i = 0; i < list.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i).FindPropertyRelative("soundIndex"));
                    soundIndexs.Add(list.GetArrayElementAtIndex(i).FindPropertyRelative("soundIndex").intValue);
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i).FindPropertyRelative("baseSound"));
                    list.GetArrayElementAtIndex(i).FindPropertyRelative("soundVolume").floatValue = EditorGUILayout.Slider(list.GetArrayElementAtIndex(i).FindPropertyRelative("soundVolume").floatValue, 0f, 1f); ;
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        // いっぱいまで空白を埋める
                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("Delete"))
                        {
                            list.DeleteArrayElementAtIndex(i);
                        }
                    }
                }
            }

            // Listの追加
            if (GUILayout.Button("Add"))
            {
                list.InsertArrayElementAtIndex(list.arraySize);
                list.GetArrayElementAtIndex(list.arraySize-1).FindPropertyRelative("soundIndex").intValue = soundIndexs.Max() + 1; 
            }
        }

        if (soundIndexs.Distinct().Count() != soundIndexs.Count)
        {
            Debug.LogError("入力されたsoundIndexは既に使用されている番号です。\nsoundIndexをかぶらないものに変更してください。");
            EditorUtility.DisplayDialog("入力エラー", "入力されたsoundIndexは既に使用されている番号です。\nsoundIndexをかぶらないものに変更してください。", "OK","Cancel");
            return;
        }
        serializedObject.ApplyModifiedProperties();
    }
}
