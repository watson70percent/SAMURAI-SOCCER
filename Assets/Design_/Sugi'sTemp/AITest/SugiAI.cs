using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SugiAI : MonoBehaviour
{

    float GoalAttraction, BallAttraction, FriendAttraction, EnemyAttraction,firstPositionAttraction;
    public float speed = 10;
    int menber;
    int enemyMember;
    public bool nearestBall;
    GameObject goal;
    GameObject ball;
    GameObject[] friends;
    GameObject[] enemys;
    SugiAI[] AIs;
    Vector3 firstPosition;
    static bool kickCoolTime;

    public SugiAIManager AIManager;

    public bool haveBall;

    public float unfreeLevel;
    public float goalDistance;
    Rigidbody ballRig;

    public float unpassedLevel;
    // Start is called before the first frame update
    void Start()
    {


        ball = GameObject.FindGameObjectWithTag("SoccerBall");
        ballRig = ball.GetComponent<Rigidbody>();
        goal = GameObject.Find((this.gameObject.tag == "Ateam") ? "AGoal" : "BGoal");
        var temp = GameObject.FindGameObjectsWithTag(this.gameObject.tag);


        menber = temp.Length - 1;
        friends = new GameObject[menber];

        int count = 0;
        for (int i = 0; i < menber + 1; i++)
        {
            if (temp[i] != gameObject)
            {
                friends[count] = temp[i];
                count++;

            }

        }
        enemys = GameObject.FindGameObjectsWithTag((this.gameObject.tag == "Ateam") ? "Bteam" : "Ateam");
        enemyMember = enemys.Length;


        AIManager = GameObject.Find("GameManager").GetComponent<SugiAIManager>();
        firstPosition = transform.position;
        //print(friends[0].transform.position.ToString());

        AIs = new SugiAI[menber];
        for(int i = 0; i < menber; i++)
        {
            AIs[i] = friends[i].GetComponent<SugiAI>();
        }

    }

    // Update is called once per frame



        

    void Update()
    {


        if (haveBall&&!kickCoolTime) {
            kickCoolTime = true;
            Invoke("EnableKick", 0.2f);
            /*

            var vec = goal.transform.position - transform.position;

            ballRig.AddForce(vec.normalized*2000 );
            */
            Acction();
                }

        if (AIManager.WhoHasBall == SugiAIManager.Who.NoOne) { GoalAttraction = 0; BallAttraction = (nearestBall) ? 100 : 10; FriendAttraction = (nearestBall)?-1:-10; EnemyAttraction = 0; firstPositionAttraction = 2; }
        else
        {
            if (AIManager.WhoHasBall.ToString() == this.gameObject.tag.ToString())
            {
                GoalAttraction = 10; BallAttraction = (haveBall)?100:1; FriendAttraction =(haveBall)?0:-500; EnemyAttraction = -5; firstPositionAttraction = 1;
            }
            else
            {
                GoalAttraction = 0; BallAttraction = 30; FriendAttraction = -15; EnemyAttraction = 0; firstPositionAttraction = 0.1f;
            }
        }


        Vector3 gradient = Vector3.zero;

        gradient += BallAttraction * BallAttract();
        gradient += GoalAttraction * GoalAttract();
        gradient += EnemyAttraction * EnemyAttract();

        gradient += FriendAttraction * friendAttract();
        gradient += firstPositionAttraction * firstPositionAttract();
        gradient.y = 0;

        transform.Translate(gradient.normalized * Time.deltaTime * speed);

        unpassedLevel= unfreeLevel *goalDistance;
    }


    void EnableKick()
    {
        kickCoolTime = false;
    }

    Vector3 friendAttract()
    {
        var friendVecs = new Vector3[menber];
        var friendEffect = new Vector3[menber];

        for (int i = 0; i < menber; i++)
        {

            friendVecs[i] = friends[i].transform.position - transform.position;
        }


        for (int i = 0; i < menber; i++)
        {
            var distance = friendVecs[i].magnitude;
            friendEffect[i] = friendVecs[i] * Potential(distance);
        }

        var direction = Vector3.zero;

        for (int i = 0; i < menber; i++)
        {
            direction += friendEffect[i];
        }
        return direction;
    }

    Vector3 EnemyAttract()
    {
        var enemyVecs = new Vector3[enemyMember];
        var enemyEffect = new Vector3[enemyMember];

        for (int i = 0; i < enemyMember; i++)
        {

            enemyVecs[i] = enemys[i].transform.position - transform.position;
        }

        unfreeLevel = 0;
        for (int i = 0; i < enemyMember; i++)
        {
            var distance = enemyVecs[i].magnitude;
            enemyEffect[i] = enemyVecs[i] * Potential(distance);

            unfreeLevel+= Mathf.Exp(-distance*distance/100)+0.1f;
        }

        var direction = Vector3.zero;

        for (int i = 0; i < enemyMember; i++)
        {
            direction += enemyEffect[i];
        }
        return direction;
    }

    Vector3 GoalAttract()
    {
        Vector3 goalVec = goal.transform.position - transform.position;
        var distance = goalVec.magnitude;
        var direction = goalVec * Potential(distance);


        goalDistance = distance*distance*distance/1000;

        return direction;
    }
    Vector3 BallAttract()
    {
        Vector3 bollVec = ball.transform.position - transform.position;
        var distance = bollVec.magnitude;
        var direction = bollVec * (Potential3(distance)+1);
        return direction;
    }

    Vector3 firstPositionAttract()
    {
        Vector3 Vec = firstPosition- transform.position;
        var distance = Vec.magnitude;
        var direction = Vec * Potential2(distance);
        return direction;
    }

    float Potential(float x)
    {
        return Mathf.Exp(-x*x/100)+3*Mathf.Exp(-x*x);
    }
    float Potential2(float x)
    {
        return Mathf.Abs(x)/10;
    }
    float Potential3(float x)
    {
        return 1 / x ;
    }

    
    void Acction()
    {

        float evaluation = this.unpassedLevel;
        GameObject passPerson = null;
        for(int i = 0; i < menber; i++)
        {
            if (AIs[i].unpassedLevel < evaluation)
            {
                passPerson = friends[i];
                evaluation = AIs[i].unpassedLevel;
            }
        }

        if (passPerson == null)
        {
            Drible();
        }
        else
        {
            Pass(passPerson);
        }
    }

     int driblePower=400, passPower=1700;

    void Drible()
    {
        print("drible");
        var vec1 = (goal.transform.position - transform.position);
        var vec2 = EnemyAttract();

        var dribleVec = (vec1 - vec2).normalized;

        ballRig.AddForce(dribleVec * driblePower);

    }
    void Pass(GameObject person)
    {

        print("pass");
        var vec = person.transform.position - transform.position;

        ballRig.AddForce(vec.normalized*passPower);
    }
}
