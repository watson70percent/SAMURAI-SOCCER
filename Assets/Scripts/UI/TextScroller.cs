using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace SamuraiSoccer.UI
{
    /// <summary>
    /// UIテキストの一文字ずつ送る動作を行う．
    /// </summary>
    public class TextScroller : MonoBehaviour
    {
        [SerializeField]
        private Text m_text;

        [SerializeField]
        private int m_waitms4char = 30;

        private string m_fullText = "";
        private bool m_isBeforeTouched = false;
        private bool m_isTouched = false;

        // Start is called before the first frame update
        void Start()
        {
            m_text.text = "";
        }

        void Update()
        {
            var isTouched = Input.touchCount > 0;
            if (!m_isBeforeTouched && isTouched)
            {
                m_isTouched = true;
                m_text.text = m_fullText;
            }
            m_isBeforeTouched = isTouched;
        }

        /// <summary>
        /// 文字を一文字ずつ表示する．
        /// </summary>
        /// <param name="text">表示する文字．</param>
        public async UniTask ShowText(string text)
        {
            InitializeState(text);
            for (var i = 0; i < text.Length - 1; i++)
            {
                m_text.text = text[0..i];
                await UniTask.Delay(m_waitms4char);
                if (m_isTouched)
                {
                    break;
                }
            }
            m_text.text = text;
        }

        private void InitializeState(string text)
        {
            m_text.text = "";
            m_fullText = text;
            m_isBeforeTouched = Input.touchCount > 0;
            m_isTouched = false;
        }
    }
}
