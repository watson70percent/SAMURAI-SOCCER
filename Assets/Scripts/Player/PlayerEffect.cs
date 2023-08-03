using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SamuraiSoccer.Event;
using UniRx;


namespace SamuraiSoccer.Player
{
    public class PlayerEffect : MonoBehaviour
    {
        [SerializeField]
        GameObject aura;
        // ため攻撃ができるときにはオーラを発するように
        void Start()
        {
            PlayerEvent.IsEnableChargeAttack.Subscribe(isEnableAttack => { aura.SetActive(isEnableAttack); }).AddTo(this);

        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
