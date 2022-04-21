using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageDataSave : MonoBehaviour
{

    private void Start()
    {
        StartCoroutine(Save());
    }


    IEnumerator Save()
    {
        Result result;
        while ((result=GetComponent<ResultManager>().ResultState) == Result.Undefined)
        {
            yield return null;
        }

        if (result == Result.Win)
        {
            while (GetComponent<ResultManager>().NowStageData ==null )
            {
                yield return null;
            }

            StageDataManager.SaveStageData(GetComponent<ResultManager>().NowStageData);

           
        }

    }
}
