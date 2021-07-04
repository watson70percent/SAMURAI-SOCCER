using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanjanMake : MonoBehaviour
{
    GameManager gameManager;
    public GameObject panjan;
    bool panjanExist;
    Vector3 respone = new Vector3(30, 2.0f, 110);

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(gameManager.CurrentGameState == GameState.Playing && !panjanExist)
        {
            Instantiate(panjan,respone,Quaternion.identity);
            panjanExist = true;
        }
        if(gameManager.CurrentGameState == GameState.Standby)
        {
            panjanExist = false;
        }
    }
}
