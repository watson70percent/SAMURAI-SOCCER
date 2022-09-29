using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SamuraiSoccer.SoccerGame.AI;
using UniRx.Triggers;
using UniRx;

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
            else if(obj.gameObject.tag == "Ball" && m_isAttackBall) // ボールに当たったとき
            {
                var dir = obj.transform.position - transform.position;
                dir = dir.normalized;
                dir = new Vector3(dir.x,0.3f,dir.z);
                obj.gameObject.GetComponent<Rigidbody>().AddForce(dir * 1000);
                m_isAttackBall = false;

                
            }
        }
    }

}