using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SamuraiSoccer.Event;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;

namespace SamuraiSoccer.UK
{
    public class PanjanExplode : MonoBehaviour
    {
        bool isBurn = true;
        PanjanMake panjanMake;
        GameObject fire;
        // Start is called before the first frame update
        void Start()
        {
            InGameEvent.Goal.Subscribe(_ =>
            {
                isBurn = false;
            }).AddTo(this);
            this.OnCollisionEnterAsObservable()
            .Select(hit => hit.gameObject.tag)
            .Where(tag => tag == "Player")
            .Subscribe(_ => {
                if(isBurn){
                    fire.SetActive(true);
                    InGameEvent.FinishOnNext();
                }
            }).AddTo(this);
        }

        public void SetFireObject(GameObject fire){
            this.fire = fire;
        }
    }
}