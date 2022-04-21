using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

public class CallStatueOfLiberty : MonoBehaviour
{
    private GameManager _gameManager;//Scene内のGameManagerクラス
    private IEnumerator _enumerator; //コルーチンに追加するIEnumratorを保持しておく

    public GameObject StatuePrefab;//自由の女神プレハブ 

    private void Start()
    {
        //GameNamagerなかったらエラーなのでこの先は動かさない。
        if (!GameObject.FindGameObjectWithTag("GameManager"))
        {
            Debug.LogError("GameManagerが存在しません");
        }
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _enumerator = CallStatueCoroutine();
        StartCoroutineForPlayingState.AddTaskIEnumrator(_enumerator);
    }

    private void OnDestroy()
    {
        StartCoroutineForPlayingState.RemoveTaskIEnumrator(_enumerator);
    }

    /// <summary>
    /// 自由の女神を呼び出す
    /// </summary>
    /// <returns></returns>
    IEnumerator CallStatueCoroutine()
    {
        yield return 3.0f;
        while (_gameManager.CurrentGameState != GameState.Finish)
        {
            Instantiate(StatuePrefab, new Vector3(60f, -70.0f, 50+Random.Range(-40f, 40f)), Quaternion.identity);
            yield return 6.0f;
        }

    }

}
