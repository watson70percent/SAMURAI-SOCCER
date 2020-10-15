using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameManager))]
public class StartCoroutineForPlayingState : MonoBehaviour
{
    public static List<IEnumerator> taskEnumerators = new List<IEnumerator>();
    private GameManager _gameManager;

    public void Start()
    {
        _gameManager = GetComponent<GameManager>();
    }

    public void Update()
    {
        if (_gameManager.CurrentGameState != GameState.Playing)
        {
            return;
        }

        List<int> idx = new List<int>();
        for (int i = 0;i < taskEnumerators.Count;i++)
        {
            if (!taskEnumerators[i].MoveNext())
            {
                idx.Add(i);
            }
        }
        for (int i = 0; i < idx.Count; i++)
        {
            taskEnumerators.RemoveAt(idx[idx.Count - 1 - i]);
        }
    }
}
