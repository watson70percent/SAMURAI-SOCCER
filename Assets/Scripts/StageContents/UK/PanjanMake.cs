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
    [SerializeField] Transform player;
    [SerializeField] GameObject fire;
    bool panjanExist;
    bool isEnd;
    Vector3 respone = new Vector3(30, 2, 95);
    Quaternion quaternion = new Quaternion(0,1,0,0);

    // Start is called before the first frame update
        private void Start()
        {
            InGameEvent.Play.Subscribe(_ =>
            {
                PanjanRoll panjanRoll = Instantiate(panjan,respone,quaternion).GetComponent<PanjanRoll>();
                panjanRoll.SetObjects(fire,player);
            }).AddTo(this);
        }
    }
}