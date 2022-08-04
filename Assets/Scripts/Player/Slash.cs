using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SamuraiSoccer.SoccerGame.AI;

namespace SamuraiSoccer.Player
{
    public class Slash : MonoBehaviour
    {

        public Animator animator;
        ParticleSystem.MainModule particle;
        float time;
        float alpha;
        public AudioClip slash;
        // Start is called before the first frame update
        void Start()
        {
            particle = GetComponent<ParticleSystem>().main;
            alpha = particle.startColor.color.a;
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


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<EasyCPU>()?.status.ally == false)
            {
                var dir = other.transform.position - transform.position;
                other.gameObject.GetComponent<Rigidbody>().AddForce(dir * 1000);
                GameObject.FindGameObjectWithTag("Referee").GetComponent<AudioSource>().PlayOneShot(slash);
                other.GetComponent<EasyCPU>().Attacked();
            }
            else if (other.gameObject.tag == "TutorialEnemy")
            {
                var dir = other.transform.position - transform.position;
                other.gameObject.GetComponent<Rigidbody>().AddForce(dir * 1000);
                GameObject.FindGameObjectWithTag("Referee").GetComponent<AudioSource>().PlayOneShot(slash);
            }
        }
    }

}