using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Random = UnityEngine.Random;

public class EasyCPU : MonoBehaviour
{
    public BallControler ball;
    public GameObject dest;
    public EasyCPUManager manager;
    public PersonalStatus status;
    private double velocity;
    private bool isPause = false;

    public void Pause()
    {
        isPause = true;
    }

    public void Play()
    {
        isPause = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPause)
        {
            return;
        }

        if ((dest.transform.position - transform.position).sqrMagnitude < 4)
        {
            try
            {
                if (status.ally)
                {
                    var temp = manager.team.Where(value => value.transform.position.y > transform.position.y);
                    var to = Random.Range(0, temp.Count() + 1);
                    if (to == temp.Count())
                    {
                        ball.Shoot(gameObject, status);
                    }
                    else
                    {
                        ball.Pass(gameObject.ToVector2Int(), temp.Skip(to).First().ToVector2Int());
                    }
                }
                else
                {
                    var temp = manager.opp.Where(value => value.transform.position.y < transform.position.y);
                    var to = Random.Range(0, temp.Count() + 1);
                    if (to == temp.Count())
                    {
                        ball.Shoot(gameObject, status);
                    }
                    else
                    {
                        ball.Pass(gameObject.ToVector2Int(), temp.Skip(to).First().ToVector2Int());
                    }

                }
            }
            catch (Exception) { }
        }

        AllMove(dest.ToVector2Int(), status.fast);

    }

    private void AllMove(Vector2 dest, float max = 5)
    {
        Vector2 vec = dest - gameObject.ToVector2Int();
        vec = vec.normalized * 10;

        try
        {
            if (status.ally)
            {
                if(vec.sqrMagnitude > 400 && ball.gameObject.transform.position.z > gameObject.transform.position.z + 10)
                {
                    vec = Vector2.zero;
                }

                if (vec.sqrMagnitude > 25)
                {

                    manager.team.ForEach(value =>
                    {
                        if (value != gameObject)
                        {
                            Vector2 t = gameObject.ToVector2Int() - value.ToVector2Int();
                            float tmp = status.seelen - t.magnitude > 0 ? status.seelen - t.magnitude : 0;
                            vec += t.normalized * tmp;
                        }
                    });
                }
            }
            else
            {
                if (vec.sqrMagnitude > 400 && ball.gameObject.transform.position.z < gameObject.transform.position.z - 10)
                {
                    vec = Vector2.zero;
                }

                if (vec.sqrMagnitude > 25)
                {
                    manager.opp.ForEach(value =>
                    {
                        if (value != gameObject)
                        {
                            Vector2 t = gameObject.ToVector2Int() - value.ToVector2Int();
                            float tmp = status.seelen - t.magnitude > 0 ? status.seelen - t.magnitude : 0;
                            vec += t.normalized * tmp;
                        }
                    });

                }
            }
           
        }
        catch (Exception)
        {


        }
        float dis = vec.magnitude;

        if (dis > 10)
        {
            if (velocity < max)
            {
                velocity += 0.2f;
            }
        }
        else
        {
            if (velocity < Mathf.Log10(dis + 1) * max)
            {
                velocity += 0.2f;
            }
            else
            {
                velocity = Mathf.Log10(dis + 1) * max;
            }
        }

        vec = CalcNextPoint(vec, velocity);

        Vector3 rot = new Vector3(0, Mathf.Atan2(vec.x, vec.y) * Mathf.Rad2Deg);

        gameObject.transform.Translate(vec.x, 0, vec.y, Space.World);
        gameObject.transform.rotation = Quaternion.Euler(rot);
    }

    private Vector2 CalcNextPoint(Vector2 vec, double velocity)
    {
        var move = vec.normalized * (float)velocity * Time.deltaTime;
        var next = move + new Vector2(transform.position.x, transform.position.z);


        if(next.x < 0)
        {
            move.x = -transform.position.x;
        }
        else if(next.x > Constants.Width)
        {
            move.x = Constants.Width - transform.position.x;
        }

        if(next.y < 0)
        {
            move.y = -transform.position.x;
        }
        else if(next.y > Constants.G2G)
        {
            move.y = Constants.G2G - transform.position.y;
        }

        return move.normalized * (float)velocity * Time.deltaTime;
    }
}
