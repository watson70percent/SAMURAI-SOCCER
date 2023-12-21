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
        

        bool m_isEnableAttack = false;
        bool m_isRunning=false;
        bool m_isIdle = false;

        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();
            PlayerEvent.Attack.Subscribe(x => { Attack(); }).AddTo(this);
            PlayerEvent.StickInput.Subscribe(stickDir => {
                if (stickDir != Vector3.zero) {
                    if (!m_isRunning)
                    {
                        m_isRunning = true;
                        m_isIdle = false;
                        Run();
                    }
                }
                else
                {
                    if (!m_isIdle)
                    {
                        m_isIdle = true;
                        m_isRunning = false;
                        Stay();
                    }
                }
            }).AddTo(this);
        }



        public void Attack()
        {
            animator.SetTrigger("Attack");  
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