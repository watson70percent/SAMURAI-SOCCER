using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceMF : AIBase
{
    public DefenceMF(List<GameObject> team, List<GameObject> opp, BallControler ball, bool ally) : base(team, opp, ball, ally) { }

    public override void Revaluation()
    {
        InitRevaluat();
        CalcBenefit();
        CalcRisk();
    }

    private void CalcBenefit()
    {
        if ((ball.transform.position.z < 20 && goal.y == 100) || (ball.transform.position.z > 80 && goal.y == 0))
        {
            //前に行くほど、x軸に近いほど高評価
            foreach (var mate in teamMate)
            {
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
                        benefitMap[i, j] = (int)(PointValue(i, j) * CalcDistance(i, j, x, y));
                        if (benefitMap[i, j] < 0)
                        {
                            benefitMap[i, j] = 0;
                        }
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
                                riskMap[i, j] += 5;
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
                                riskMap[i, j] += 5;
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
                    if (goal.y == 100)
                    {
                        if (benefitMap[i, j] > 30)
                        {
                            benefitMap[i, j] = 30;
                        }

                        if (j < ball.gameObject.transform.position.z)
                        {
                            benefitMap[i, j] += 10;
                        }
                    }
                    else
                    {
                        if (benefitMap[i, j] > 30)
                        {
                            benefitMap[i, j] = 30;
                        }

                        if (j > ball.gameObject.transform.position.z)
                        {
                            benefitMap[i, j] += 10;
                        }
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
            for (int i = 0; i < 30; i++)
            {
                for (int j = 99; j > ball.transform.position.z; j--)
                {
                    if (j >= 0)
                    {
                        riskMap[i, j] += j - (int)ball.transform.position.z - 30;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < 30; i++)
            {
                for (int j = 0; j < ball.transform.position.z; j++)
                {
                    if (j < 100)
                    {
                        riskMap[i, j] += (int)ball.transform.position.z + 30 - j;
                    }
                }
            }
        }


        //近くなりすぎを避ける
        foreach (var mate in goal.y == 100 ? teamMate : opponent)
        {
            foreach (var mate2 in goal.y == 100 ? teamMate : opponent)
            {
                if (mate != mate2)
                {
                    if ((mate.ToVector2Int() - mate2.ToVector2Int()).sqrMagnitude < 100)
                    {
                        int x_min, x_max, y_min, y_max;
                        if (mate.transform.position.x > mate2.transform.position.x)
                        {
                            x_max = (int)mate2.transform.position.x;
                            x_min = (int)mate.transform.position.x;
                        }
                        else
                        {
                            x_min = (int)mate2.transform.position.x;
                            x_max = (int)mate.transform.position.x;
                        }

                        if (mate.transform.position.z > mate2.transform.position.z)
                        {
                            y_max = (int)mate2.transform.position.z;
                            y_min = (int)mate.transform.position.z;
                        }
                        else
                        {
                            y_min = (int)mate2.transform.position.z;
                            y_max = (int)mate.transform.position.z;
                        }

                        for (int i = x_min; i < x_max; i++)
                        {
                            for (int j = y_min; j < y_max; j++)
                            {
                                if (i >= 0 && i < 60 && j >= 0 && j < 100)
                                {
                                    riskMap[i, j] += (int)((mate.ToVector2Int() - mate2.ToVector2Int()).sqrMagnitude * 0.1f);
                                }
                                
                            }
                        }
                    }
                }
            }
        }
    }

    private int PointValue(int x, int y)
    {
        int temp;
        int max = 35;
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

    public override Vector2Int MaxValuePoint(Vector2Int self, StrategyMode strategy = StrategyMode.Nomal)
    {
        return base.MaxValuePoint(self, strategy);
    }
}
