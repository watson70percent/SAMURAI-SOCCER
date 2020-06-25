using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// CPUを操作するクラス
/// </summary>
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

    /// <summary>
    /// 選手を殺す
    /// </summary>
    /// <param name="dead">死ぬ対象の選手</param>
    public void kill(GameObject dead)
    {
        opp.Remove(dead);
        team.Remove(dead);
        Destroy(dead);
    }

    /// <summary>
    /// 選手復活
    /// </summary>
    /// <param name="status">ステータス</param>
    /// <param name="pos">復活場所</param>
    /// <return>復活した選手</return>
    public GameObject Sporn(PersonalStatus status, Vector3 pos)
    {
        var temp = Instantiate(teammate, pos, Quaternion.identity, team_p);
        var setting = temp.GetComponent<EasyCPU>();
        setting.ball = ball;
        setting.dest = ball.gameObject;
        setting.manager = this;
        setting.status = status;
        if (status.ally)
        {
            team.Add(temp);
        }
        else
        {
            opp.Add(temp);
        }

        return temp;
    }

    /// <summary>
    /// 選手復活。中央に出てくる。
    /// </summary>
    /// <param name="status">ステータス</param>
    /// <return>復活した選手</return>
    public GameObject Sporn(PersonalStatus status)
    {
        return Sporn(status, new Vector3(0, 0, 50));
    }

    /// <summary>
    /// 初期化。選手の生成をしてる。
    /// </summary>
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
                var t1 = Sporn(new PersonalStatus() { ally = true }, new Vector3(15 * j + 15, 0, 35 - 15 * i));
                MakeRandomStatus(t1.GetComponent<PersonalStatus>());

                var t2 = Sporn(new PersonalStatus() { ally = false }, new Vector3(15 * j + 15, 0, 65 + 15 * i));
                MakeRandomStatus(t2.GetComponent<PersonalStatus>());
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
