using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SamuraiWordBase))]
public class SamuraiWordBaseEditor : Editor
{

    // Listの折りたたみ用の変数
    bool folding_list = false;
    public override void OnInspectorGUI()
    {
        SamuraiWordBase samuraiWordBase = target as SamuraiWordBase;
        var list = samuraiWordBase.samuraiwords;
        // Listを折りたたみ表示
        if (folding_list = EditorGUILayout.Foldout(folding_list, "List"))
        {
            // インデントを増やす
            EditorGUI.indentLevel++;

            for (int i = 0; i < list.Count; i++)
            {
                list[i] =  EditorGUILayout.TextArea(list[i]);
                EditorGUILayout.BeginHorizontal();

                // いっぱいまで空白を埋める
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Delete"))
                {
                    list.RemoveAt(i);
                }

                EditorGUILayout.EndHorizontal();
                // --ここまで--
            }


            // インデントを減らす
            EditorGUI.indentLevel--;
            

            // Listの追加
            if (GUILayout.Button("Add"))
            {
                list.Add("");
            }
            if (GUILayout.Button("Save"))
            {
                Undo.RecordObject(target, "Save");
                EditorUtility.SetDirty(samuraiWordBase);
            }


            // インデントを減らす
            EditorGUI.indentLevel--;
        }
    }
}
