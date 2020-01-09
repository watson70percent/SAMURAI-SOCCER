﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceDF : AIBase
{
    public DefenceDF(List<GameObject> team, List<GameObject> opp, BallControler ball, bool ally) : base(team, opp, ball, ally) { }

    public override void Revaluation()
    {
        InitRevaluat();
        CalcBenefit();
        CalcRisk();
    }

    private void CalcBenefit()
    {
        if ((ball.transform.position.z > 50 && goal.y == 100) || (ball.transform.position.z < 50 && goal.y == 0))
        {
            //前に行くほど、x軸に近いほど高評価
            for (int i = 0; i < 60; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    benefitMap[i, j] = PointValue(i, j);
                    if (benefitMap[i, j] < 0)
                    {
                        benefitMap[i, j] = 0;
                    }
                }
            }
        }
        else
        {
            //敵のボールを奪い取る位置が高評価
            foreach (var mate in goal.y == 100 ? opponent : teamMate)
            {
                Vector2 delta = (mate.ToVector2Int() - ball.gameObject.ToVector2Int()).nomalize();
                Vector2 pos = ball.gameObject.ToVector2Int();
                int magnitude = (int)(mate.ToVector2Int() - ball.gameObject.ToVector2Int()).magnitude;

                for (int k = 0; k < (magnitude - 5) / 3; k++)
                {
                    for (int i = (int)pos.x - 5; i < (int)pos.x + 5; i++)
                    {
                        for (int j = (int)pos.y - 5; j < (int)pos.y + 5; j++)
                        {
                            if (i >= 0 && i < 60 && j >= 0 && j < 100)
                            {
                                benefitMap[i, j] += 5;
                            }
                        }
                    }

                    pos += delta * 3;
                }

                for (int k = 0; k < 5; k++)
                {
                    for (int i = (int)pos.x - 5; i < (int)pos.x + 5; i++)
                    {
                        for (int j = (int)pos.y - 5; j < (int)pos.y + 5; j++)
                        {
                            if (i >= 0 && i < 60 && j >= 0 && j < 100)
                            {
                                benefitMap[i, j] += 5;
                            }
                        }
                    }
                    pos += delta;
                }

            }

            //ボール持っている人の評価高くなりすぎ防止と、ボール持っている人の前を高評価
            int x_min = ball.gameObject.transform.position.x < 10 ? 0 : (int)ball.gameObject.transform.position.x - 10;
            int x_max = ball.gameObject.transform.position.x > 50 ? 60 : (int)ball.gameObject.transform.position.x + 10;
            int y_min = ball.gameObject.transform.position.z < 10 ? 0 : (int)ball.gameObject.transform.position.z - 10;
            int y_max = ball.gameObject.transform.position.z > 90 ? 100 : (int)ball.gameObject.transform.position.z + 10;

            for (int i = x_min; i < x_max; i++)
            {
                for (int j = y_min; j < y_max; j++)
                {
                    if (benefitMap[i, j] > 30)
                    {
                        benefitMap[i, j] = 30;
                    }

                    if ((new Vector2Int(i, j) - goal).sqrMagnitude < (ball.gameObject.ToVector2Int() - goal).sqrMagnitude)
                    {
                        benefitMap[i, j] += 10;
                    }

                }
            }
        }
    }

    private void CalcRisk()
    {
        //前に行きすぎ回避
        if (goal.y == 100)
        {
            for (int i = 0; i < 60; i++)
            {
                for (int j = 99; j > ball.transform.position.z - 30; j--)
                {
                    if (j >= 0)
                    {
                        riskMap[i, j] += j - (int)ball.transform.position.z + 30;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < 60; i++)
            {
                for (int j = 0; j < ball.transform.position.z + 30; j++)
                {
                    if (j < 100)
                    {
                        riskMap[i, j] += (int)ball.transform.position.z - 30 - j;
                    }
                }
            }
        }

        AvoidTeamMate();
    }

    private int PointValue(int x, int y)
    {
        int temp;
        int max = 20;
        //ボールより一定前まで前に行くとを高評価（場合による）
        if (goal.y == 100)
        {
            temp = y;
        }
        else
        {
            temp = 100 - y;
        }

        if (temp > max)
        {
            temp = max;
        }

        temp += (30 - (int)Mathf.Abs(ball.transform.position.x - x) / 2);

        return temp;
    }

    public override List<Vector2Int> MaxValuePoint(Vector2Int self,int num ,StrategyMode strategy = StrategyMode.Nomal)
    {
        return base.MaxValuePoint(self,num ,strategy);
    }
}
