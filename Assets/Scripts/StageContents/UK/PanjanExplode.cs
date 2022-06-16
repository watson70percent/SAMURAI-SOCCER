using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SamuraiSoccer.Event;
using Cysharp.Threading.Tasks;

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

            });
            InGameEvent.Finish.Subscribe(_ =>
            {
                
            });
            InGameEvent.Goal.Subscribe(_ =>
            {
                isBurn = false;
            });
        }


        private void OnCollisionEnter(Collision other)
        {
            //衝突がプレイヤーだったらゲームオーバー
            if (isBurn && other.gameObject.tag == "Player")
            {
                fire.SetActive(true);
            }
        }

        public void SetFireObject(GameObject fire){
            this.fire = fire;
        }
    }
}