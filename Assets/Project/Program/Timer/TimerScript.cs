using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{
    public bool playing;//試合中のフラグ
    bool end = false;//試合終了のフラグ
    public float timer;//残り試合時間
    Text timeText;//時間の表示  Textに表示する前提だからTextにアタッチしてないとバグると思われ

    public bool isTimeUp()//タイムアップか否かを返す  true:タイムアップ false:まだ
    {
        return end;
    }

    public void setTimer(int min,int sec)//時間をセットする  (int 分　int 秒)
    {
        timer = min * 60 + sec;
        end = false;
    }

    public void pause()//停止する
    {
        playing = false;
    }

    public void pauseEnd()//停止解除する
    {
        playing = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        timeText = this.GetComponent<Text>();///バグの要因
    }

    // Update is called once per frame
    void Update()
    {
        if (playing && !end)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                ///バグの要因
                timeText.text = ((int)(timer / 60)).ToString("0") + ":" + Mathf.CeilToInt(timer % 60).ToString("00");
                ///
            }
            else
            {
                playing = false;
                end = true;
            }
        }
       
    }
}
