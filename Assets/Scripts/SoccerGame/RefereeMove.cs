using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using SamuraiSoccer.Event;

namespace SamuraiSoccer.SoccerGame
{
    public class RefereeMove : MonoBehaviour
    {
        public GameObject ball;
        public float lookatspeed;
        public float runningspeed;
        public float radius;
        Rigidbody rig;
        public RefereeArea refereeArea;


        enum State
        {
            Moving,Stop
        }

        State state;

        //public AnimationController animcon;
        //ParticleSystem bikkuri;
        // Start is called before the first frame update


        public Animator anicon;


        protected virtual void Start()
        {

            InGameEvent.Play.Subscribe(x=> { state = State.Moving; }).AddTo(this);
            InGameEvent.Standby.Subscribe(x=> { state = State.Stop; }).AddTo(this);
            InGameEvent.Pause.Subscribe(isPause=> { state = isPause?State.Stop:State.Moving; }).AddTo(this);

            rig = GetComponent<Rigidbody>();


        }

        // Update is called once per frame
        protected virtual void Update()
        {
            switch (state)
            {
                case State.Stop:
                    anicon.speed = 0;
                    break;
                case State.Moving:
                    LookAtBall();
                    MoveAroundBall();
                    anicon.speed = 1;
                    break;
                default: break;
            }




        }


        void MoveAroundBall()
        {

            Vector3 dir = ball.transform.position - this.transform.position;
            dir.y = 0;
            if (dir.magnitude > radius)
            {

                if (dir.magnitude - radius < 5)
                {
                    rig.transform.position += dir.normalized * runningspeed * Time.deltaTime * 0.5f;

                }
                else
                {
                    rig.transform.position += dir.normalized * runningspeed * Time.deltaTime;

                }




            }

        }



        protected virtual void LookAtBall()
        {

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

}