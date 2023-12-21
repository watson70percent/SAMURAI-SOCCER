using SamuraiSoccer.SoccerGame.AI;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SamuraiSoccer.UI
{
    public class OpponentNumber : MonoBehaviour
    {
        [SerializeField]
        private Text m_remainNumberText; //Žc‚è“G”•\Ž¦

        [SerializeField]
        private EasyCPUManager m_easyCPUManager;

        // Start is called before the first frame update
        void Start()
        {
            m_easyCPUManager.ObserveEveryValueChanged(x => x.OpponentMemberCount).Subscribe(_ =>
            {
                m_remainNumberText.text = m_easyCPUManager.OpponentMemberCount.ToString();
            });
        }

    }
}
