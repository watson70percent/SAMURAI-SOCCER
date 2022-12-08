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
        [SerializeField] GameObject ScrollObject;//������3D�I�u�W�F�N�g
        [SerializeField] float rotSpeed;//��]���x
        [SerializeField] float startX, goalX;//�����̏����ʒu�ƍŏI�ʒu��X���W


        /// <summary>
        /// �����������ʒu������Ώ̂̈ʒu�Ɉړ�������
        /// </summary>
        /// <returns></returns>
        public async UniTask ScrollSlide()
        {
            float elapsedTime = 0;
            float y = rectra.anchoredPosition.y;
            float rotSign;//��]�̌���
            if (startX > goalX)
            {
                rotSign = -1;
            }
            else
            {
                rotSign = 1;
            };
            while (elapsedTime < slideTime)
            {
                elapsedTime += Time.deltaTime;
                float x = easeOutCubic(elapsedTime, goalX, startX, slideTime);
                rectra.anchoredPosition = new Vector2(x, y);
                ScrollObject.transform.eulerAngles = new Vector3(0, rotSign * x * rotSpeed, 0);
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
