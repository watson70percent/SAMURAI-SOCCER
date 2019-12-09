using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffenceMF : AIBase
{
    public OffenceMF(List<GameObject> team, List<GameObject> opp, BallControler ball, bool ally) : base(team, opp, ball, ally) { }

    public override void Revaluation()
    {
        InitRevaluat();
        CalcBenefit();
        CalcRisk();
    }

    private void CalcBenefit()
    {
        //前に行くほど、x軸に近いほど高評価
        foreach (var mate in teamMate)
        {
            int x = (int)mate.transform.position.x;
            int y = (int)mate.transform.position.y;

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

    private void CalcRisk()
    {
        //敵の直線上を避ける
        foreach (var mate in goal.y == 100 ? opponent : teamMate)
        {
            Vector2 delta = (mate.ToVector2Int() - ball.gameObject.ToVector2Int());
            Vector2 pos = mate.ToVector2Int();

            while (pos.x > 0 && pos.x < 60 && pos.y > 0 && pos.y < 100)
            {
                for (int i = (int)pos.x - 5; i < (int)pos.x + 5; i++)
                {
                    for (int j = (int)pos.y - 5; j < (int)pos.y + 5; j++)
                    {
                        if (i < 0 || i > 60 || j < 0 || j > 100)
                        {
                            continue;
                        }
                        riskMap[i, j] += 2;
                    }
                }

                pos += delta;
            }
        }

        //近くなりすぎを避ける
        foreach (var mate in goal.y == 100 ? teamMate : opponent)
        {
            foreach (var mate2 in goal.y == 100 ? teamMate : opponent)
            {
                if (mate != mate2)
                {
                    if ((mate.ToVector2Int() - mate2.ToVector2Int()).sqrMagnitude < 400)
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
                                if (i < 0 || i > 60 || j < 0 || j > 100)
                                {
                                    continue;
                                }
                                riskMap[i, j] += (int)(mate.ToVector2Int() - mate2.ToVector2Int()).sqrMagnitude;
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
        int max = (int)ball.transform.position.z + 30;
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
