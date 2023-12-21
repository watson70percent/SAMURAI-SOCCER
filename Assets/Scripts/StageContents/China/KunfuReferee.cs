using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SamuraiSoccer.SoccerGame;

namespace SamuraiSoccer.StageContents.China
{
    /// <summary>
    /// カンフーシーンのレフェリー
    /// </summary>
    public class KunfuReferee : RefereeMove
    {
        protected override void Start()
        {
            ball = GameObject.FindGameObjectWithTag("Ball");
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);

                if (child.name == "RefereeArea") { refereeArea = child.GetComponent<RefereeArea>(); }
                if (child.name == "KunfuReferee(Clone)") { anicon = child.GetComponent<Animator>(); }
            }
            base.Start();
            runningspeed = 7;
            radius = 5;
        }

        protected override void LookAtBall()
        {
            transform.Rotate(0, 400 * Time.deltaTime, 0); //ぐるぐる回転
        }

    }
}
