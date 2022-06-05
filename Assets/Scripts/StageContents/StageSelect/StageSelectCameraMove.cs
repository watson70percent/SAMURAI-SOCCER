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
        private List<StageCamera> m_stageCameras; //シーン内のカメラ群
        [SerializeField]
        private CinemachineBrain m_cinemachineBrain;//メインカメラについているCinemachineBrain
        private bool m_canTouchWorld = true; //カメラ移動可能かどうか

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
        /// 指定したステージのカメラ優先度を高くしてフォーカスする
        /// </summary>
        /// <param name="stage">指定するステージ</param>
        /// <returns></returns>
        public async UniTask ChangeCameraView(Stage stage)
        {
            //カメラ移動中は追加の移動を不可に
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
            //カメラの移動が終わるまで待機
            await UniTask.Delay((int)(m_cinemachineBrain.m_DefaultBlend.BlendTime * 1000));
            m_canTouchWorld = true;
        }
    }

    /// <summary>
    /// 各ステージをフォーカスするカメラ
    /// </summary>
    [System.Serializable]
    public class StageCamera
    {
        public Stage stage; //どのステージか
        public CinemachineVirtualCamera camera; //そのステージで使用されているカメラ

    }

}
