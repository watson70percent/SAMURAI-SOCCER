using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum Result
{
    Win,
    Lose,
    Draw
}

public class ResultManager : MonoBehaviour
{

    Result resultState;
    [SerializeField]Text result;
    [SerializeField]public string resultText;
    Text samuraiPhrase;
    public SamuraiWordBase samuraiWordBase;



    // Start is called before the first frame update
    void Start()
    {
        //勝敗に応じてテキスト変更
        switch (resultState) 
        {
            case Result.Win:
                result.text = "勝利";
                break;
            case Result.Lose:
                result.text = "敗北";
                break;
            case Result.Draw:
                result.text = "引分";
                break;
        }

        //今日のひとこと
        samuraiPhrase.text = samuraiWordBase.samuraiwords[Random.Range(0,samuraiWordBase.samuraiwords.Count)];

    }

    public void SetResult(Result resultState,string resultText)
    {
        this.resultState = resultState;
        this.resultText = resultText;
    }
}
