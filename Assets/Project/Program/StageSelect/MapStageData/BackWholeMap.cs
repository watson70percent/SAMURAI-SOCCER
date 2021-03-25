using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(StageSelectCamera))]
public class BackWholeMap : MonoBehaviour
{
    [SerializeField]
    private float _cameraDistance = 1000f;//カメラと対象オブジェクトとの距離
    [SerializeField]
    private CinemachineVirtualCamera _WholeMapVirtualCamera;//全体世界地図用のvirtualCamera

    private StageSelectCamera _selectCamera;

    private void Start()
    {
        _WholeMapVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = _cameraDistance;
        _selectCamera = GetComponent<StageSelectCamera>();
    }

    public void Back()
    {
        _selectCamera.ChangeCameraView(_WholeMapVirtualCamera);
    }
}
