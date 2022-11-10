using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SamuraiSoccer.Event.EditorExtension
{
    public class InGameEventEditor : EditorWindow
    {
        private bool m_hasYellowCard = false;
        private bool m_isPaused = false;

        [MenuItem("Custom/InGameEventDebugger")]
        static void ShowWindow()
        {
            var window = GetWindow(typeof(InGameEventEditor));
            window.titleContent = new GUIContent("InGameEventDebugger");
        }
        private void OnGUI()
        {
            EditorGUILayout.LabelField("TriggerEvent");
            if (GUILayout.Button("Reset"))
            {
                InGameEvent.ResetOnNext();
            }
            if (GUILayout.Button("Standby"))
            {
                InGameEvent.StandbyOnNext();
            }
            if (GUILayout.Button("Play"))
            {
                InGameEvent.PlayOnNext();
            }
            if (GUILayout.Button("Goal"))
            {
                InGameEvent.GoalOnNext();
            }
            m_hasYellowCard = EditorGUILayout.Toggle("HasYellowCard", m_hasYellowCard);
            if (GUILayout.Button("Penalty"))
            {
                //イエローカードの所持数がint表記なのでboolから変換
                if (m_hasYellowCard)
                {
                    InGameEvent.PenaltyOnNext(1);
                }
                else
                {
                    InGameEvent.PenaltyOnNext(0);
                }
            }
            m_isPaused = EditorGUILayout.Toggle("IsPaused", m_isPaused);
            if (GUILayout.Button("Pause"))
            {
                InGameEvent.PauseOnNext(!m_isPaused);
            }
            if (GUILayout.Button("Finish"))
            {
                InGameEvent.FinishOnNext();
            }
        }

    }
}
