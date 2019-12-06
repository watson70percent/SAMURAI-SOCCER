using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBase
{
    protected int[,] benefitMap = new int[60, 100];
    protected int[,] riskMap = new int[60, 100];
    protected int goal;
    public GameObject[] teamMate = new GameObject[10];
    public GameObject[] opponent = new GameObject[10];
    public BallControler ball;

    public virtual void Revaluation() { }

    public virtual Vector2 MaxValuePoint(StrategyMode strategy = StrategyMode.Nomal) { return Vector2.zero; }

    protected float CalcDistance(int point1_x,int point1_y,int point2_x, int point2_y)
    {
        float temp = 10 - Mathf.Abs(point1_x - point2_x) - Mathf.Abs(point1_y - point2_y);
        if(temp < 0)
        {
            temp = 0;
        }
        temp += 1;

        temp = Mathf.Log10(temp);

        return temp;
    }
}

public enum StrategyMode
{
    Nomal,
    Positive,
    Passive
}
