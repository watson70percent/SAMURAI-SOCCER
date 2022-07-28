using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using SamuraiSoccer.Event;
using Cysharp.Threading.Tasks;

namespace SamuraiSoccer.UK
{
    public class PanjanMake : MonoBehaviour
    {
   [SerializeField]GameObject panjan;
    bool panjanExist;
    bool isEnd;
    Vector3 respone = new Vector3(30, 2, 95);
    Quaternion quaternion = new Quaternion(0,1,0,0);

    // Start is called before the first frame update
        private void Start()
        {
            InGameEvent.Reset.Subscribe(_ =>
            {

            });
            InGameEvent.Standby.Subscribe(_ =>
            {

            });
            InGameEvent.Pause.Subscribe(_ =>
            {

            });
            InGameEvent.Play.Subscribe(_ =>
            {
                Instantiate(panjan,respone,quaternion).GetComponent<PanjanRoll>();
            });
            InGameEvent.Finish.Subscribe(_ =>
            {

            });
            InGameEvent.Goal.Subscribe(_ =>
            {

            });
        }
    }
}