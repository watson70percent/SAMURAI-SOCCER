using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 自由の女神の状態
/// </summary>
public enum StatueMode {Idle, Rise, FallDown };

/// <summary>
/// 自由の女神が上がってきて倒れる動きの監視+実効
/// </summary>
public class RiseStatue : MonoBehaviour
{
    private GameManager _gameManager;

    /// <summary>
    /// 現在の自由の女神の状態
    /// </summary>
    public StatueMode CurrentStatueMode { get; private set; } = StatueMode.Idle;

    private void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        StartCoroutineForPlayingState.AddTaskIEnumrator(RiseStatueCoroutine());
    }

    private void Update()
    {
        //もしゴールが入ったらこのオブジェクトを消去
        if (_gameManager.CurrentGameState == GameState.Standby)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator RiseStatueCoroutine()
    {
        CurrentStatueMode = StatueMode.Rise;
        while (gameObject.transform.position.y < 0.95f)
        {
            yield return 0.1f;
            try
            {
                gameObject.transform.position += new Vector3(0f, 1.0f, 0f);
            }
            catch
            {
                yield break;
            }
        }
        CurrentStatueMode = StatueMode.FallDown;
        while (gameObject.transform.eulerAngles.z < 90)
        {
            yield return 0.01f;
            try
            {
                gameObject.transform.eulerAngles += new Vector3(0f, 0f, 1.0f);
            }
            catch
            {
                yield break;
            }
        }
        CurrentStatueMode = StatueMode.Idle;
        yield return 3.0f;
        Destroy(gameObject);
    }
}
