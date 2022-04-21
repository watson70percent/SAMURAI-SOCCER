using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikeObjectScript : MonoBehaviour
{
    public float shotInterval = 4.0f; //障害物生成間隔
    public float emergencyInterval = 2.0f; //警告時間間隔
    float elapsedTime;//生成後経過した時間
    public GameObject ShotObject;//生成するobject
    float[] shotPosZ = {15.5f, 50, 84.3f };//グラウンドの道路の座標
    public GameManager gameManager;
    public GameObject emergencySign;

    bool isEnd, _isActive = true;
    private BallControler _ball;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        _ball = GameObject.FindGameObjectWithTag("Ball").GetComponent<BallControler>();
        _ball.Goal += InActiveFunction;

    }

    // Update is called once per frame
    void Update()
    {
        if(gameManager.CurrentGameState == GameState.Playing && _isActive==true)
        {
            emergencySign.transform.position = new Vector3(GameObject.FindGameObjectWithTag("Player").transform.position.x + 1, 6.0f, transform.position.z);
            //時間が経ったらobject生成まで警告
            if (elapsedTime >= shotInterval - emergencyInterval)
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
        if(gameManager.CurrentGameState == GameState.Standby && _isActive ==false)
        {
            _isActive = true;
        }
    }

    public void InActiveFunction(object sender, GoalEventArgs goalEventArgs)
    {
        _isActive = false;
        _ball.Goal -= InActiveFunction;
    }





}
