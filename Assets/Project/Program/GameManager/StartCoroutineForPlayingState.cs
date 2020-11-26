using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(GameManager))]
public class StartCoroutineForPlayingState : MonoBehaviour
{
    //実行中のIEnumeratorのリスト
    private static List<IEnumerator> taskEnumerators = new List<IEnumerator>();
    //実行中のIEnumeratorの次までの待ち時間
    private static Dictionary<IEnumerator, float> taskWaitTime = new Dictionary<IEnumerator, float>();

    private GameManager _gameManager;

    public void Start()
    {
        _gameManager = GetComponent<GameManager>();
    }

    public void Update()
    {
        //PlayingState以外では処理しない
        if (_gameManager.CurrentGameState != GameState.Playing)
        {
            return;
        }

        List<int> idx = new List<int>();
        for (int i = 0; i < taskEnumerators.Count; i++)
        {
            //IEnumerator待ち時間がまだあればTime.deltaTimeだけ引いて、待ち時間が0以下になればMoveNextを実行
            if (taskWaitTime[taskEnumerators[i]] > 0)
            {
                taskWaitTime[taskEnumerators[i]] -= Time.deltaTime;
                continue;
            }
            //実行し終わったIEnumeratorは削除リストに追加
            if (!taskEnumerators[i].MoveNext())
            {
                idx.Add(i);
            }

            //待ち時間を取得(例外は握りつぶす→処理しない)
            try
            {
                var waitTime = (float)taskEnumerators[i].Current;
                taskWaitTime[taskEnumerators[i]] = waitTime;
            }
            catch (Exception)
            {

            }

        }
        //実行し終わったIEnumeratorの削除
        for (int i = 0; i < idx.Count; i++)
        {
            taskWaitTime.Remove(taskEnumerators[idx[idx.Count - 1 - i]]);
            taskEnumerators.RemoveAt(idx[idx.Count - 1 - i]);

        }
    }
    /// <summary>
    /// IEnumeratorを追加してコルーチンっぽく処理する
    /// ○○秒待ちたいときはyeild return 秒数(float型)でその秒数待つ
    /// </summary>
    /// <param name="enumerator">
    /// 処理してほしいIEnumerator
    /// </param>
    public static void AddTaskIEnumrator(IEnumerator enumerator)
    {
        taskEnumerators.Add(enumerator);
        taskWaitTime.Add(enumerator, 0);
    }
}
