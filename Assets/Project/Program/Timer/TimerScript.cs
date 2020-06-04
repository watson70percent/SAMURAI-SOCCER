using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{
    public bool playing;//試合中のフラグ
    bool end = false;//試合終了のフラグ
    public float timer;
    public GameObject finishMessage;//試合終了のメッセージを入れた物
    Text timeText;

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
            if (timer > 1)
            {

                timer -= Time.deltaTime;
                timeText.text = ((int)(timer / 60)).ToString("0") + ":" + ((int)(timer % 60)).ToString("00");

            }
            else
            {
                finishMessage.SetActive(true);//試合終了のメッセージを入れた物を表示
                playing = false;
                end = true;
                //ここに多分処理を追加することになる
                Time.timeScale = 0;

            }
        }
       
    }
}
