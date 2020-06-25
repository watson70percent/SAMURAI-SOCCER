using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SugiAIManager : MonoBehaviour
{

    public GameObject[] Aplayers;
    public GameObject[] Bplayers;
    public GameObject Ball;
    public SugiAI[] AIsA;
    public SugiAI[] AIsB;
    const float GET_BALL_DISTANCE = 1f;
    const float INDIPENDENT_DISTANCE = 5;
    public int Amembers;
    public int Bmembers;


    public enum Who
    {
        Ateam,
        Bteam,
        NoOne
    }
    public Who WhoHasBall;

    // Start is called before the first frame update
    void Start()
    {

        Ball = GameObject.FindGameObjectWithTag("SoccerBall");
        Aplayers = GameObject.FindGameObjectsWithTag("Ateam");
        Bplayers = GameObject.FindGameObjectsWithTag("Bteam");

        Amembers = Aplayers.Length;
        Bmembers = Bplayers.Length;
        AIsA = new SugiAI[Amembers];
        AIsB = new SugiAI[Bmembers];
        for (int i = 0; i < Amembers; i++)
        {
            AIsA[i] = Aplayers[i].GetComponent<SugiAI>();
        }
        for (int i = 0; i < Bmembers; i++)
        {
            AIsB[i] = Bplayers[i].GetComponent<SugiAI>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        WhoseBall();
    }

    void WhoseBall()
    {
        GameObject nearestBallManA = null;
        SugiAI nearestBallAIA = null;
        GameObject nearestBallManB = null;
        SugiAI nearestBallAIB = null;
        float nearestBallDistanceA = 10000;
        float nearestBallDistanceB = 10000;

        for (int i = 0; i < Amembers; i++)
        {
            AIsA[i].haveBall = false;
            AIsA[i].nearestBall = false;
            var distance = (Aplayers[i].transform.position - Ball.transform.position).magnitude;
            if (distance < nearestBallDistanceA)
            {
                nearestBallDistanceA = distance;
                nearestBallManA = Aplayers[i];
                nearestBallAIA = AIsA[i];

            }

        }
        for (int i = 0; i < Bmembers; i++)
        {
            AIsB[i].haveBall = false;
            AIsB[i].nearestBall = false;
            var distance = (Bplayers[i].transform.position - Ball.transform.position).magnitude;
            if (distance < nearestBallDistanceB)
            {
                nearestBallDistanceB = distance;
                nearestBallManB = Bplayers[i];
                nearestBallAIB = AIsB[i];
            }

        }


        nearestBallAIA.nearestBall = true;
        nearestBallAIB.nearestBall = true;

        GameObject nearestBallMan;
        float nearestBallDistance;
        SugiAI nearestBallAI;

        if (nearestBallDistanceA < nearestBallDistanceB)
        {
            nearestBallDistance = nearestBallDistanceA;
            nearestBallMan = nearestBallManA;
            nearestBallAI = nearestBallAIA;
        }
        else
        {
            nearestBallDistance = nearestBallDistanceB;
            nearestBallMan = nearestBallManB;
            nearestBallAI = nearestBallAIB;
        }


        if (nearestBallDistance < GET_BALL_DISTANCE)
        {
            nearestBallAI.haveBall = true;
            switch (nearestBallMan.tag)
            {
                case "Ateam": WhoHasBall = Who.Ateam; break;
                case "Bteam": WhoHasBall = Who.Bteam; break;
                default: print("ThereIsBug"); break;
            }
        }
        else
        {
            if (nearestBallDistance > INDIPENDENT_DISTANCE)
            {

                WhoHasBall = Who.NoOne;
            }
        }
    }
}
