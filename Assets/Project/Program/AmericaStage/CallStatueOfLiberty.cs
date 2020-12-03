using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

public class CallStatueOfLiberty : MonoBehaviour
{
    private GameManager _gameManager;//Scene内のGameManagerクラス

    public GameObject StatuePrefab;//自由の女神プレハブ 

    private void Start()
    {
        //GameNamagerなかったらエラーなのでこの先は動かさない。
        if (!GameObject.FindGameObjectWithTag("GameManager"))
        {
            Debug.LogError("GameManagerが存在しません");
        }
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        StartCoroutineForPlayingState.AddTaskIEnumrator(CallStatueCoroutine());
    }

    /// <summary>
    /// 自由の女神を呼び出す
    /// </summary>
    /// <returns></returns>
    IEnumerator CallStatueCoroutine()
    {
        while (_gameManager.CurrentGameState != GameState.Finish)
        {
            yield return 10.0f;
            Instantiate(StatuePrefab, new Vector3(60f, -70.0f, 50+Random.Range(-40f, 40f)), Quaternion.identity);
        }

    }

}
