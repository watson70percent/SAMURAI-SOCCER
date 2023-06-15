using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using SamuraiSoccer.Event;

namespace SamuraiSoccer.UI
{
    public class AttackButton : MonoBehaviour
    {
        float m_pushTime = 0;
        float CHARGETIME = 1;
        bool m_isCharging = false;
        private void Update()
        {
            if (m_isCharging)
            {
                m_pushTime += Time.deltaTime;
                if (m_pushTime > CHARGETIME)
                {
                    PlayerEvent.SetIsEnableChargeAtack(true);
                }
            }
        }

        public void OnPushAttackButton()
        {
            if (!PlayerEvent.IsInChargeAttack.Value)
            {
                m_isCharging = true;
            }
        }


        public void OnLeaveAttackButton()
        {
            if (m_pushTime > CHARGETIME)
            {
                PlayerEvent.SetIsInChargeAtack(true);
            }
            else
            {
                PlayerEvent.AttackOnNext();
            }

            m_isCharging = false;
            m_pushTime = 0;
            PlayerEvent.SetIsEnableChargeAtack(false);
        }
    }
}
