using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SamuraiSoccer.SoccerGame;

namespace SamuraiSoccer.StageContents.China
{
    public class KunfuSceneManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_referee, m_prefab;
        [SerializeField]
        private GameObject m_kunfu; //審判とすげ替えるカンフー
        private void Start()
        {
            //レフェリーを消してカンフーを入れる
            m_kunfu = Instantiate(m_prefab);
            m_kunfu.transform.parent = m_referee.transform.parent;
            m_kunfu.transform.localPosition = m_referee.transform.localPosition;

            var parent = m_referee.transform.parent;
            Destroy(parent.GetComponent<RefereeMove>());
            parent.gameObject.AddComponent<KunfuReferee>();
            Destroy(m_referee);
        }

        private void Update()
        {
            float height = m_kunfu.transform.position.y;
        }
    }
}