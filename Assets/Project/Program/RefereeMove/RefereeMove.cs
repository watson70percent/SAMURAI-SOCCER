using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefereeMove : MonoBehaviour
{
    public GameObject ball;
    public float lookatspeed;
    public float runningspeed;
    public float radius;
    Rigidbody rig;
    public RefereeArea refereeArea;
    public AnimationController animcon;
    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();

        animcon.AttackEvent += (sender, e) =>
        {
            var vec = ((AnimationController)sender).transform.position - transform.position;
            if (vec.magnitude < refereeArea.size)
            {
                if (Vector3.Dot(vec.normalized, transform.forward) > Mathf.Cos(refereeArea.maxang/360*2*Mathf.PI))
                {
                    print("yakki");
                }
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
        LookAtBall();
        MoveAroundBall();

    }


    void MoveAroundBall() {

        Vector3 dir = ball.transform.position - this.transform.position;
        dir.y = 0;
        if (dir.magnitude > radius) {

            if (dir.magnitude - radius < 5)
            {
                rig.transform.position += dir.normalized * runningspeed * Time.deltaTime*0.5f;

            }
            else
            {
            rig.transform.position += dir.normalized * runningspeed*Time.deltaTime;

            }

            


        }

    }



    void LookAtBall() {

        Vector3 dir = ball.transform.position - this.transform.position; //向きたい方向のベクトル
        Quaternion rotation = Quaternion.LookRotation(dir); //向きたい方向になったときの回転
        Quaternion rotate = rotation * Quaternion.Inverse(transform.rotation); //向きたい方向になるために必要な回転

        Vector3 eulerrotate = rotate.eulerAngles;
        eulerrotate.x = 0;
        eulerrotate.z = 0;
        eulerrotate.y = (eulerrotate.y - 180 <= 0) ? eulerrotate.y % 360 : ((360 - eulerrotate.y) % 360) * -1; //0~360°で角度が返ってくるので-180°~180°に変換
       // print(eulerrotate.y);

        if (Mathf.Abs(eulerrotate.y) > lookatspeed) { eulerrotate.y = lookatspeed * Mathf.Sign(eulerrotate.y); }  //一度に回転する量を制限


        rotate = Quaternion.Euler(eulerrotate);




        transform.rotation = rotate * transform.rotation;

    }
}
