using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace SamuraiSoccer
{
    public class ModifyStageData : EditorWindow
    {
        [MenuItem("Custom/Modify stage data")]
        private static void ShowWindow()
        {
            GetWindow<ModifyStageData>("Modify stage data");
        }

        private SaveData m_data;

        private void OnGUI()
        { 
            using(new EditorGUILayout.VerticalScope())
            {
                if (m_data == null)
                {
                    var client = new InFileTransmitClient<SaveData>();
                    client.TryGet(StorageKey.KEY_STAGENUMBER, out m_data);
                }
                m_data.m_stageData = EditorGUILayout.IntField("Stage save data", m_data.m_stageData);
                if (GUILayout.Button("Save"))
                {
                    var client = new InFileTransmitClient<SaveData>();
                    client.Set(StorageKey.KEY_STAGENUMBER, m_data.m_stageData);
                }
            }
        }
    }
}
