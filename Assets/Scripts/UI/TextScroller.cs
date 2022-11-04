using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UniRx;

namespace SamuraiSoccer.UI
{
    /// <summary>
    /// UIテキストの一文字ずつ送る動作を行う．
    /// </summary>
    public class TextScroller : MonoBehaviour
    {
        [SerializeField]
        private TouchProvider m_provider;

        [SerializeField]
        private Text m_text;

        [SerializeField]
        private int m_waitms4char = 40;

        [SerializeField]
        private int m_guardTime = 500;

        private string m_fullText = "";
        private bool m_touchGuard = false;
        private bool m_isTouched = false;

        // Start is called before the first frame update
        void Start()
        {
            m_provider.IsTouchingReactiveProperty.Where(b => b).Subscribe(OnTouched).AddTo(this);
            m_text.text = "";
        }

        void Update()
        {
            if (m_isTouched)
            {
                m_text.text = m_fullText;
            }
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

        private void OnTouched(bool isTouch)
        {
            if (!m_touchGuard)
            {
                m_isTouched = true;
            }
        }

        private void InitializeState(string text)
        {
            m_text.text = "";
            m_fullText = text;
            m_isTouched = false;
            m_touchGuard = true;
            _ = GuardTimer();
        }

        private async UniTask GuardTimer()
        {
            await UniTask.Delay(m_guardTime);
            m_touchGuard = false;
        }
    }
}
