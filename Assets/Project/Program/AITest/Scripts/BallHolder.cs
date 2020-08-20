using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallHolder : AIBase
{
    private int benefitGoal;
    private int riskGoal;

    public BallHolder(List<GameObject> team, List<GameObject> opp, BallControler ball, bool ally) : base(team, opp, ball, ally) { }

    public override void Revaluation()
    {
        InitRevaluat();
        benefitGoal = 0;
        riskGoal = 0;
        CalcPositive();
        CalcRiskMap();
        AddDribbleBenefit(ball.owner);
    }

    private void CalcPositive()
    {   //味方いるところ評価アップ
        foreach (var mate in teamMate)
        {
            int x = (int)mate.transform.position.x;
            int y = (int)mate.transform.position.z;

            int x_min = (x - 10) > 0 ? x - 10 : 0;
            int y_min = (y - 10) > 0 ? y - 10 : 0;
            int x_max = (x + 10) < 60 ? x + 10 : 60;
            int y_max = (y + 10) < 100 ? y + 10 : 100;

            int to_goal = (int)Mathf.Abs(goal.x - x) + (int)Mathf.Abs(goal.y - y);

            if (ball.owner == mate)
            {
                int t = 40 - to_goal > 0 ? 40 - to_goal : 0;
                benefitGoal = t * t;
            }
            else
            {
                for (int i = x_min; i < x_max; i++)
                {
                    for (int j = y_min; j < y_max; j++)
                    {
                        benefitMap[i, j] = (int)((100 - to_goal) * CalcDistance(i, j, x, y));
                        if (benefitMap[i, j] < 0)
                        {
                            benefitMap[i, j] = 0;
                        }
                    }
                }
            }

        }
    }

    private void CalcRiskMap()
    {
        riskGoal = 0;

        Vector2Int temp = ball.gameObject.ToVector2Int();

        Vector2Int p1 = new Vector2Int(21, goal.y) - temp;
        Vector2Int p2 = new Vector2Int(39, goal.y) - temp;
        float theta = Vector2.Dot(p1, p2) / p1.magnitude * p2.magnitude;

        //敵の近くは評価ダウン
        foreach (var mate in opponent)
        {
            int x = (int)mate.transform.position.x;
            int y = (int)mate.transform.position.z;

            int x_min = (x - 5) > 0 ? x - 5 : 0;
            int y_min = (y - 5) > 0 ? y - 5 : 0;
            int x_max = (x + 5) < 60 ? x + 5 : 60;
            int y_max = (y + 5) < 100 ? y + 5 : 100;

            int to_goal;
            if (goal.y == 100)
            {
                to_goal = Mathf.Abs(30 - x) + Mathf.Abs(y);
            }
            else
            {
                to_goal = Mathf.Abs(30 - x) + Mathf.Abs(100 - y);
            }

            for (int i = x_min; i < x_max; i++)
            {
                for (int j = y_min; j < y_max; j++)
                {
                    riskMap[i, j] = (int)((100 - to_goal) * CalcDistance(i, j, x, y));
                    if (riskMap[i, j] < 0)
                    {
                        riskMap[i, j] = 0;
                    }
                }
            }
            //ゴールまでの直線上にいると評価ダウン
            if (ball.owner != mate)
            {
                Vector2Int p3 = mate.ToVector2Int() - temp;
                Vector2Int p4 = mate.ToVector2Int() - goal;
                float th = Vector2.Dot(p3, p4) / p3.magnitude * p4.magnitude;
                if (th > theta)
                {
                    riskGoal += 10;
                }
            }

            AvoidTeamMate(0.5f);
        }
    }


    private void AddDribbleBenefit(GameObject mate)
    {
        //自分の近くを評価上げる
        int x = (int)mate.transform.position.x;
        int y = (int)mate.transform.position.z;

        int x_min = (x - 10) > 0 ? x - 10 : 0;
        int y_min = (y - 10) > 0 ? y - 10 : 0;
        int x_max = (x + 10) < 60 ? x + 10 : 60;
        int y_max = (y + 10) < 100 ? y + 10 : 100;

        for (int i = x_min; i < x_max; i++)
        {
            for (int j = y_min; j < y_max; j++)
            {
                if ((x - i) * (x - i) + (y - j) * (y - j) < 100)
                {
                    if (goal.y == 100)
                    {
                        benefitMap[i, j] += 50 - 2 * (y_max - j);
                    }
                    else
                    {
                        benefitMap[i, j] += 50 - 2 * (j - y_min);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 最大期待値の点を返す
    /// </summary>
    /// <param name="strategy">戦略</param>
    /// <returns>最も高い期待値の点。シュートの場合Vector2Int.downを返す</returns>
    public override List<Vector2Int> MaxValuePoint(Vector2Int self,int num, StrategyMode strategy = StrategyMode.Nomal)
    {
        Vector2Int temp = self;
        List<Vector2Int> res = new List<Vector2Int>();
        int max = -1000;

        for (int i = 0; i < 60; i++)
        {
            for (int j = 0; j < 100; j++)
            {

                if (benefitMap[i, j] != 0)
                {
                    switch (strategy)
                    {
                        case StrategyMode.Nomal:
                            if (max < benefitMap[i, j] - riskMap[i, j])
                            {
                                max = benefitMap[i, j] - riskMap[i, j];
                                temp = new Vector2Int(i, j);
                            }
                            break;

                        case StrategyMode.Positive:
                            if (max < benefitMap[i, j] * 2 - riskMap[i, j])
                            {
                                max = benefitMap[i, j] * 2 - riskMap[i, j];
                                temp = new Vector2Int(i, j);
                            }
                            break;

                        case StrategyMode.Passive:
                            if (max < benefitMap[i, j] - riskMap[i, j] * 2)
                            {
                                max = benefitMap[i, j] - riskMap[i, j] * 2;
                                temp = new Vector2Int(i, j);
                            }
                            break;
                    }
                }
            }
        }
        res.Add(temp);
        return res;
    }
}
