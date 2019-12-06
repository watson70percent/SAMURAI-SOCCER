using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallHolderTM : AIBase
{
    public override void Revaluation()
    {
        CalcPositive();
        CalcRiskMap();
        AddDribbleBenefit(ball.owner);
    }

    private void CalcPositive()
    {
        foreach (var mate in teamMate)
        {
            int x = (int)mate.transform.position.x;
            int y = (int)mate.transform.position.y;

            int x_min = (x - 10) > 0 ? x - 10 : 0;
            int y_min = (y - 10) > 0 ? y - 10 : 0;
            int x_max = (x + 10) < 60 ? x + 10 : 60;
            int y_max = (y + 10) < 100 ? y + 10 : 100;

            int to_goal = Mathf.Abs(30 - x) + Mathf.Abs(100 - y);

            for (int i = x_min; i < x_max; i++)
            {
                for (int j = y_min; j < y_max; j++)
                {
                    benefitMap[i, j] = (int)(to_goal * CalcDistance(i, j, x, y));
                }
            }
        }
    }

    private void CalcRiskMap()
    {
        foreach (var mate in opponent)
        {
            int x = (int)mate.transform.position.x;
            int y = (int)mate.transform.position.y;

            int x_min = (x - 10) > 0 ? x - 10 : 0;
            int y_min = (y - 10) > 0 ? y - 10 : 0;
            int x_max = (x + 10) < 60 ? x + 10 : 60;
            int y_max = (y + 10) < 100 ? y + 10 : 100;

            int to_goal = Mathf.Abs(30 - x) + Mathf.Abs(y);

            for (int i = x_min; i < x_max; i++)
            {
                for (int j = y_min; j < y_max; j++)
                {
                    riskMap[i, j] = (int)(to_goal * CalcDistance(i, j, x, y));
                }
            }
        }
    }


    private void AddDribbleBenefit(GameObject mate)
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
                if (CalcDistance(i, j, x, y) > 0)
                {
                    benefitMap[i, j] += 10;
                }
            }
        }
    }

    public override Vector2 MaxValuePoint(StrategyMode strategy = StrategyMode.Nomal)
    {
        Vector2 temp = new Vector2();
        int max = -1000;
        for(int i = 0; i < 60; i++)
        {
            for(int j = 0; j < 100; j++)
            {
                switch (strategy)
                {
                    case StrategyMode.Nomal:
                        if(max < benefitMap[i,j] - riskMap[i, j])
                        {
                            max = benefitMap[i, j] - riskMap[i, j];
                            temp = new Vector2(i, j);
                        }break;

                    case StrategyMode.Positive:
                        if (max < benefitMap[i, j] * 2 - riskMap[i, j])
                        {
                            max = benefitMap[i, j] * 2 - riskMap[i, j];
                            temp = new Vector2(i, j);
                        }
                        break;

                    case StrategyMode.Passive:
                        if (max < benefitMap[i, j] - riskMap[i, j] * 2)
                        {
                            max = benefitMap[i, j] - riskMap[i, j] * 2;
                            temp = new Vector2(i, j);
                        }
                        break;
                }
            }
        }
        return temp;
    }
}
