using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using UnityEngine.SceneManagement;

namespace SamuraiSoccer.SoccerGame
{
    public class EndingManagerScript : MonoBehaviour
    {
        [SerializeField]
        Material[] StaffList;
        [SerializeField]
        GameObject test;
        [SerializeField]
        float Interval;
        [SerializeField]
        float velocity;
        [SerializeField]
        GameObject sponeSpot;

        GameObject staff;
        Rigidbody rb;
        Material material;
        float elapsedTime;
        int staffIndex=0;
        bool is_staffEnd;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > Interval && !is_staffEnd)
            {
                elapsedTime = 0;
                staff = Instantiate(test,sponeSpot.transform.position,sponeSpot.transform.rotation);
                staff.GetComponent<Rigidbody>().velocity = Vector3.down * velocity;
                staff.GetComponent<MeshRenderer>().material = StaffList[staffIndex];
                staffIndex++;
            }
            if(staffIndex >= StaffList.Length && !is_staffEnd)
            {
                is_staffEnd = true;
                ReturnTitle();
            }
        }
        
        async UniTask ReturnTitle()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(10)); //最後のパネルを流してから5秒後にシーン遷移
            SceneManager.LoadScene("Start");
        }
    }
}
