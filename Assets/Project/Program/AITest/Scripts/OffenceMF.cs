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

    private void CalcRisk()
    {
        //敵の直線上を避ける
        foreach (var mate in goal.y == 100 ? opponent : teamMate)
        {
            Vector2 delta = (mate.ToVector2Int() - ball.gameObject.ToVector2Int());
            Vector2 pos = mate.ToVector2Int();

            while (pos.x > 0 && pos.x < 60 && pos.y > 0 && pos.y < 100)
            {
                for (int i = (int)pos.x - 3; i < (int)pos.x + 3; i++)
                {
                    for (int j = (int)pos.y - 3; j < (int)pos.y + 3; j++)
                    {
                        if (i >= 0 && i < 60 && j >= 0 && j < 100)
                        {
                            riskMap[i, j] += 6;
                        }
                    }
                }

                pos += delta;
            }
        }

        AvoidTeamMate();
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


    public override List<Vector2Int> MaxValuePoint(Vector2Int self,int num ,StrategyMode strategy = StrategyMode.Nomal)
    {
        return base.MaxValuePoint(self, num, strategy);
    }
}
