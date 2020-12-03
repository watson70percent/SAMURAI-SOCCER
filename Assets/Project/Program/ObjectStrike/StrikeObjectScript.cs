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


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //時間が経ったらobject生成
        if (elapsedTime>=shotInterval)
        {
            Vector3 pos = transform.position;
            pos.z = shotPosZ[(int)Random.Range(0, 3)];
            transform.position = pos;
            elapsedTime = 0.0f;
            GameObject realObject = Instantiate(ShotObject, this.transform.position, transform.rotation, this.transform);

        }
        //時間増やす
        elapsedTime += Time.deltaTime;
    }

    

}
