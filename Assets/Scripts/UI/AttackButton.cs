using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using SamuraiSoccer.Event;

namespace SamuraiSoccer.UI
{
    public class AttackButton : MonoBehaviour
    {
        public void OnPushAttackButton()
        {
            PlayerEvent.AttackOnNext();
        }
    }
}
