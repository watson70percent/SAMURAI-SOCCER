using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using System.IO;

/// <summary>
/// CPUを操作するクラス
/// </summary>
public class EasyCPUManager : MonoBehaviour
{
    public List<GameObject> team;
    public Team team_stock;
    public List<GameObject> opp;

    public Team opp_stock;

    public Transform team_p;
    public Transform opp_p;
    public BallControler ball;
    private GameObject teammate;
    private GameObject opponent;

    /// <summary>
    /// 味方の人数
    /// </summary>
    public int TeamMemberCount { 
        get
        {
            return team.Count + team_stock.member.Count;
        } 
    }

    /// <summary>
    /// 敵の人数
    /// </summary>
    public int OpponentMemberCount {
        get
        {
            return opp.Count + opp_stock.member.Count;
        }
    }


    void Awake()
    {
        teammate = Resources.Load<GameObject>("Teammate");
        opponent = Resources.Load<GameObject>("opponent");

        // opponent = Resources.Load<GameObject>(OpponentName.name);
        LoadMember();

    }

    private void Start()
    {
        ball.Goal += (sender, e) => { Init(e.Ally); };
        Init();
    }

    private void LoadMember()
    {
        var team_string = File.ReadAllText(Application.streamingAssetsPath + "/our.json");
        team_stock = JsonUtility.FromJson<Team>(team_string);
        var opp_string = File.ReadAllText(Application.streamingAssetsPath + "/" + OpponentName.name + ".json");
        opp_stock = JsonUtility.FromJson<Team>(opp_string);
    }

    /// <summary>
    /// 選手を殺す。一応瞬時復活もさせる。
    /// </summary>
    /// <param name="dead">死ぬ対象の選手</param>
    public void kill(GameObject dead)
    {
        bool ally = dead.GetComponent<EasyCPU>().status.ally;
        opp.Remove(dead);
        team.Remove(dead);
        Destroy(dead);

        if (ally)
        {
            Sporn(team_stock.member[0], Constants.TeammateSpornPoint);
            team_stock.member.RemoveAt(0);
        }
        else
        {
            Sporn(opp_stock.member[0], Constants.OppornentSpornPoint);
            opp_stock.member.RemoveAt(0);
        }
    }

    /// <summary>
    /// 選手復活
    /// </summary>
    /// <param name="status">ステータス</param>
    /// <param name="pos">復活場所</param>
    /// <return>復活した選手</return>
    public GameObject Sporn(PersonalStatus status, Vector3 pos)
    {
        GameObject temp = default;
        if (status.ally)
        {
            temp = Instantiate(teammate, pos, Quaternion.identity, team_p);
        }
        else
        {
            temp = Instantiate(opponent, pos, Quaternion.identity, opp_p);
        }

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
    /// 初期化。選手の生成をしてる。
    /// </summary>
    public void Init(bool centerIsOppornent = true)
    {
        foreach(var t in team)
        {
            team_stock.member.Insert(0, t.GetComponent<EasyCPU>().status);

            Destroy(t);
        }

        team.Clear();

        foreach(var t in opp)
        {

            opp_stock.member.Insert(0, t.GetComponent<EasyCPU>().status);

            Destroy(t);
        }

        opp.Clear();


        int teamCount = team_stock.member.Count > 11 ? 11 : team_stock.member.Count;
        int oppCount = opp_stock.member.Count > 11 ? 11 : opp_stock.member.Count;

        if (centerIsOppornent)
        {
            for(int i = 0; i < teamCount; i++)
            {
                Sporn(team_stock.member[0], Constants.TeammateInitialSpornPointCenterOppornent[i]);
                team_stock.member.RemoveAt(0);
            }

            for (int i = 0; i < oppCount; i++)
            {
                Sporn(opp_stock.member[0], Constants.OpprnentInitialSpornPointCenterOppornent[i]);
                opp_stock.member.RemoveAt(0);
            }
        }
        else
        {
            for (int i = 0; i < teamCount; i++)
            {
                Sporn(team_stock.member[0], Constants.TeammateInitialSpornPointCenterTeam[i]);
                team_stock.member.RemoveAt(0);
            }

            for (int i = 0; i < oppCount; i++)
            {
                Sporn(opp_stock.member[0], Constants.OpprnentInitialSpornPointCenterTeam[i]);
                opp_stock.member.RemoveAt(0);
            }
        }


        ball.gameObject.transform.position = (Constants.OppornentGoalPoint + Constants.OurGoalPoint) / 2 + new Vector3(0,0.5f,0);
        ball.rb.velocity = Vector3.zero;
    }

}
