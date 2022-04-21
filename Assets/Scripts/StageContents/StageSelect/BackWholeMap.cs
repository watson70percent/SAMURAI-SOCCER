using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BackWholeMap : MonoBehaviour
{
    [SerializeField]
    //全体世界地図用のvirtualCamera
    private CinemachineVirtualCamera _wholeMapVirtualCamera;
    public CinemachineVirtualCamera WholeMapVirtualCamera
    {
        get { return _wholeMapVirtualCamera; }
        private set { _wholeMapVirtualCamera = value; }
    }
}
