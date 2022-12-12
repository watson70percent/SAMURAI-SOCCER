using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SamuraiSoccer.StageContents.StageSelect
{
    public class ScrollIcon : StageIcon
    {
        [SerializeField, Range(0f, 1f)]
        private float m_colorCoef;

        // Start is called before the first frame update
        void Start()
        {
            if (State == StageState.NotPlayable)
            {
                gameObject.GetComponent<Image>().color = new Color(m_colorCoef, m_colorCoef, m_colorCoef);
            }
        }
    }
}
