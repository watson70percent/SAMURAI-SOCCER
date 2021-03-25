using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TargetWorldData : MonoBehaviour
{
    [SerializeField]
    private float _cameraDistance = 5f;//カメラと対象オブジェクトとの距離
    public float FocusStrength
    {
        get { return _cameraDistance; }
        private set { _cameraDistance = value; }
    }

    private void Start()
    {
        GetComponentInChildren<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = _cameraDistance;
    }
}
