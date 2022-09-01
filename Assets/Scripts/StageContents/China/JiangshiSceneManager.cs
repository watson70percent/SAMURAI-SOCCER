using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SamuraiSoccer.SoccerGame;
using SamuraiSoccer.StageContents;

namespace SamuraiSoccer.StageContents.China
{
    public class JiangshiSceneManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_referee, m_prefab;
        private GameObject m_jianshi;
        [SerializeField]
        private RefereeMove m_refereeMove;
        [SerializeField]
        private RefereeArea m_refereeArea;
        private int m_areaSize, m_areaAngle;
        private void Awake()
        {
            //レフェリーを消して代わりにキョンシーをレフェリーに
            m_jianshi = Instantiate(m_prefab);
            m_jianshi.transform.parent = m_referee.transform.parent;
            m_jianshi.transform.localPosition = m_referee.transform.localPosition;
            Destroy(m_referee);

            m_refereeMove.anicon = m_jianshi.GetComponent<Animator>();
            var stagePrefabManager = GameObject.Find("DefaultStage").GetComponent<StagePrefabManager>();
            m_areaSize = stagePrefabManager.refereeAreaSize;
            m_areaAngle = stagePrefabManager.refereeMaxAng;
        }

        private void Update()
        {
            //アニメーションによるy座標の変化からRefereeAreaの大きさと角度を変更
            float height = m_jianshi.transform.position.y;
            m_refereeArea.SerAreaSize(m_areaSize + height * 20);

            var angle = m_areaAngle + height * 6;
            angle = Mathf.Min(angle, 180);
            m_refereeArea.SerMaxAngle(angle);
            m_refereeArea.MeshMaker();
        }

    }

}