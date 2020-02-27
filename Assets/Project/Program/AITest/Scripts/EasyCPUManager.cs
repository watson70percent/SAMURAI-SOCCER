using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EasyCPUManager : MonoBehaviour
{
    public List<GameObject> team;
    public List<GameObject> opp;
    public Transform team_p;
    public Transform opp_p;
    public BallControler ball;
    private GameObject teammate;
    private GameObject opponent;

    void Awake()
    {
        teammate = Resources.Load<GameObject>("Teammate");
        opponent = Resources.Load<GameObject>("opponent");
    }

    private void Start()
    {
        ball.Goal += (sender, e) => { Init(); };
        Init();
    }

    public void kill(GameObject dead)
    {
        opp.Remove(dead);
        team.Remove(dead);
        Destroy(dead);
    }


    public void RespornTeammate()
    {
        var temp = Instantiate(teammate, new Vector3(0, 0, 50), Quaternion.identity, team_p);
        var setting = temp.GetComponent<EasyCPU>();
        setting.ball = ball;
        setting.dest = ball.gameObject;
        setting.manager = this;
        setting.status = temp.GetComponent<PersonalStatus>();
        team.Add(temp);
    }

    public void RespornOpponent()
    {
        var temp = Instantiate(opponent, new Vector3(60, 0, 50), Quaternion.identity, team_p);
        var setting = temp.GetComponent<EasyCPU>();
        setting.ball = ball;
        setting.dest = ball.gameObject;
        setting.manager = this;
        setting.status = temp.GetComponent<PersonalStatus>();
        opp.Add(temp);
    }

    public void Init()
    {
        foreach(var t in team)
        {
            Destroy(t);
        }

        team.Clear();

        foreach(var t in opp)
        {
            Destroy(t);
        }

        opp.Clear();

        for(int i = 0; i < 3; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                var temp = Instantiate(teammate, new Vector3(15 * j + 15, 0, 35 - 15 * i), Quaternion.identity, team_p);
                var setting = temp.GetComponent<EasyCPU>();
                setting.ball = ball;
                setting.dest = ball.gameObject;
                setting.manager = this;
                setting.status = temp.GetComponent<PersonalStatus>();
                MakeRandomStatus(setting.status);
                team.Add(temp);
                var temp2 = Instantiate(opponent, new Vector3(15 * j + 15, 0, 65 + 15 * i), Quaternion.identity, opp_p);
                var setting2 = temp2.GetComponent<EasyCPU>();
                setting2.ball = ball;
                setting2.dest = ball.gameObject;
                setting2.manager = this;
                setting2.status = temp2.GetComponent<PersonalStatus>();
                MakeRandomStatus(setting2.status);
                opp.Add(temp2);
            }
        }
        kill(team[1]);
        ball.gameObject.transform.position = new Vector3(30, 0.5f, 50);
        ball.rb.velocity = Vector3.zero;
    }

    private void MakeRandomStatus(PersonalStatus status)
    {
        status.fast += Random.Range(-2.0f, 2.0f);
        status.power += Random.Range(-2, 2);
        status.see += Random.Range(-0.2f, 0.2f);
        status.seelen += Random.Range(-5.0f, 3.0f);
    }
}
