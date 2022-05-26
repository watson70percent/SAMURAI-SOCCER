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
        bool playing;//�������̃t���O
        bool end = false;//�����I���̃t���O
        float elapsedTime = 0;//�o�ߎ���
        [SerializedField] float limitTime;//��������
        [SerializedField] GameObject displayText;//���Ԃ�\�����������
        public Text timeText;//���Ԃ�\�����������

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