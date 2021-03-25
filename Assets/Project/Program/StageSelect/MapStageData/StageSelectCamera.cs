using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class StageSelectCamera : MonoBehaviour
{
    private  List<CinemachineVirtualCamera> _virtualCameras = new List<CinemachineVirtualCamera>();//StageSelectに使用するカメラ群

    /// <summary>
    /// _virtualCamerasをシーン内から取得
    /// </summary>
    private void Start()
    {
        GameObject[] _worldObjs = GameObject.FindGameObjectsWithTag("VirtualCamera");
        for (int i = 0; i < _worldObjs.Length; i++)
        {
            _virtualCameras.Add(_worldObjs[i].GetComponent<CinemachineVirtualCamera>());
        }
    }
    /// <summary>
    /// 画面をタップしてワールドを移動するときの処理
    /// </summary>
    private void Update()
    {
        //一度も画面をタップしていないときに以下がエラーを吐かないように
        if (Input.touchCount <= 0)
        {
            return;
        }
        //画面をタップした瞬間のみRayを飛ばす
        if (Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit, 2000.0f) && raycastHit.collider.gameObject.tag == "TargetWorld")
            {
                ChangeCameraView(raycastHit.collider.gameObject.GetComponentInChildren<CinemachineVirtualCamera>());
            }
        }

    }

    /// <summary>
    /// シーン内に存在するvirtualCameraと入力を比較してカメラの優先度を変更
    /// </summary>
    /// <param name="virtualCamera"></param>
    public void ChangeCameraView(CinemachineVirtualCamera virtualCamera)
    {
        for (int i = 0; i < _virtualCameras.Count; i++)
        {
            if (_virtualCameras[i] == virtualCamera)
            {
                _virtualCameras[i].Priority = 11;
            }
            else
            {
                _virtualCameras[i].Priority = 10;
            }
        }
    }
}
