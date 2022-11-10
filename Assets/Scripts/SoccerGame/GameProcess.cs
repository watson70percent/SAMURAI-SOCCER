using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using SamuraiSoccer.Event;
using SamuraiSoccer;

namespace SamuraiSoccer.SoccerGame
{
    public class GameProcess : MonoBehaviour
    {
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
            InGameEvent.Reset.Subscribe(async _ =>
            {
                await ResetContents();
            });

            InGameEvent.Standby.First().Subscribe(async _ =>
            {
                await FirstStandbyContents();
            });

            InGameEvent.ResetOnNext();
        }

        private async UniTask ResetContents()
        {
            await UniTask.Delay(1000);
            SoundMaster.Instance.PlaySE(m_trumpetShellSENumber); //ほら貝のSEを流す
            InGameEvent.StandbyOnNext();
        }

        private async UniTask FirstStandbyContents()
        {
            await UniTask.Delay(6000); 
            SoundMaster.Instance.PlaySE(m_whistleSENumber); //ホイッスルのSEを流す
            await UniTask.Delay(500);
            SoundMaster.Instance.PlayBGM(m_gameBGM); //ゲームのBGMを流す
            InGameEvent.PlayOnNext();
        }
    }
}

