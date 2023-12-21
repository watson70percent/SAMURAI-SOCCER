using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SamuraiSoccer.SoccerGame.AI;
using UniRx.Triggers;
using UniRx;
using SamuraiSoccer.SoccerGame;
using System.Linq;

namespace SamuraiSoccer.Player
{
    public class Slash : MonoBehaviour
    {

        public Animator animator;
        ParticleSystem.MainModule particle;
        float time;
        float alpha;
        public AudioClip slash;
        bool m_isAttackBall = true;
        // Start is called before the first frame update
        void Start()
        {
            particle = GetComponent<ParticleSystem>().main;
            alpha = particle.startColor.color.a;

            this.OnTriggerEnterAsObservable().Subscribe(hit => OnHit(hit.gameObject));

        }


        // Update is called once per frame
        void Update()
        {
            time += Time.deltaTime;
            particle.startColor = new Color(particle.startColor.color.r, particle.startColor.color.g, particle.startColor.color.b, alpha - time);


            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Finish"))
            {
                Destroy(transform.root.gameObject);
            }


        }


        private void OnHit(GameObject obj)
        {
            // 当たった対象のinterfaceごとに処理が切り替わる。
            var dir = ((obj.transform.position - transform.position).normalized+3f*transform.forward).normalized;
            obj.GetComponents<ISlashed>().ToList().ForEach(x => x.Slashed(dir));
        }
    }
}