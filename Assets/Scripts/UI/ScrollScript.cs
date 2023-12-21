using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using SamuraiSoccer.Event;
using System.Threading;

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
        [SerializeField] float rotSpeed;//��]���x�W��
        [SerializeField] Material[] FlagMaterials;//���̃}�e���A��
        MeshRenderer ScrollMaterial;//����(3D)��MeshRenderer

        private Vector3 initRot;

        public void Start()
        {
            initRot = ScrollObject.transform.eulerAngles;
            ScrollMaterial = ScrollObject.GetComponent<MeshRenderer>();
        }

        /// <summary>
        /// �����̉�]�������s��
        /// </summary>
        /// <param name="startX">�����̏����ʒu��X���W</param>
        /// <param name="goalX">�����̍ŏI�ʒu��X���W</param>
        /// <param name="Y">������Y���W</param>
        /// <param name="rollTime">�ړ��A��]�ɂ����鎞��</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async UniTask ScrollSlide(float startX, float goalX, float Y, float rollTime, CancellationToken cancellationToken = default)
        {
            float elapsedTime = 0;
            rectra.localPosition = new Vector3(startX, Y, rectra.localPosition.z);
            while (elapsedTime < rollTime)
            {
                // �L�����Z������Ă�����OperationCanceledException���X���[
                cancellationToken.ThrowIfCancellationRequested();
                elapsedTime += Time.deltaTime;
                float x = easeOutCubic(elapsedTime, goalX, startX, rollTime);
                rectra.anchoredPosition = new Vector2(x, Y);
                ScrollObject.transform.eulerAngles = new Vector3(0, -(x - startX) * rotSpeed, 0);
                await UniTask.Delay(1);
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

        /// <summary>
        /// �������������ʒu�ɖ߂�
        /// </summary>
        /// <param name="startX">�����̏����ʒu</param>
        public void ResetObject(float startX)
        {
            rectra.localPosition = new Vector3(startX, rectra.localPosition.y, rectra.localPosition.z);
            ScrollObject.transform.eulerAngles = initRot;
        }

        /// <summary>
        /// ������Material��ύX����
        /// </summary>
        /// <param name="nowStage">�����ɔ��f�����鍑</param>
        public void ChangeMaterial(Stage nowStage)
        {
            ScrollMaterial.material = FlagMaterials[(int)nowStage];
        }
    }
}
