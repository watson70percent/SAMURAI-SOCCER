using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRX;
using SamuraiSoccer.Event;

namespace SamuraiSoccer.UK
{
    public class StrikeObjectScript : MonoBehaviour
    {
        [SerializedField] float shotInterval = 4.0f; //障害物生成間隔
        [SerializedField] float emergencyInterval = 2.0f; //警告時間間隔
        float elapsedTime;//生成後経過した時間
        [SerializedField] GameObject ShotObject;//生成するobject
        //float[] shotPosZ = { 15.5f, 50, 84.3f };//グラウンドの道路の座標
        [SerializedField] Transform[] Cannons;//生成したい場所にオブジェクト置いてアタッチ
        [SerializedField] GameObject emergencySign;
        [SerialisedField] Transform player;

        int index, maxIndex;
        Vector3 emergePos;

        private void Start()
        {
            maxIndex = Cannons.Length;
            InGameEvent.Standby.Subscribe(_ =>
            {
                elapsedTime = 0;
            });
            InGameEvent.UpdateDuringPlay(_ =>
            {//時間が経ったらobject生成まで警告
                if (elapsedTime >= shotInterval - emergencyInterval) ShowEmerge();
                //時間が経ったらobject生成
                if (elapsedTime >= shotInterval) Shot();
                //時間増やす
                elapsedTime += Time.deltaTime;
            });

        }

        //時間が経ったらobject生成まで警告
        void ShowEmerge()
        {
            emergePos.x = player.position.x + 1;
            emergencySign.transform.position = player.position;
            emergencySign.SetActive(true);
        }
        //時間が経ったらobject生成
        void Shot()
        {
            index = (int)RandomRange(0, maxIndex);
            Instantiate(ShotObject, Cannons[index].position, Cannons[index].rotation);
            emergencySign.SetActive(false);
            elapsedTime = 0.0f;
        }



    }
}