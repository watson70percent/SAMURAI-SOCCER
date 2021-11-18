using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikeObjectScript : MonoBehaviour
{
    public float shotInterval;//障害物生成間隔
    float elapsedTime;//生成後経過した時間
    public GameObject ShotObject;//生成するobject
    float[] shotPosZ = {15.5f, 50, 84.3f };//グラウンドの道路の座標
    public GameManager gameManager;
    public GameObject emergencySign;


    // Update is called once per frame
    void Update()
    {
        if(gameManager.CurrentGameState == GameState.Playing)
        {
            emergencySign.transform.position = new Vector3(GameObject.FindGameObjectWithTag("Player").transform.position.x + 1, 6.0f, transform.position.z);

            if (elapsedTime >= shotInterval - 2)
            {
                emergencySign.SetActive(true);
            }
            //時間が経ったらobject生成
            if (elapsedTime >= shotInterval)
            {
                Vector3 pos = transform.position;

                Instantiate(ShotObject, this.transform.position, transform.rotation);
                pos.z = shotPosZ[(int)Random.Range(0, 3)];
                transform.position = pos;
                emergencySign.SetActive(false);
                elapsedTime = 0.0f;
            }
            //時間増やす
            elapsedTime += Time.deltaTime;
        }
        
    }



}
