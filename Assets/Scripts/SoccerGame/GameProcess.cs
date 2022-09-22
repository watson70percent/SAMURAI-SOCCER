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
        [Tooltip("ほら貝のSE")]
        private int m_trumpetShellSENumber;

        [SerializeField]
        [Tooltip("ホイッスル開始音")]
        private int m_whistleSENumber;

        [SerializeField]
        [Tooltip("ホイッスル音(短い)")]
        private int m_shortWhistleSENumber;

        [SerializeField]
        [Tooltip("ゲームのBGM")]
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
            SoundMaster.Instance.PlaySE(m_trumpetShellSENumber); //ほら貝のSEを流す
            await UniTask.Delay(6000);
            SoundMaster.Instance.PlayBGM(m_gameBGM); //ゲームのBGMを流す
            InGameEvent.StandbyOnNext();
        }

        private async UniTask StandbyContents()
        {
            await UniTask.Delay(500);
            SoundMaster.Instance.PlaySE(m_whistleSENumber); //ホイッスルのSEを流す
            InGameEvent.PlayOnNext();
        }

        private async UniTask GoalContents()
        {
            SoundMaster.Instance.PlaySE(m_whistleSENumber); //ホイッスルのSEを流す
            await UniTask.Delay(m_goalDelayMiliSec);
            m_goalFadeAnimator.SetTrigger("Fade");
            await UniTask.Delay(500);
            InGameEvent.StandbyOnNext();
        }

        public async UniTask FinishContents()
        {
            //シーンの移動処理
            await UniTask.Delay(m_finishDelayMiliSec);
            Time.timeScale = 1;
            SceneManager.LoadScene(m_resultSceneName);
        }

    }
}

