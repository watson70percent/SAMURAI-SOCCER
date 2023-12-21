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
            //初期状態としてWorldを発行しておき保留、ストリームが発行されるたびに前回の状態と比較して変更があれば処理する
            StageSelectEvent.Stage.StartWith(Stage.World).Where(_=> m_canTouchWorld).Buffer(2, 1)
                .Where(stage => stage[0] != stage[1]).Subscribe(async stage =>
            {
                SoundMaster.Instance.PlaySE(0);
                await ChangeCameraView(stage[1]);
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
