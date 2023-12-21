using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace SamuraiSoccer.UI
{
    public class TargetWorldData : MonoBehaviour
    {
        [SerializeField]
        private float m_cameraDistance = 5f; //カメラと対象オブジェクトとの距離
        public float CameraDistance
        {
            get { return m_cameraDistance; }
            private set { m_cameraDistance = value; }
        }

        [SerializeField]
        private int m_stageNumber;
        public int StageNumber
        {
            get { return m_stageNumber; }
            private set { m_stageNumber = value; }
        }

        private void Start()
        {
            GetComponentInChildren<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = m_cameraDistance;
        }
    }
}

