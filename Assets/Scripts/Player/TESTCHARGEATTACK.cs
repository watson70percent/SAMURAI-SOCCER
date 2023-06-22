using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SamuraiSoccer.Event;

namespace SamuraiSoccer.Player
{
    public class TESTCHARGEATTACK : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void OnClick()
        {
            PlayerEvent.SetIsInChargeAtack(true);
        }
    }
}
