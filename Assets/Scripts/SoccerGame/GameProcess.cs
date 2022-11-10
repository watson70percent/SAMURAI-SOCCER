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
        [Tooltip("�ق�L��SE")]
        private int m_trumpetShellSENumber;

        [SerializeField]
        [Tooltip("�z�C�b�X���J�n��")]
        private int m_whistleSENumber;

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

            InGameEvent.Standby.First().Subscribe(async _ =>
            {
                await FirstStandbyContents();
            });

            InGameEvent.ResetOnNext();
        }

        private async UniTask ResetContents()
        {
            await UniTask.Delay(1000);
            SoundMaster.Instance.PlaySE(m_trumpetShellSENumber); //�ق�L��SE�𗬂�
            InGameEvent.StandbyOnNext();
        }

        private async UniTask FirstStandbyContents()
        {
            await UniTask.Delay(6000); 
            SoundMaster.Instance.PlaySE(m_whistleSENumber); //�z�C�b�X����SE�𗬂�
            await UniTask.Delay(500);
            SoundMaster.Instance.PlayBGM(m_gameBGM); //�Q�[����BGM�𗬂�
            InGameEvent.PlayOnNext();
        }
    }
}

