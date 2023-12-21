using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiSoccer.UI
{
    public class ExeptionLogUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_logContents;

        [SerializeField]
        private GameObject[] m_hiddenObjects;

        private bool m_logDisplayed;

        public void LogDisplay()
        {
            // log���\������Ă��Ȃ���Ε\������A�\������Ă������\���ɖ߂�
            if (!m_logDisplayed)
            {
                m_logContents.SetActive(true);
                for (int i = 0; i < m_hiddenObjects.Length; i++)
                {
                    m_hiddenObjects[i].gameObject.SetActive(false);
                }
                m_logDisplayed = true;
            }
            else
            {
                m_logContents.SetActive(false);
                for (int i = 0; i < m_hiddenObjects.Length; i++)
                {
                    m_hiddenObjects[i].gameObject.SetActive(true);
                }
                m_logDisplayed = false;
            }
        }
    }
}
