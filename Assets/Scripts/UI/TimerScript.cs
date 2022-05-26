using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using SamuraiSoccer.Event;

namespace SamuraiSoccer.UI
{
    public class TimerScript : MonoBehaviour
    {
        bool playing;//試合中のフラグ
        bool end = false;//試合終了のフラグ
        float elapsedTime = 0;//経過時間
        [SerializedField] float limitTime;//制限時間
        [SerializedField] GameObject displayText;//時間を表示させるもの
        public Text timeText;//時間を表示させるもの

        private void Start()
        {
            InGameEvent.Reset.Subscribe(_ =>
            {
                elapsedTime = 0;
            });
            InGameEvent.Standby.Subscribe(_ =>
            {
                int displayTime = (int)(limitTime - elapsedTime);
                timeText.text = ((int)(displayTime / 60)).ToString("0") + ":" + Mathf.CeilToInt(displayTime % 60).ToString("00");
            });
            InGameEvent.Pause.Subscribe(_ =>
            {
                playing = false;
            });
            InGameEvent.Play.Subscribe(_ =>
            {
                playing = true;
            });
            InGameEvent.Finish.Subscribe(_ =>
            {
                playing = false;
                end = true;
            });
        }

        // Update is called once per frame
        void Update()
        {
            if (playing && !end)
            {
                if (limitTime > elapsedTime)
                {
                    elapsedTime += Time.deltaTime;
                    //Debug.Log(elapsedTime);
                    int displayTime = (int)(limitTime - elapsedTime);
                    timeText.text = ((int)(displayTime / 60)).ToString("0") + ":" + Mathf.CeilToInt(displayTime % 60).ToString("00");
                }
                else
                {
                    InGameEvent.FinishOnNext();
                }
            }
        }
    }
}