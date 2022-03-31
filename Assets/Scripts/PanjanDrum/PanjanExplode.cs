using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PanjanExplode : MonoBehaviour
{
    GameManager gameManager;
    PanjanMake panjanMake;
    // Start is called before the first frame update
    void Start()
    {
        //gameManager取得
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        panjanMake=GameObject.Find("PanjanMaker").GetComponent<PanjanMake>();
    }


    private void OnCollisionEnter(Collision other)
    {
        if (gameManager.CurrentGameState == GameState.Playing)
        {
            //    //衝突がプレイヤーだったらゲームオーバー
            if (other.gameObject.tag == "Player")
            {
                other.transform.Find("Fire").gameObject.SetActive(true);
                other.transform.GetComponent<Rigidbody>();
                panjanMake.Burn();
            }
        }
    }

   

}
