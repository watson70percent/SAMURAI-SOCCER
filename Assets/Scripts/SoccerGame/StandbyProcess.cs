using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using SamuraiSoccer.Event;
using SamuraiSoccer;

namespace SamuraiSoccer.Event
{
    public class StandbyProcess : MonoBehaviour
    {
        // Start is called before the first frame update
        async void Start()
        {
            InGameEvent.Standby.Subscribe(async _ =>
            {
                await StandbyContents();
            });
        }

        private async UniTask StandbyContents()
        {
            await UniTask.Delay(1000);
            SoundMaster.Instance.PlaySE(0); //������SE�𗬂�
            await UniTask.Delay(6000);
            SoundMaster.Instance.PlaySE(1); //�z�C�b�X����SE�𗬂�
            await UniTask.Delay(1000);
            SoundMaster.Instance.PlayBGM(0); //�Q�[����BGM�𗬂�
        }
    }
}

