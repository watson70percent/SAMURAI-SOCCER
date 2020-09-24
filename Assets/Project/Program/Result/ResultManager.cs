using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum ResultState
{
    Win,
    TimeOver,
    Violation
}

public class ResultManager : MonoBehaviour
{

    public ResultState resultState;
    public Text resultText;
    public Text samuraiPhrase;
    public SamuraiWordBase samuraiWordBase;



    // Start is called before the first frame update
    void Start()
    {
        //勝敗に応じてテキスト変更
        switch (resultState) 
        {
            case ResultState.Win:
                resultText.text = "勝利";
                break;
            case ResultState.TimeOver:
                resultText.text = "敗北";
                break;
            case ResultState.Violation:
                resultText.text = "敗北";
                break;
        }
        //今日のひとこと
        samuraiPhrase.text = samuraiWordBase.samuraiwords[Random.Range(0,samuraiWordBase.samuraiwords.Count)];

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
