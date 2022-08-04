using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SamuraiSoccer.Event;
using UniRx;



namespace SamuraiSoccer.Player
{
    public class AnimationController : MonoBehaviour
    {
        Animator animator;
        public GameObject slash;

        bool m_isEnableAttack = false;

        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();
            InGameEvent.Play.Subscribe(x=> { m_isEnableAttack = true; }).AddTo(this);
            InGameEvent.Goal.Subscribe(x=> { m_isEnableAttack = false; }).AddTo(this);
            InGameEvent.Finish.Subscribe(x=> { m_isEnableAttack = false; }).AddTo(this);
            PlayerEvent.Attack.Subscribe(x => { Attack(); }).AddTo(this);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Attack()
        {
            if (!m_isEnableAttack) { return; }
            animator.SetTrigger("Attack");
            Instantiate(slash, transform.position, transform.rotation);
        }

        public void Run()
        {

            animator.SetBool("Run", true);

        }

        public void Stay()
        {
            animator.SetBool("Run", false);
        }
    }

}