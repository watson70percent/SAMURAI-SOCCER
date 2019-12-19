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
    private static List<CPUMove> cpus = new List<CPUMove>();
    private List<bool> evalutable;
    private int cpu_count;
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

    // Start is called before the first frame update
    void Start()
    {
        evalutable = new List<bool>();
        all = new List<AIBase>();

        foreach(var o in team)
        {
            cpus.Add(o.GetComponent<CPUMove>());
            evalutable.Add(true);
        }
        foreach(var o in opp)
        {
            cpus.Add(o.GetComponent<CPUMove>());
            evalutable.Add(true);
        }
        cpu_count = cpus.Count;

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

        ballHolder_o = new BallHolder(team, opp, ball, false);
        all.Add(ballHolder_o);
        offenceFW_o = new OffenceFW(team, opp, ball, false);
        all.Add(offenceFW_o);
        offenceMF_o = new OffenceMF(team, opp, ball, false);
        all.Add(offenceMF_o);
        offenceDF_o = new OffenceDF(team, opp, ball, false);
        all.Add(offenceDF_o);
        defenceFW_o = new DefenceFW(team, opp, ball, false);
        all.Add(defenceFW_o);
        defenceMF_o = new DefenceMF(team, opp, ball, false);
        all.Add(defenceMF_o);
        defenceDF_o = new DefenceDF(team, opp, ball, false);
        all.Add(defenceDF_o);

        StartCoroutine(Revaluate());

        if (debug)
        {
            for(int i = 0; i < all.Count; i++)
            {
                all[i].InitVisualize(plane, new Vector3(70 * (i + 1), 0, 0), new Vector3(70 * (i + 1), 0, 110));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < cpu_count; i++)
        {
            SetState(i);
        }

        if (debug)
        {
            for(int i = 0; i < all.Count; i++)
            {
                all[i].UpdateVisualize();
            }
        }
    }

    private void OnPass(object sender, PassEventArgs e)
    {
        var recever = FindByVector2(e.recever);
        recever.destination = new Vector2Int((int)e.recever.x,(int)e.recever.y);
        int idx1 = cpus.IndexOf(recever);
        evalutable[idx1] = false;
        recever.action = CPUAction.GetBall;

        var send = FindByVector2(e.sender);
        int idx2 = cpus.IndexOf(send);
        evalutable[idx2] = false;
        send.action = CPUAction.Move;


        if(e.height == PassHeight.Low)
        {
            if(idx1 < team.Count)
            {
                for(int i = team.Count; i < cpu_count; i++)
                {
                    Cutball(e, i);
                }
            }
            else
            {
                for(int i = team.Count - 1; i > -1; i--)
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
                    if((e.recever - cpus[i].gameObject.ToVector2Int()).sqrMagnitude < 100)
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

    private CPUMove FindByVector2(Vector2 pos)
    {
        return cpus.Aggregate((result, next) => (result.gameObject.ToVector2Int() - pos).sqrMagnitude > (next.gameObject.ToVector2Int() - pos).sqrMagnitude ? result : next);
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
        temp.SetState(CPUAction.Move, temp.destination);
        ball.owner = e.stealer;
    }

    private void OnGoal(object sender, GoalEventArgs e)
    {
        //TODO: ゴール時の処理
    }

    private void OnOutOfBall(object sender, OutBallEventArgs e)
    {
        //TODO: ボールが外に出たときの処理
    }

    private void SetState(int i)
    {
        if (!evalutable[i])
        {
            return;
        }

        if(cpus[i].gameObject == ball.owner)
        {
            Vector2Int point;
            if (cpus[i].ally)
            {
                point = ballHolder_t.MaxValuePoint(cpus[i].gameObject.ToVector2Int());
            }
            else
            {
                point = ballHolder_o.MaxValuePoint(cpus[i].gameObject.ToVector2Int());
            }

            if(point == Vector2Int.down)
            {
                cpus[i].SetState(CPUAction.Shoot, Vector2Int.zero);
            }
            else if ((cpus[i].gameObject.ToVector2Int() - point).sqrMagnitude > 100)
            {
                cpus[i].SetState(CPUAction.Pass, point);
            }
            else
            {
                cpus[i].SetState(CPUAction.Dribble, point);
            }

            return;
        }

        switch (cpus[i].position)
        {
            case Position.FW:
                if(i < team.Count)
                {
                    if(ball.owner.GetComponent<CPUMove>().ally)
                    {
                        cpus[i].SetState(CPUAction.Move, offenceFW_t.MaxValuePoint(cpus[i].gameObject.ToVector2Int()));
                    }
                    else
                    {
                        cpus[i].SetState(CPUAction.Move, defenceFW_t.MaxValuePoint(cpus[i].gameObject.ToVector2Int()));                                                                                                                
                    }
                }
                else
                {
                    if (ball.owner.GetComponent<CPUMove>().ally)
                    {
                        cpus[i].SetState(CPUAction.Move, offenceFW_o.MaxValuePoint(cpus[i].gameObject.ToVector2Int()));
                    }
                    else
                    {
                        cpus[i].SetState(CPUAction.Move, defenceFW_o.MaxValuePoint(cpus[i].gameObject.ToVector2Int()));
                    }
                }break;

            case Position.MF:
                if (i < team.Count)
                {
                    if (ball.owner.GetComponent<CPUMove>().ally)
                    {
                        cpus[i].SetState(CPUAction.Move, offenceMF_t.MaxValuePoint(cpus[i].gameObject.ToVector2Int()));
                    }
                    else
                    {
                        cpus[i].SetState(CPUAction.Move, defenceMF_t.MaxValuePoint(cpus[i].gameObject.ToVector2Int()));
                    }
                }
                else
                {
                    if (ball.owner.GetComponent<CPUMove>().ally)
                    {
                        cpus[i].SetState(CPUAction.Move, offenceMF_o.MaxValuePoint(cpus[i].gameObject.ToVector2Int()));
                    }
                    else
                    {
                        cpus[i].SetState(CPUAction.Move, defenceMF_o.MaxValuePoint(cpus[i].gameObject.ToVector2Int()));
                    }
                }break;

            case Position.DF:
                if (i < team.Count)
                {
                    if (ball.owner.GetComponent<CPUMove>().ally)
                    {
                        cpus[i].SetState(CPUAction.Move, offenceDF_t.MaxValuePoint(cpus[i].gameObject.ToVector2Int()));
                    }
                    else
                    {
                        cpus[i].SetState(CPUAction.Move, defenceDF_t.MaxValuePoint(cpus[i].gameObject.ToVector2Int()));
                    }
                }
                else
                {
                    if (ball.owner.GetComponent<CPUMove>().ally)
                    {
                        cpus[i].SetState(CPUAction.Move, offenceDF_o.MaxValuePoint(cpus[i].gameObject.ToVector2Int()));
                    }
                    else
                    {
                        cpus[i].SetState(CPUAction.Move, defenceDF_o.MaxValuePoint(cpus[i].gameObject.ToVector2Int()));
                    }
                }
                break;
        }
    }


    private IEnumerator Revaluate()
    {
        while (true)
        {
            foreach(var e in all)
            {
                e.Revaluation();

                yield return null;
            }
        }
    }

    public static float MinimamEnemy(Vector2 sender, Vector2 recever)
    {
        List<float> temp = new List<float>();

        cpus.ForEach(value =>
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
