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
        [Tooltip("ほら貝のSE、-1の時はなにも流さない")]
        private int m_trumpetShellSENumber;

        [SerializeField]
        [Tooltip("ホイッスル開始音、-1の時はなにも流さない")]
        private int m_whistleSENumber;

        [SerializeField]
        [Tooltip("ゲームのBGM、-1の時はなにも流さない")]
        private int m_gameBGM;

        private void Awake()
        {
            InGameEvent.Reset.Subscribe(async _ =>
            {
                await ResetContents();
            }).AddTo(this);
        }

        // Start is called before the first frame update
        private void Start()
        {
            InGameEvent.Standby.First().Subscribe(async _ =>
            {
                await FirstStandbyContents();
            }).AddTo(this);

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
            if (m_gameBGM != -1)
            {
                SoundMaster.Instance.PlayBGM(m_gameBGM); //ゲームのBGMを流す
            }
            InGameEvent.PlayOnNext();
        }
    }
}

