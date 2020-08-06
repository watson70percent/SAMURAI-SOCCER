using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Penalty : MonoBehaviour
{

    public GameObject[] yellowCard=new GameObject[2];
    int penaltycount = 0;
    // Start is called before the first frame update

    private void Reset(StateChangedArg a)
    {
        if (a.gameState == GameState.Reset)
        {
            penaltycount = 0;
            yellowCard[0].SetActive(false);
            yellowCard[1].SetActive(false);
        }

    }
    void Start()
    {
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.StateChange += Reset;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void YellowCard()
    {
        yellowCard[penaltycount].SetActive(true);
        penaltycount++;

    }
}
