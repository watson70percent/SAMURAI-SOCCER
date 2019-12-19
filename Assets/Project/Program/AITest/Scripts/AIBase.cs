using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Object = UnityEngine.Object;

public class AIBase
{
    protected int[,] benefitMap = new int[60, 100];
    protected int[,] riskMap = new int[60, 100];
    protected List<GameObject> teamMate = new List<GameObject>();
    protected List<GameObject> opponent = new List<GameObject>();
    protected Vector2Int goal;
    protected BallControler ball;
    protected GameObject plane;
    protected MeshRenderer[,] objs_b = new MeshRenderer[15, 25];
    protected MeshRenderer[,] objs_r = new MeshRenderer[15, 25];

    public AIBase(List<GameObject> team, List<GameObject> opp, BallControler _ball, bool _ally)
    {
        teamMate = team;
        opponent = opp;
        ball = _ball;
        if (_ally)
        {
            goal = new Vector2Int(30, 100);
        }
        else
        {
            goal = new Vector2Int(30, 0);
        }
    }

    /// <summary>
    /// 期待値を計算
    /// </summary>
    public virtual void Revaluation() { }

    public virtual Vector2Int MaxValuePoint(Vector2Int self, StrategyMode strategy = StrategyMode.Nomal)
    {
        Vector2Int temp = self;
        int max = -1000;
        int x_min = self.x < 10 ? 0 : self.x - 10;
        int x_max = self.x > 50 ? 60 : self.x + 10;
        int y_min = self.y < 10 ? 0 : self.y - 10;
        int y_max = self.y > 90 ? 100 : self.y + 10;

        for (int i = x_min; i < x_max; i++)
        {
            for (int j = y_min; j < y_max; j++)
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

        return temp;
    }

    /// <summary>
    /// 二点のマンハッタン距離を返す
    /// </summary>
    /// <param name="point1_x"></param>
    /// <param name="point1_y"></param>
    /// <param name="point2_x"></param>
    /// <param name="point2_y"></param>
    /// <returns></returns>
    protected float CalcDistance(int point1_x, int point1_y, int point2_x, int point2_y)
    {
        float temp = 10 - Mathf.Abs(point1_x - point2_x) - Mathf.Abs(point1_y - point2_y);
        if (temp < 0)
        {
            temp = 0;
        }
        temp += 1;

        temp = Mathf.Log10(temp);

        return temp;
    }

    protected void InitRevaluat()
    {
        for (int i = 0; i < 60; i++)
        {
            for (int j = 0; j < 100; j++)
            {
                benefitMap[i, j] = 0;
                riskMap[i, j] = 0;
            }
        }
    }

    public void InitVisualize(Transform[] parent, GameObject obj, Vector3 origin_b, Vector3 origin_r)
    {
        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 25; j++)
            {
                objs_b[i, j] = Object.Instantiate(obj, origin_b + new Vector3(i * 4, 0, j * 4), Quaternion.identity, parent[0]).GetComponent<MeshRenderer>();
                objs_r[i, j] = Object.Instantiate(obj, origin_r + new Vector3(i * 4, 0, j * 4), Quaternion.identity, parent[1]).GetComponent<MeshRenderer>();
            }
        }
    }

    public void UpdateVisualize()
    {
        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 25; j++)
            {
                int sum1 = 0;
                int sum2 = 0;
                for (int k = 0; k < 4; k++)
                {
                    for (int l = 0; l < 4; l++)
                    {
                        sum1 += benefitMap[4 * i + k, 4 * j + l];
                        sum2 += riskMap[4* i + k, 4 * j + l];
                    }
                }
               
                    objs_b[i, j].material.color = Color.HSVToRGB((240 - sum1 * 0.0625f) / 360, 1, 1);
                    objs_r[i, j].material.color = Color.HSVToRGB((240 - sum2 * 0.0625f) / 360, 1, 1);              
            }
        }
    }
}

public static class GameObjectExtention
{
    public static Vector2Int ToVector2Int(this GameObject obj)
    {
        return new Vector2Int((int)obj.transform.position.x, (int)obj.transform.position.z);
    }
}

public static class Vector2IntExtention
{
    public static Vector2 nomalize(this Vector2Int vec)
    {
        Vector2 temp = vec;
        return temp.normalized;
    }
}

public enum StrategyMode
{
    Nomal,
    Positive,
    Passive
}
