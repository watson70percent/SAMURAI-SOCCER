using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SamuraiSoccer.Event;
using Cinemachine;
using UniRx;
using Cysharp.Threading.Tasks;

namespace SamuraiSoccer.StageContents.StageSelect
{
    public class StageSelectCameraMove : MonoBehaviour
    {
        [SerializeField]
        private List<StageCamera> m_stageCameras; //�V�[�����̃J�����Q
        [SerializeField]
        private CinemachineBrain m_cinemachineBrain;//���C���J�����ɂ��Ă���CinemachineBrain
        private bool m_canTouchWorld = true; //�J�����ړ��\���ǂ���

        // Start is called before the first frame update
        void Start()
        {
            StageSelectEvent.World.Subscribe(async _ =>
            {
                if (m_canTouchWorld)
                {
                    SoundMaster.Instance.PlaySE(0);
                    await ChangeCameraView(Stage.World);
                }
            });
            StageSelectEvent.Stage.Subscribe(async x =>
            {
                if (m_canTouchWorld)
                {
                    SoundMaster.Instance.PlaySE(0);
                    await ChangeCameraView(x);
                }
            });
        }

        /// <summary>
        /// �w�肵���X�e�[�W�̃J�����D��x���������ăt�H�[�J�X����
        /// </summary>
        /// <param name="stage">�w�肷��X�e�[�W</param>
        /// <returns></returns>
        public async UniTask ChangeCameraView(Stage stage)
        {
            //�J�����ړ����͒ǉ��̈ړ���s��
            m_canTouchWorld = false;
            foreach (StageCamera stageCamera in m_stageCameras)
            {
                if (stageCamera.stage == stage)
                {
                    stageCamera.camera.Priority = 11;
                }
                else
                {
                    stageCamera.camera.Priority = 10;
                }
            }
            //�J�����̈ړ����I���܂őҋ@
            await UniTask.Delay((int)(m_cinemachineBrain.m_DefaultBlend.BlendTime * 1000));
            m_canTouchWorld = true;
        }
    }

    /// <summary>
    /// �e�X�e�[�W���t�H�[�J�X����J����
    /// </summary>
    [System.Serializable]
    public class StageCamera
    {
        public Stage stage; //�ǂ̃X�e�[�W��
        public CinemachineVirtualCamera camera; //���̃X�e�[�W�Ŏg�p����Ă���J����

    }

}
