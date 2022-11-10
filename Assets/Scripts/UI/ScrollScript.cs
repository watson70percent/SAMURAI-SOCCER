using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace SamuraiSoccer.UI
{
    /// <summary>
    /// �������Ȃ߂炩�Ɉړ�������
    /// </summary>
    public class ScrollScript : MonoBehaviour
    {
        [SerializeField] RectTransform rectra;//������������RectTransform
        [SerializeField] float slideTime;//�ړ��ɂ����鎞��
        Vector3 rotVec;
        Vector2 startVec, goalVec;

        // Start is called before the first frame update
        void Start()
        {
            startVec = rectra.anchoredPosition;
            goalVec = new Vector2(-startVec.x, startVec.y);
        }
        /// <summary>
        /// �����������ʒu������Ώ̂̈ʒu�Ɉړ�������
        /// </summary>
        /// <returns></returns>
        public async UniTask ScrollSlide()
        {
            float elapsedTime = 0;
            while (elapsedTime < slideTime)
            {
                elapsedTime += Time.deltaTime;
                float x = easeOutCubic(elapsedTime, goalVec.x, startVec.x, slideTime);
                rectra.anchoredPosition = new Vector2(x, startVec.y);
                await UniTask.Delay(1);
                Debug.Log(elapsedTime);
            }
        }

        /// <summary>
        /// �C�[�W���O�̌v�Z
        /// </summary>
        /// <param name="t">���݂̎���</param>
        /// <param name="goal">�ŏI�n�_��x���W</param>
        /// <param name="start">t=0��x���W</param>
        /// <param name="goalTime">�ŏI�n�_�̎���</param>
        /// <returns>���݂�x���W</returns>
        float easeOutCubic(float t, float goal, float start, float goalTime)
        {
            return (goal - start) * (1 - Mathf.Pow(goalTime - t, 3)) + start;
        }
    }
}
