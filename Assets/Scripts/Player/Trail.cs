using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SamuraiSoccer.SoccerGame.AI;
using UniRx.Triggers;
using UniRx;
using Cysharp.Threading.Tasks;
using SamuraiSoccer.SoccerGame;
using System.Linq;
using System.Threading;

namespace SamuraiSoccer.Player
{
    public class Trail : MonoBehaviour
    {
        // Start is called before the first frame update
        private void Start()
        {
            this.OnTriggerEnterAsObservable().Subscribe(hit => OnHit(hit.gameObject)).AddTo(this);
            Vanish(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private void OnHit(GameObject obj)
        {
            // ���������Ώۂ�interface���Ƃɏ������؂�ւ��B
            var dir = (obj.transform.position - transform.position).normalized;
            obj.GetComponents<ISlashed>().ToList().ForEach(x => x.Slashed(dir));
        }

        async UniTask Vanish(CancellationToken token)
        {
            await UniTask.Delay(1000);
            // �V�[�����܂����Ƃ���Trail���o���������ƃG���[�ɂȂ�
            token.ThrowIfCancellationRequested();
            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }
    }
}
