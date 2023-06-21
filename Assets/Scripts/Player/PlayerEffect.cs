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
        // ‚½‚ßUŒ‚‚ª‚Å‚«‚é‚Æ‚«‚É‚ÍƒI[ƒ‰‚ð”­‚·‚é‚æ‚¤‚É
        void Start()
        {
            PlayerEvent.IsEnableChargeAttack.Subscribe(isEnableAttack => { aura.SetActive(isEnableAttack); });

        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
