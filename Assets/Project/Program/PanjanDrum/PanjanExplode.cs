using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PanjanExplode : MonoBehaviour
{
    GameManager gameManager;
    bool isEnd;
    PanjanMake panjanMake;
    //GameObject PrefabFire;
    // Start is called before the first frame update
    void Start()
    {
        //PrefabFire = Resources.Load<GameObject>("Assets/Design_/Sugi'sTemp/Ninja/Sources/Fire");
        //gameManager取得
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        panjanMake=GameObject.Find("PanjanMaker").GetComponent<PanjanMake>();
        //transform=new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100));
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
