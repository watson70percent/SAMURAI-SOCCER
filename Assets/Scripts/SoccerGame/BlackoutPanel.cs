using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace SamuraiSoccer.SoccerGame
{
    public class BlackoutPanel : MonoBehaviour
    {
        [SerializeField]
        private Image m_panelImage;

        /// <summary>
        /// ������ʂ̈Ó]���s��
        /// </summary>
        /// <param name="totaltimesec">�Ó]�̑S�̎���</param>
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
