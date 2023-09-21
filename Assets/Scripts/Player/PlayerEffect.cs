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
        bool m_isPlaying = false;
        // ため攻撃ができるときにはオーラを発するように
        void Start()
        {
            //PlayingState以外ではオーラを出さないように
            InGameEvent.Pause.Subscribe(isPause => { m_isPlaying = !isPause; }).AddTo(this);
            InGameEvent.Reset.Subscribe(x => { m_isPlaying=false; }).AddTo(this);
            InGameEvent.Standby.Subscribe(x => { m_isPlaying = false; }).AddTo(this);
            InGameEvent.Play.Subscribe(x => 
            {
                m_isPlaying = true;
                if (PlayerEvent.IsEnableChargeAttack.Value)
                {
                    aura.SetActive(true);
                }
            }).AddTo(this);
            InGameEvent.Finish.Subscribe(x => { m_isPlaying = false; }).AddTo(this);

            PlayerEvent.IsEnableChargeAttack.Subscribe(isEnableAttack => { if(m_isPlaying) aura.SetActive(isEnableAttack); }).AddTo(this);
        }
    }
}
