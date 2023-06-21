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

        public AudioClip slash;
        // Start is called before the first frame update
        void Start()
        {
            this.OnTriggerEnterAsObservable().Subscribe(hit => OnHit(hit.gameObject));
            Vanish();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnHit(GameObject obj)
        {
            // “–‚½‚Á‚½‘ÎÛ‚Ìinterface‚²‚Æ‚Éˆ—‚ªØ‚è‘Ö‚í‚éB
            var dir = (obj.transform.position - transform.position).normalized;
            obj.GetComponents<ISlashed>().ToList().ForEach(x => x.Slashed(dir));
        }

        async UniTask Vanish()
        {
            await UniTask.Delay(1000);
            Destroy(gameObject);
        }
    }
}
