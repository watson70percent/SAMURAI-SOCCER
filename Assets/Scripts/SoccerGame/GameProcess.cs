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
        private int m_delayMiliSec = 1000;

        [SerializeField]
        [Tooltip("ほら貝のSE")]
        private int m_trumpetShellSENumber;

        [SerializeField]
        [Tooltip("ホイッスル開始音")]
        private int m_whistleSENumber;

        [SerializeField]
        [Tooltip("ゲームのBGM")]
        private int m_gameBGM;

        // Start is called before the first frame update
        async void Start()
        {
            InGameEvent.Reset.Subscribe(_ =>
            {
                InGameEvent.StandbyOnNext();
            });

            InGameEvent.Standby.Subscribe(async _ =>
            {
                await StandbyContents();
            });

            InGameEvent.Finish.Subscribe(async _ =>
            {
                await FinishContents();
            });

            InGameEvent.ResetOnNext();
        }

        private async UniTask StandbyContents()
        {
            await UniTask.Delay(1000);
            SoundMaster.Instance.PlaySE(m_trumpetShellSENumber); //刀音のSEを流す
            await UniTask.Delay(6000);
            SoundMaster.Instance.PlaySE(m_whistleSENumber); //ホイッスルのSEを流す
            await UniTask.Delay(1000);
            SoundMaster.Instance.PlayBGM(m_gameBGM); //ゲームのBGMを流す
            InGameEvent.PlayOnNext();
        }

        public async UniTask FinishContents()
        {
            //シーンの移動処理
            await UniTask.Delay(m_delayMiliSec);
            Time.timeScale = 1;
            SceneManager.LoadScene(m_resultSceneName);
        }
    }
}

