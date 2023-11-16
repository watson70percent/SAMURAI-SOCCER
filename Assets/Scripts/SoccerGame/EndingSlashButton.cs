using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SamuraiSoccer;

namespace SamuraiSoccer.SoccerGame
{
    public class EndingSlashButton : MonoBehaviour
    {

        [SerializeField]
        GameObject m_slash;
        [SerializeField]
        GameObject slash_parent;

        public void OnPushAttackButton()
        {
            Instantiate(m_slash,slash_parent.transform.position, slash_parent.transform.rotation);
        }
    }
}
