using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiSoccer.UI
{
    /// <summary>
    /// 巻物の右側をHideパネルで隠しておく
    /// UIのレンダー順はいじれなかったのでスクリプト側で解決した
    /// </summary>
    public class HidePanel : MonoBehaviour
    {
        [SerializeField]
        RectTransform m_parent;
        [SerializeField]
        Vector3 m_pos;
        // Start is called before the first frame update
        void Start()
        {
            m_pos = transform.position - m_parent.position;
        }

        // Update is called once per frame
        public void Update()
        {
            this.transform.position = m_parent.position + m_pos;
        }
    }
}
