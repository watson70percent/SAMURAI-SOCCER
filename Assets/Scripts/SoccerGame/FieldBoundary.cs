using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiSoccer.SoccerGame
{
    public class FieldBoundary : MonoBehaviour
    {
        static float m_xMin, m_xMax, m_zMin, m_zMax;


        public static float XMin { get { return m_xMin; } }
        public static float XMax { get { return m_xMax; } }
        public static float ZMin { get { return m_zMin; } }
        public static float ZMax { get { return m_zMax; } }


        [SerializeField]
        GameObject m_minFlag, m_maxFlag;

        private void Start()
        {
            m_xMin = m_minFlag.transform.position.x;
            m_zMin = m_minFlag.transform.position.z;
            m_xMax = m_maxFlag.transform.position.x;
            m_zMax = m_maxFlag.transform.position.z;
        }
    }
}
