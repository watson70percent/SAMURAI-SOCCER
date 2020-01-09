using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIManager : MonoBehaviour
{
    public BallControler ball;
    public List<GameObject> team;
    public List<GameObject> opp;
    public bool debug;
    public GameObject plane;
    public List<Transform> parents;

    private static List<CPUMove> cpus = new List<CPUMove>();
    private List<Vector2Int>[,,] dest = new List<Vector2Int>[2, 3, 2];
    private static List<CPUMove> team_cpu = new List<CPUMove>();
    private static List<CPUMove> opp_cpu = new List<CPUMove>();
    private List<GameObject> t_FW = new List<GameObject>();
    private List<CPUMove> t_FW_cpu = new List<CPUMove>();
    private List<GameObject> t_MF = new List<GameObject>();
    private List<CPUMove> t_MF_cpu = new List<CPUMove>();
    private List<GameObject> t_DF = new List<GameObject>();
    private List<CPUMove> t_DF_cpu = new List<CPUMove>();
    private List<GameObject> o_FW = new List<GameObject>();
    private List<CPUMove> o_FW_cpu = new List<CPUMove>();
    private List<GameObject> o_MF = new List<GameObject>();
    private List<CPUMove> o_MF_cpu = new List<CPUMove>();
    private List<GameObject> o_DF = new List<GameObject>();
    private List<CPUMove> o_DF_cpu = new List<CPUMove>();
    private static int team_count;
    private List<bool> evalutable;
    private static int cpu_count;

    private BallHolder ballHolder_t;
    private OffenceFW offenceFW_t;
    private OffenceMF offenceMF_t;
    private OffenceDF offenceDF_t;
    private DefenceFW defenceFW_t;
    private DefenceMF defenceMF_t;
    private DefenceDF defenceDF_t;
    private BallHolder ballHolder_o;
    private OffenceFW offenceFW_o;
    private OffenceMF offenceMF_o;
    private OffenceDF offenceDF_o;
    private DefenceFW defenceFW_o;
    private DefenceMF defenceMF_o;
    private DefenceDF defenceDF_o;
    private List<AIBase> all;
    private bool start = false;

    // Start is called before the first frame update
    void Start()
    {
        evalutable = new List<bool>();
        all = new List<AIBase>();

        foreach (var o in team)
        {
            cpus.Add(o.GetComponent<CPUMove>());
            evalutable.Add(true);
        }
        team_cpu = team.Select(value => value.GetComponent<CPUMove>()).ToList();
        team_count = cpus.Count;
        foreach (var o in opp)
        {
            cpus.Add(o.GetComponent<CPUMove>());
            evalutable.Add(true);
        }
        opp_cpu = opp.Select(value => value.GetComponent<CPUMove>()).ToList();
        cpu_count = cpus.Count;

        t_FW = cpus.Where(value => { if (value.ally && value.position == Position.FW) { return true; } else return false; }).Select(value => value.gameObject).ToList();
        t_MF = cpus.Where(value => { if (value.ally && value.position == Position.MF) { return true; } else return false; }).Select(value => value.gameObject).ToList();
        t_DF = cpus.Where(value => { if (value.ally && value.position == Position.DF) { return true; } else return false; }).Select(value => value.gameObject).ToList();
        o_FW = cpus.Where(value => { if (!value.ally && value.position == Position.FW) { return true; } else return false; }).Select(value => value.gameObject).ToList();
        o_MF = cpus.Where(value => { if (!value.ally && value.position == Position.MF) { return true; } else return false; }).Select(value => value.gameObject).ToList();
        o_DF = cpus.Where(value => { if (!value.ally && value.position == Position.DF) { return true; } else return false; }).Select(value => value.gameObject).ToList();

        t_FW_cpu = t_FW.Select(value => value.GetComponent<CPUMove>()).ToList();
        t_MF_cpu = t_MF.Select(value => value.GetComponent<CPUMove>()).ToList();
        t_DF_cpu = t_DF.Select(value => value.GetComponent<CPUMove>()).ToList();
        o_FW_cpu = o_FW.Select(value => value.GetComponent<CPUMove>()).ToList();
        o_MF_cpu = o_MF.Select(value => value.GetComponent<CPUMove>()).ToList();
        o_DF_cpu = o_DF.Select(value => value.GetComponent<CPUMove>()).ToList();

        for (int i = 0; i < 2; i++)
        {
            for(int j = 0;j < 3; j++)
            {
                for(int k = 0; k < 2; k++)
                {
                    dest[i, j, k] = new List<Vector2Int>();
                }
            }
        }


        ball.PassSend += OnPass;
        ball.Trapping += OnTrap;
        ball.DribbleKick += OnDribble;
        ball.StealBall += OnSteal;

        ballHolder_t = new BallHolder(team, opp, ball, true);
        all.Add(ballHolder_t);
        offenceFW_t = new OffenceFW(team, opp, ball, true);
        all.Add(offenceFW_t);
        offenceMF_t = new OffenceMF(team, opp, ball, true);
        all.Add(offenceMF_t);
        offenceDF_t = new OffenceDF(team, opp, ball, true);
        all.Add(offenceDF_t);
        defenceFW_t = new DefenceFW(team, opp, ball, true);
        all.Add(defenceFW_t);
        defenceMF_t = new DefenceMF(team, opp, ball, true);
        all.Add(defenceMF_t);
        defenceDF_t = new DefenceDF(team, opp, ball, true);
        all.Add(defenceDF_t);

        ballHolder_o = new BallHolder(opp, team, ball, false);
        all.Add(ballHolder_o);
        offenceFW_o = new OffenceFW(opp, team, ball, false);
        all.Add(offenceFW_o);
        offenceMF_o = new OffenceMF(opp, team, ball, false);
        all.Add(offenceMF_o);
        offenceDF_o = new OffenceDF(opp, team, ball, false);
        all.Add(offenceDF_o);
        defenceFW_o = new DefenceFW(opp, team, ball, false);
        all.Add(defenceFW_o);
        defenceMF_o = new DefenceMF(opp, team, ball, false);
        all.Add(defenceMF_o);
        defenceDF_o = new DefenceDF(opp, team, ball, false);
        all.Add(defenceDF_o);

        foreach (var e in all)
        {
            e.Revaluation();
        }

        StartCoroutine(Revaluate());

        if (debug)
        {
            for (int i = 0; i < all.Count; i++)
            {
                all[i].InitVisualize(parents.Skip(2 * i).Take(2).ToArray(), plane, new Vector3(70 * (i + 1), 0, 0), new Vector3(70 * (i + 1), 0, 110));
            }
        }

        start = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            SetDest();
            CalcState();

            if (debug)
            {
                for (int i = 0; i < all.Count; i++)
                {
                    all[i].UpdateVisualize();
                }
            }
        }
    }

    private void SetDest()
    {
        dest[0, 0, 0] = offenceFW_t.MaxValuePoint(Vector2Int.zero, t_FW.Count);
        dest[0, 0, 1] = defenceFW_t.MaxValuePoint(Vector2Int.zero, t_FW.Count);
        dest[0, 1, 0] = offenceMF_t.MaxValuePoint(Vector2Int.zero, t_MF.Count);
        dest[0, 1, 1] = defenceMF_t.MaxValuePoint(Vector2Int.zero, t_MF.Count);
        dest[0, 2, 0] = offenceDF_t.MaxValuePoint(Vector2Int.zero, t_DF.Count);
        dest[0, 2, 1] = defenceDF_t.MaxValuePoint(Vector2Int.zero, t_DF.Count);
        dest[1, 0, 0] = offenceFW_o.MaxValuePoint(Vector2Int.zero, o_FW.Count);
        dest[1, 0, 1] = defenceFW_o.MaxValuePoint(Vector2Int.zero, o_FW.Count);
        dest[1, 1, 0] = offenceMF_o.MaxValuePoint(Vector2Int.zero, o_MF.Count);
        dest[1, 1, 1] = defenceMF_o.MaxValuePoint(Vector2Int.zero, o_MF.Count);
        dest[1, 2, 0] = offenceDF_o.MaxValuePoint(Vector2Int.zero, o_DF.Count);
        dest[1, 2, 1] = defenceDF_o.MaxValuePoint(Vector2Int.zero, o_DF.Count);
    }

    private void OnPass(object sender, PassEventArgs e)
    {
        var ally = ball.owner.GetComponent<CPUMove>().ally;
        var recever = FindByVector2(ally ? team_cpu : opp_cpu, e.recever);
        recever.destination = new Vector2Int((int)e.recever.x, (int)e.recever.y);
        int idx1 = cpus.IndexOf(recever);
        evalutable[idx1] = false;
        recever.action = CPUAction.GetBall;
        recever.destination = new Vector2Int((int)e.recever.x, (int)e.recever.y);

        var send = FindByVector2(ally ? team_cpu : opp_cpu, e.sender);
        int idx2 = cpus.IndexOf(send);
        evalutable[idx2] = false;
        Debug.Log("recever :" + recever + ", point :" + e.recever);

        if (e.height == PassHeight.Low)
        {
            if (idx1 < team.Count)
            {
                for (int i = team.Count; i < cpu_count; i++)
                {
                    Cutball(e, i);
                }
            }
            else
            {
                for (int i = team.Count - 1; i > -1; i--)
                {
                    Cutball(e, i);
                }
            }
        }
        else
        {
            if (idx1 < team.Count)
            {
                for (int i = team.Count; i < cpu_count; i++)
                {
                    if ((e.recever - cpus[i].gameObject.ToVector2Int()).sqrMagnitude < 100)
                    {
                        cpus[i].action = CPUAction.CutBall;
                        cpus[i].destination = new Vector2Int((int)e.recever.x, (int)e.recever.y);
                        evalutable[i] = false;

                    }
                }
            }
            else
            {
                for (int i = team.Count - 1; i > -1; i--)
                {
                    if ((e.recever - cpus[i].gameObject.ToVector2Int()).sqrMagnitude < 100)
                    {
                        cpus[i].action = CPUAction.CutBall;
                        cpus[i].destination = new Vector2Int((int)e.recever.x, (int)e.recever.y);
                        evalutable[i] = false;
                    }
                }
            }
        }
    }

    private void Cutball(PassEventArgs e, int i)
    {
        Vector2 vec1 = e.recever - e.sender;
        Vector2 vec2 = cpus[i].gameObject.ToVector2Int() - e.sender;
        var theta = Vector2.Dot(vec1, vec2) / vec1.magnitude * vec2.magnitude;

        if (theta > 0.95)
        {
            cpus[i].action = CPUAction.CutBall;
            Vector2 dest = e.sender + vec1.normalized * vec2.magnitude * theta;
            cpus[i].destination = new Vector2Int((int)dest.x, (int)dest.y);
            evalutable[i] = false;
        }

    }

    private static CPUMove FindByVector2(List<CPUMove> objs ,Vector2 pos)
    {
        return objs.Aggregate((result, next) => (result.gameObject.ToVector2Int() - pos).sqrMagnitude < (next.gameObject.ToVector2Int() - pos).sqrMagnitude ? result : next);
    }

    private void OnTrap(object sender, TrapEventArgs e)
    {
        ball.owner = e.owner;
        evalutable.ForEach(value => value = true);
    }

    private void OnDribble(object sender, DribbleEventArgs e)
    {
        ball.owner = e.owner;
    }

    private void OnSteal(object sender, StealEventArgs e)
    {
        var temp = ball.owner.GetComponent<CPUMove>();
        temp.action = CPUAction.Move;
        temp.can_change = false;
        StartCoroutine(temp.CoolTime());
        ball.owner = e.stealer;
        ball.owner.GetComponent<CPUMove>().action = CPUAction.Dribble;
    }

    private void OnGoal(object sender, GoalEventArgs e)
    {
        //TODO: ゴール時の処理
    }

    private void OnOutOfBall(object sender, OutBallEventArgs e)
    {
        //TODO: ボールが外に出たときの処理
    }

    private void CalcState()
    {
        bool ally = ball.owner.GetComponent<CPUMove>().ally;
        var t_FW = ally ? dest[0, 0, 0] : dest[0, 0, 1];
        var t_MF = ally ? dest[0, 1, 0] : dest[0, 0, 1];
        var t_DF = ally ? dest[0, 2, 0] : dest[0, 2, 1];
        var o_FW = ally ? dest[1, 0, 1] : dest[1, 0, 0];
        var o_MF = ally ? dest[1, 1, 1] : dest[1, 1, 0];
        var o_DF = ally ? dest[1, 2, 1] : dest[1, 2, 0];
        CalcEachState(t_FW, t_FW_cpu);
        CalcEachState(t_MF, t_MF_cpu);
        CalcEachState(t_DF, t_DF_cpu);
        CalcEachState(o_FW, o_FW_cpu);
        CalcEachState(o_MF, o_MF_cpu);
        CalcEachState(o_DF, o_DF_cpu);

    }

    private void CalcEachState(List<Vector2Int> dest, List<CPUMove> c)
    {
        CPUMove[] temp = new CPUMove[c.Count];
        c.CopyTo(temp);
        var left = temp.ToList();
        foreach (var d in dest)
        {
            var obj = FindByVector2(left, d);
            if (obj.gameObject == ball.owner)
            {
                SetOwnerState(obj);
            }
            else
            {
                SetState(CPUAction.Move, d, cpus.IndexOf(obj));
            }
            left.Remove(obj);
        }
    }

    private void SetOwnerState(CPUMove owner)
    {

        Vector2Int point;
        if (owner.ally)
        {
            point = ballHolder_t.MaxValuePoint(owner.gameObject.ToVector2Int(), 1).First();
        }
        else
        {
            point = ballHolder_o.MaxValuePoint(owner.gameObject.ToVector2Int(), 1).First();
        }

        if (point == Vector2Int.down)
        {
            owner.SetState(CPUAction.Shoot, Vector2Int.zero);
        }
        else if (FindByVector2(team_cpu, point) == owner || FindByVector2(opp_cpu, point) == owner)
        {
            owner.SetState(CPUAction.Dribble, point);
            
        }
        else
        {
            owner.SetState(CPUAction.Pass, point);
        }
    }

    private void SetState(CPUAction action, Vector2Int dest, int idx)
    {
        if (!evalutable[idx])
        {
            return;
        }
        else
        {
            cpus[idx].SetState(action, dest);
        }
    }


    private IEnumerator Revaluate()
    {
        while (true)
        {
            foreach (var e in all)
            {
                e.Revaluation();

                yield return null;
            }
        }
    }

    public static float MinimamEnemy(Vector2 sender, Vector2 recever)
    {
        List<float> temp = new List<float>();
        int skip;
        int take;
        var obj = GameObject.Find("ball").GetComponent<BallControler>().owner.GetComponent<CPUMove>().ally ? team_cpu : opp_cpu;

        if (cpus.IndexOf(FindByVector2(obj ,sender)) < team_count)
        {
            skip = 0;
            take = team_count;
        }
        else
        {
            skip = team_count;
            take = cpu_count - team_count;
        }

        cpus.Skip(skip).Take(take).ToList().ForEach(value =>
        {
            Vector2 val = value.gameObject.ToVector2Int();
            var x1 = recever.x - sender.x;
            var y1 = recever.y - sender.y;
            var a2 = x1 * x1;
            var b2 = y1 * y1;
            var r2 = a2 + b2;
            var tt = -(x1 * (x1 - val.x) + y1 * (y1 - val.y));
            if (tt < 0)
            {
                temp.Add((x1 - val.x) * (x1 - val.x) + (y1 - val.y) * (y1 - val.y));
                return;
            }
            if (tt > r2)
            {
                temp.Add((recever.x - val.x) * (recever.x - val.x) + (recever.y - val.y) * (recever.y - val.y));
                return;
            }
            var f1 = x1 * (y1 - val.y) - y1 * (x1 - val.x);
            temp.Add((f1 * f1) / r2);
            return;
        });

        return temp.Min();
    }
}
