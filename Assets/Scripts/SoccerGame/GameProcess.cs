using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using SamuraiSoccer.Event;
using SamuraiSoccer;
using UnityEngine.SceneManagement;

namespace SamuraiSoccer.Event
{
    public class GameProcess : MonoBehaviour
    {
        [SerializeField]
        private string m_resultSceneName = "Result";

        [SerializeField]
        private int m_goalDelayMiliSec = 5000;

        [SerializeField]
        private int m_finishDelayMiliSec = 1000;

        [SerializeField]
        private Animator m_goalFadeAnimator;

        [SerializeField]
        [Tooltip("�ق�L��SE")]
        private int m_trumpetShellSENumber;

        [SerializeField]
        [Tooltip("�z�C�b�X���J�n��")]
        private int m_whistleSENumber;

        [SerializeField]
        [Tooltip("�z�C�b�X����(�Z��)")]
        private int m_shortWhistleSENumber;

        [SerializeField]
        [Tooltip("�Q�[����BGM")]
        private int m_gameBGM;

        // Start is called before the first frame update
        async void Start()
        {
            InGameEvent.Reset.Subscribe(async _ =>
            {
                await ResetContents();
            });

            InGameEvent.Standby.Subscribe(async _ =>
            {
                await StandbyContents();
            });

            InGameEvent.Goal.Subscribe(async _ =>
            {
                await GoalContents();
            });

            InGameEvent.Finish.Subscribe(async _ =>
            {
                await FinishContents();
            });

            InGameEvent.ResetOnNext();
        }

        private async UniTask ResetContents()
        {
            await UniTask.Delay(1000);
            SoundMaster.Instance.PlaySE(m_trumpetShellSENumber); //�ق�L��SE�𗬂�
            await UniTask.Delay(6000);
            SoundMaster.Instance.PlayBGM(m_gameBGM); //�Q�[����BGM�𗬂�
            InGameEvent.StandbyOnNext();
        }

        private async UniTask StandbyContents()
        {
            await UniTask.Delay(500);
            SoundMaster.Instance.PlaySE(m_whistleSENumber); //�z�C�b�X����SE�𗬂�
            InGameEvent.PlayOnNext();
        }

        private async UniTask GoalContents()
        {
            SoundMaster.Instance.PlaySE(m_whistleSENumber); //�z�C�b�X����SE�𗬂�
            await UniTask.Delay(m_goalDelayMiliSec);
            m_goalFadeAnimator.SetTrigger("Fade");
            await UniTask.Delay(500);
            InGameEvent.StandbyOnNext();
        }

        public async UniTask FinishContents()
        {
            //�V�[���̈ړ�����
            await UniTask.Delay(m_finishDelayMiliSec);
            Time.timeScale = 1;
            SceneManager.LoadScene(m_resultSceneName);
        }

    }
}

