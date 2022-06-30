using System.Collections;
using System.Collections.Generic;
using UnityEngine;
usi

public class PandaMaker : MonoBehaviour
{
    [SerializeField] GameObject panda;
    [SerializeField] float minSize, maxSize;
    GameManager gameManager;
    [SerializeField] GameState state = GameState.Reset;
    void SwitchState(StateChangedArg a)
    {
        state = a.gameState;
    }
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Spawn", 1, 0.4f);
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.StateChange += SwitchState;
    }

    // Update is called once per frame
    void Update()
    {
    }


    void Spawn()
    {
        switch (state)
        {
            case GameState.Playing:
                {
                    Vector3 pos = new Vector3(58 * Random.value, 100, 100 * Random.value);
                    GameObject p = Instantiate(panda, pos, Quaternion.Euler(Random.value * 360, Random.value * 360, Random.value * 360));
                    p.transform.localScale = Vector3.one * (Random.value * (maxSize-minSize) + minSize);
                    break;
                }
            default: break;


        }
    }
}
