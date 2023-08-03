using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using SamuraiSoccer.Event;

namespace SamuraiSoccer.StageContents.UK
{
    public class StrikeObjectScript : MonoBehaviour
    {
        [SerializeField] float shotInterval = 4.0f; //障害物生成間隔
        float elapsedTime;//生成後経過した時間
        [SerializeField] GameObject ShotObject;//生成するobject
        [SerializeField] Transform[] Cannons;//生成したい場所にオブジェクト置いてアタッチ
        [SerializeField] Transform player;

        int index, maxIndex;

        private void Start()
        {
            maxIndex = Cannons.Length;
            InGameEvent.Standby.Subscribe(_ =>
            {
                elapsedTime = 0;
            }).AddTo(this);
            InGameEvent.UpdateDuringPlay.Subscribe(_ =>
            {
                //時間が経ったらobject生成
                if (elapsedTime >= shotInterval) Shot();
                //時間増やす
                elapsedTime += Time.deltaTime;
            }).AddTo(this);
        }

        //時間が経ったらobject生成
        void Shot()
        {
            index = (int)Random.Range(0, maxIndex);
            Instantiate(ShotObject, Cannons[index].position, Cannons[index].rotation);
            elapsedTime = 0.0f;
        }
    }
}