using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace SamuraiSoccer.UI
{
    public class UIFade : MonoBehaviour
    {
        [SerializeField, Tooltip("�t�F�[�h������UI")]
        private List<Image> m_images = new List<Image>();

        [SerializeField]
        private float m_fadeSpeed = 1;

        [SerializeField]
        private bool m_isStartEnabled = true;

        private void Start()
        {
            // �K�v������Ώ����������Ƃ��ĉ摜��Color�𓧖��ɂ���
            if (!m_isStartEnabled)
            {
                for (int i=0; i < m_images.Count; i++)
                {
                    m_images[i].color = new Color(m_images[i].color.r, m_images[i].color.g, m_images[i].color.b, 0);
                }
            }
        }

        /// <summary>
        /// �摜�̃��l�𑝂₷���Ƃŉ摜���o�������� 
        /// </summary>
        /// <returns></returns>
        public async UniTask FadeInUI()
        {
            bool needFade = true;
            while (needFade)
            {
                for (int i = 0; i < m_images.Count; i++)
                {
                    m_images[i].color += new Color(0, 0, 0, Time.deltaTime * m_fadeSpeed);
                }
                if (m_images.Any(x => x.color.a < 1))
                {
                    needFade = true;
                }
                else
                {
                    needFade = false;
                }
                await UniTask.DelayFrame(3);
            }
        }

        /// <summary>
        /// �摜�̃��l�����炷���Ƃŉ摜�������Ȃ��悤�ɂ���
        /// </summary>
        /// <returns></returns>
        public async UniTask FadeOutUI()
        {
            bool needFade = true;
            while (needFade)
            {
                for (int i = 0; i < m_images.Count; i++)
                {
                    m_images[i].color -= new Color(0, 0, 0, Time.deltaTime * m_fadeSpeed);
                }
                if (m_images.Any(x => x.color.a > 0))
                {
                    needFade = true;
                }
                else
                {
                    needFade = false;
                }
                await UniTask.DelayFrame(3);
            }
        }
    }
}
