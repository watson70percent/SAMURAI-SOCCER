using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            if(staffIndex >= StaffList.Length)
            {
                is_staffEnd = true;
            }
        }
    }
}
