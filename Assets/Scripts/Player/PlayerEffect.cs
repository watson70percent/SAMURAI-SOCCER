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
        // Start is called before the first frame update
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
