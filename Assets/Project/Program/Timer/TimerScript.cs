using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{
    public bool playing;//試合中のフラグ
    bool end = false;//試合終了のフラグ
    public float timer;//残り試合時間
    Text timeText;//時間の表示

    public bool isTimeUp()
    {
        return end;
    }

    public void setTimer(int min,int sec)
    {
        timer = min * 60 + sec;
        end = false;
    }

    public void pause()
    {
        playing = false;
    }

    public void pauseEnd()
    {
        playing = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        timeText = this.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playing && !end)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                timeText.text = ((int)(timer / 60)).ToString("0") + ":" + Mathf.CeilToInt(timer % 60).ToString("00");
            }
            else
            {
                playing = false;
                end = true;
            }
        }
       
    }
}
