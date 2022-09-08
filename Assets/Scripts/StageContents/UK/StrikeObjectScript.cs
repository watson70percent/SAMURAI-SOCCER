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
        [SerializeField] float emergencyInterval = 2.0f; //警告時間間隔
        float elapsedTime;//生成後経過した時間
        [SerializeField] GameObject ShotObject;//生成するobject
        //float[] shotPosZ = { 15.5f, 50, 84.3f };//グラウンドの道路の座標
        [SerializeField] Transform[] Cannons;//生成したい場所にオブジェクト置いてアタッチ
        [SerializeField] GameObject[] emergencySign;
        [SerializeField] Transform player;

        int index, maxIndex;
        bool isEmerge = false;
        Vector3 emergePos;

        private void Start()
        {
            maxIndex = Cannons.Length;
            InGameEvent.Standby.Subscribe(_ =>
            {
                elapsedTime = 0;
            }).AddTo(this);
            InGameEvent.UpdateDuringPlay.Subscribe(_ =>
            {//時間が経ったらobject生成まで警告
                if (elapsedTime >= shotInterval - emergencyInterval && !isEmerge) ShowEmerge();
                //時間が経ったらobject生成
                if (elapsedTime >= shotInterval) Shot();
                //時間増やす
                elapsedTime += Time.deltaTime;
            }).AddTo(this);
        }

        //時間が経ったらobject生成まで警告
        void ShowEmerge()
        {
            index = (int)Random.Range(0, maxIndex);
            isEmerge = true;
            emergePos = emergencySign[index].transform.position;
            emergePos.x = player.position.x + 1;
            emergencySign[index].transform.position = emergePos;
            emergencySign[index].SetActive(true);
        }
        //時間が経ったらobject生成
        void Shot()
        {
            Debug.Log("L51 index =" + index);
            Instantiate(ShotObject, Cannons[index].position, Cannons[index].rotation);
            emergencySign[index].SetActive(false);
            elapsedTime = 0.0f;
            isEmerge = false;
        }
    }
}