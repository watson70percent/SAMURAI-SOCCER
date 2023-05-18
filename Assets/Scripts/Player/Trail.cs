using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SamuraiSoccer.SoccerGame.AI;
using UniRx.Triggers;
using UniRx;
using Cysharp.Threading.Tasks;

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

            
            if (obj.GetComponent<EasyCPU>()?.status.ally == false) //敵にあたったとき
            {
                var dir = obj.transform.position - transform.position;
                obj.gameObject.GetComponent<Rigidbody>().AddForce(dir * 1000);
                GameObject.FindGameObjectWithTag("Referee").GetComponent<AudioSource>().PlayOneShot(slash);
                obj.GetComponent<EasyCPU>().Attacked();
            }
            else if (obj.gameObject.tag == "TutorialEnemy") // チュートリアル用
            {
                var dir = obj.transform.position - transform.position;
                obj.gameObject.GetComponent<Rigidbody>().AddForce(dir * 1000);
                GameObject.FindGameObjectWithTag("Referee").GetComponent<AudioSource>().PlayOneShot(slash);
            }
            else if (obj.gameObject.tag == "Ball" ) // ボールに当たったとき
            {
                var dir = obj.transform.position - transform.position;
                dir = dir.normalized;
                dir = new Vector3(dir.x, 0.3f, dir.z);
                obj.gameObject.GetComponent<Rigidbody>().AddForce(dir * 1000);


            }
        }

        async UniTask Vanish()
        {
            await UniTask.Delay(1000);
            Destroy(gameObject);
        }
    }
}
