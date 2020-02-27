using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Random = UnityEngine.Random;

[RequireComponent(typeof(PersonalStatus))]
public class EasyCPU : MonoBehaviour
{
    public BallControler ball;
    public GameObject dest;
    public EasyCPUManager manager;
    public PersonalStatus status;
    private double velocity;

    // Update is called once per frame
    void Update()
    {
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
        Vector2 vec = (dest - gameObject.ToVector2Int()) * (status.seelen + Random.Range(-5.0f, 5.0f)) / Mathf.Pow((dest - gameObject.ToVector2Int()).sqrMagnitude, status.see);
        if(vec.sqrMagnitude < 100)
        {
            vec = vec.normalized * 10;
        }
        try
        {
            if (status.ally)
            {
                manager.team.ForEach(value =>
                {
                    if (value != gameObject)
                    {
                        Vector2 t = gameObject.ToVector2Int() - value.ToVector2Int();
                        vec += t * (status.seelen + Random.Range(-8.0f, 8.0f)) / Mathf.Pow(t.sqrMagnitude, status.see);
                    }
                });
            }
            else
            {
                manager.opp.ForEach(value =>
                {
                    if (value != gameObject)
                    {
                        Vector2 t = gameObject.ToVector2Int() - value.ToVector2Int();
                        vec += t * (status.seelen + Random.Range(-8.0f, 8.0f)) / Mathf.Pow(t.sqrMagnitude, status.see);
                    }
                });
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
                velocity += 0.05f;
            }
        }
        else
        {
            if (velocity < Mathf.Log10(dis + 1) * max)
            {
                velocity += 0.05f;
            }
            else
            {
                velocity = Mathf.Log10(dis + 1) * max;
            }
        }

        Vector3 rot = new Vector3(0, Mathf.Atan2(vec.x, vec.y) * Mathf.Rad2Deg);
        vec = vec.normalized;
        gameObject.transform.Translate(vec.x * Time.deltaTime, 0, vec.y * Time.deltaTime, Space.World);
        gameObject.transform.rotation = Quaternion.Euler(rot);
    }
}
