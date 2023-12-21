using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SamuraiSoccer
{
    public class DetectExceptionLog : MonoBehaviour
    {
        [SerializeField]
        private Text m_logText;

        private void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Exception)
            {
                m_logText.text = logString + "\n";
            }
        }
    }
}
