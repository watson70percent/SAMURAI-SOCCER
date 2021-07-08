using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Cinemachine;
using System.Linq;

[RequireComponent(typeof(SelectStateManager))]
public class SelectStateInput : MonoBehaviour
{
    public StageSelectBGM bgm;
    public GameObject backWholeMap;
    private List<CinemachineVirtualCamera> _virtualCameras = new List<CinemachineVirtualCamera>();//StageSelectに使用するカメラ群
    private CinemachineBrain _cinemachineBrain;//メインカメラについているやつ
    private WorldName _focusWorld = WorldName.WholeMap;//今どのワールドを見ているのか

    private bool _canTouchWorld = true;//ワールド移動可能かどうか
    public bool CanTouchWorld
    {
        get { return _canTouchWorld; }
        private set { _canTouchWorld = value; }
    }

    private SelectStateManager _selectStateManager;

    /// <summary>
    /// _virtualCamerasをシーン内から取得+必要なスクリプトの取得
    /// </summary>
    private void Start()
    {
        GameObject[] _worldObjs = GameObject.FindGameObjectsWithTag("VirtualCamera");
        for (int i = 0; i < _worldObjs.Length; i++)
        {
            _virtualCameras.Add(_worldObjs[i].GetComponent<CinemachineVirtualCamera>());
        }
        _cinemachineBrain = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineBrain>();
        _selectStateManager = GetComponent<SelectStateManager>();
        backWholeMap.SetActive(false);
    }

    /// <summary>
    /// SelectStateStateがWorldSelectStateのときの処理
    /// </summary>
    public void WorldSelectStateUpdate()
    {
        //一度も画面をタップしていないときに以下がエラーを吐かないように
        if (Input.touchCount <= 0)
        {
            return;
        }
        //画面をタップした瞬間のみRayを飛ばす
        if (Input.GetTouch(0).phase == TouchPhase.Began && CanTouchWorld)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            foreach (RaycastHit raycastHit in Physics.RaycastAll(ray, 2000.0f))
            {
                //当たったオブジェクトが適切なものなら画面を拡大し、StageSelect画面へ
                if (raycastHit.collider.gameObject.tag == "TargetWorld" && raycastHit.collider.gameObject.GetComponent<TargetWorldData>().WorldName != _focusWorld)
                {
                    StartCoroutine(ChangeCameraView(raycastHit.collider.gameObject.GetComponentInChildren<CinemachineVirtualCamera>(), _cinemachineBrain.m_DefaultBlend.m_Time));
                    _focusWorld = raycastHit.collider.gameObject.GetComponent<TargetWorldData>().WorldName;
                    bgm.ChangeBGM(_focusWorld, _cinemachineBrain.m_DefaultBlend.m_Time);
                    _selectStateManager.StateChangeSignal(SelectState.StageSelect);
                    backWholeMap.SetActive(true);
                }
            }
        }

    }

    /// <summary>
    /// SelectStateStateがStageSelectStateのときの処理
    /// </summary>
    public void StageSelectStateUpdate()
    {
        //一度も画面をタップしていないときに以下がエラーを吐かないように
        if (Input.touchCount <= 0)
        {
            return;
        }
        //画面をタップした瞬間のみRayを飛ばす
        if (Input.GetTouch(0).phase == TouchPhase.Began && CanTouchWorld)
        {
            //UIについて
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = Input.GetTouch(0).position;
            List<RaycastResult> result = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, result);

            foreach (RaycastResult raycastResult in result)
            {
                //当たったオブジェクトが適切なものなら画面を縮小し、WorldSelect画面へ
                if (raycastResult.gameObject.GetComponent<BackWholeMap>())
                {
                    StartCoroutine(ChangeCameraView(raycastResult.gameObject.GetComponent<BackWholeMap>().WholeMapVirtualCamera, _cinemachineBrain.m_DefaultBlend.m_Time));
                    _focusWorld = WorldName.WholeMap;
                    bgm.ChangeBGM(_focusWorld, _cinemachineBrain.m_DefaultBlend.m_Time);
                    _selectStateManager.StateChangeSignal(SelectState.WorldSelect);
                    backWholeMap.SetActive(false);
                }
                else
                {
                    Debug.Log("選択したUIにはBackWholeMapスクリプトがついていないので画面遷移はおこないません");
                }

            }

            //GameObjectについて
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            foreach (RaycastHit raycastHit in Physics.RaycastAll(ray, 2000.0f))
            {
                //当たったオブジェクトが適切なものならStagePreview画面へ
                if (raycastHit.collider.gameObject.tag == "TargetStage")
                {
                    if (raycastHit.collider.gameObject.GetComponent<StageData>().StageState != StageState.NotPlayable)
                    {
                        raycastHit.collider.gameObject.GetComponent<StageIconButton>().OnClick();
                        _selectStateManager.StateChangeSignal(SelectState.StagePreview);
                    }
                    else
                    {
                        //まだ挑戦できるステージではないのでバツっぽい効果音とか入れる
                        Debug.Log("まだ挑戦できるステージではありません");
                    }
                }
            }
        }
    }

    /// <summary>
    /// SelectStateStateがStagePreviewStateのときの処理
    /// </summary>
    public void StagePreviewStateUpdate()
    {
        //一度も画面をタップしていないときに以下がエラーを吐かないように
        if (Input.touchCount <= 0)
        {
            return;
        }
        //画面をタップした瞬間のみRayを飛ばす
        if (Input.GetTouch(0).phase == TouchPhase.Began && CanTouchWorld)
        {
            //UIについて
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = Input.GetTouch(0).position;
            List<RaycastResult> result = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, result);

            //当たったオブジェクトが適切なものなら試合開始orStageSelect画面へ戻る
            foreach (RaycastResult raycastResult in result)
            {
                if (raycastResult.gameObject.name == "Start" && raycastResult.gameObject.GetComponentInParent<StagePreviwScript>())
                {
                    raycastResult.gameObject.GetComponentInParent<StagePreviwScript>().OnClickStart();
                    _focusWorld = WorldName.WholeMap;
                    _selectStateManager.StateChangeSignal(SelectState.WorldSelect);
                }
                else if (raycastResult.gameObject.name == "Close" && raycastResult.gameObject.GetComponentInParent<StagePreviwScript>())
                {
                    raycastResult.gameObject.GetComponentInParent<StagePreviwScript>().OnClickClose();
                    _selectStateManager.StateChangeSignal(SelectState.StageSelect);
                }

            }
        }
    }

    /// <summary>
    ///  シーン内に存在するvirtualCameraと入力を比較してカメラの優先度を変更
    /// </summary>
    /// <param name="virtualCamera"></param>
    /// <param name="delayTime">移動完了までの時間</param>
    /// <returns></returns>
    public IEnumerator ChangeCameraView(CinemachineVirtualCamera virtualCamera, float delayTime)
    {
        CanTouchWorld = false;
        Debug.Log(_virtualCameras.Select(c => c.Priority.ToString()).Aggregate((now, next) => now + "," + next));
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
        yield return new WaitForSeconds(delayTime);
        CanTouchWorld = true;
    }

}
