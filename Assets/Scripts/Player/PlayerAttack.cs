using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SamuraiSoccer.Event;
using UniRx;

namespace SamuraiSoccer.Player
{
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
        }
    }
}
