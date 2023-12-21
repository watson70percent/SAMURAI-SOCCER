using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SamuraiSoccer.Event;
using UniRx;

namespace SamuraiSoccer.Player
{
    //�U���C�x���g��������Slash�v���n�u��ݒu
    public class PlayerAttack : MonoBehaviour
    {
        public GameObject slash;
        // Start is called before the first frame update
        void Start()
        {
            PlayerEvent.Attack.Subscribe(x => { Attack(); }).AddTo(this);
        }


        void Attack()
        {
            Instantiate(slash, transform.position, transform.rotation);
            PlayerEvent.FaulCheckOnNext();//�R���ɂ��t�@�[���`�F�b�N
        }
    }
}
