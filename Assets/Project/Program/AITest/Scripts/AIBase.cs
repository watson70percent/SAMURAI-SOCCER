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
    protected bool[,] evaluateble = new bool[60, 100]; 
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

    public virtual List<Vector2Int> MaxValuePoint(Vector2Int self,int num ,StrategyMode strategy = StrategyMode.Nomal)
    {
        for(int i = 0; i < 60; i++)
        {
            for(int j = 0; j < 100; j++)
            {
                evaluateble[i, j] = true;
            }
        }

        Vector2Int temp = self;
        List<Vector2Int> res = new List<Vector2Int>();
        int max = -1000;
        int harf_length = Mathf.FloorToInt(60 / num * 0.5f);

        for(int k = 0; k < num; k++) {
            for (int i = 0; i < 60; i++)
            {
                for (int j = 0; j < 100; j++)
                {

                    if (benefitMap[i, j] != 0 && evaluateble[i, j])
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
            FindPointProcess(temp.x, temp.y, harf_length);
            max = -1000;
        }

        return res;
    }

    protected void FindPointProcess(int x, int y, int length)
    {
        int x_min = x - length > 0 ? x - length : 0;
        int x_max = x + length < 60 ? x + length : 59;
        int y_min = y - length > 0 ? y - length : 0;
        int y_max = y + length < 100 ? y + length : 99;

        for(int i = x_min; i < x_max; i++)
        {
            for(int j = y_min ; j < y_max; j++)
            {
                evaluateble[i, j] = false;
            }
        }
        
    }

    /// <summary>
    /// 二点のマンハッタン距離から近いほど高い値を返す
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
        if(temp == 10)
        {
            temp = 0;
        }
        temp += 1;

        temp = Mathf.Log10(temp);

        return temp;
    }


    /// <summary>
    /// 近くなりすぎを避ける
    /// </summary>
    protected void AvoidTeamMate(float magnification = 1)
    {
        foreach (var mate in teamMate)
        {
            foreach (var mate2 in teamMate)
            {
                if (mate != mate2)
                {
                    if ((mate.ToVector2Int() - mate2.ToVector2Int()).sqrMagnitude < 100)
                    {
                        int x_min, x_max, y_min, y_max;
                        if (mate.transform.position.x > mate2.transform.position.x)
                        {
                            x_min = (int)mate2.transform.position.x - 3;
                            x_max = (int)mate.transform.position.x + 3;
                        }
                        else
                        {
                            x_max = (int)mate2.transform.position.x + 3;
                            x_min = (int)mate.transform.position.x - 3;
                        }

                        if (mate.transform.position.z > mate2.transform.position.z)
                        {
                            y_min = (int)mate2.transform.position.z - 3;
                            y_max = (int)mate.transform.position.z + 3;
                        }
                        else
                        {
                            y_max = (int)mate2.transform.position.z + 3;
                            y_min = (int)mate.transform.position.z - 3;
                        }

                        if (x_max - x_min < 8)
                        {
                            x_max += 3;
                            x_min -= 3;
                        }

                        if (y_max - y_min < 8)
                        {
                            y_max += 3;
                            y_min -= 3;
                        }

                        for (int i = x_min; i < x_max; i++)
                        {
                            for (int j = y_min; j < y_max; j++)
                            {
                                if (i >= 0 && i < 60 && j >= 0 && j < 100)
                                {
                                    riskMap[i, j] += (int)((100 - 10 * (mate.ToVector2Int() - mate2.ToVector2Int()).magnitude) * magnification);
                                }
                            }
                        }
                    }
                }
            }
        }
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
