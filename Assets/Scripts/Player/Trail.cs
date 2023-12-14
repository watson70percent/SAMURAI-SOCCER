using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SamuraiSoccer.SoccerGame.AI;
using UniRx.Triggers;
using UniRx;
using Cysharp.Threading.Tasks;
using SamuraiSoccer.SoccerGame;
using System.Linq;

namespace SamuraiSoccer.Player
{
    public class Trail : MonoBehaviour
    {
        // Start is called before the first frame update
        private void Start()
        {
            this.OnTriggerEnterAsObservable().Subscribe(hit => OnHit(hit.gameObject)).AddTo(this);
            Vanish().Forget();
        }

        private void OnHit(GameObject obj)
        {
            // 当たった対象のinterfaceごとに処理が切り替わる。
            var dir = (obj.transform.position - transform.position).normalized;
            obj.GetComponents<ISlashed>().ToList().ForEach(x => x.Slashed(dir));
        }

        async UniTask Vanish()
        {
            await UniTask.Delay(1000);
            // シーンをまたぐときにTrailが出っ放しだとエラーになる
            if(gameObject != null)
            {
                Destroy(gameObject);
            }
        }
    }
}
