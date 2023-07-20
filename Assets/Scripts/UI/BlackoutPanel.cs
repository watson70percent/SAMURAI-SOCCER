using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;
using SamuraiSoccer.Event;

namespace SamuraiSoccer.UI
{
    public class BlackoutPanel : MonoBehaviour
    {
        [SerializeField]
        private Image m_panelImage;

        private void Start()
        {
            UIEffectEvent.BlackOut.Subscribe(async totalsec =>
            {
                await Blackout(totalsec);
            }).AddTo(this);
        }

        /// <summary>
        /// ‡‰æ–Ê‚ÌˆÃ“]‚ğs‚¤
        /// </summary>
        /// <param name="totaltimesec">ˆÃ“]‚Ì‘S‘ÌŠÔ</param>
        /// <returns></returns>
        public async UniTask Blackout(float totaltimesec)
        {
            float time = 0;
            while (time < totaltimesec)
            {
                var c = new Color(0, 0, 0, 1 - Mathf.Abs(4 - time));
                m_panelImage.color = c;
                await UniTask.Yield();
                time += Time.deltaTime;
            }
        }
    }
}
