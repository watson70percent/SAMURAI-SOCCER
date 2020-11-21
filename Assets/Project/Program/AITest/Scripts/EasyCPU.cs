using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using System.Runtime.Serialization;

public class EasyCPU : MonoBehaviour
{
    public BallControler ball;
    public GameObject dest;
    public EasyCPUManager manager;
    public PersonalStatus status;
    public FieldManager field;
    public Rigidbody rb;
    private bool isPause = false;
    private Vector2 before_velocity = Vector2.zero;
    private LinkedList<Vector2> rot_chain = new LinkedList<Vector2>();

    private bool stop = false;


    /// <summary>
    /// 吹っ飛びやすさを再設定
    /// </summary>
    public void SetMass()
    {
        rb.mass = Mathf.Pow(2, status.hp - 1);
    }

    /// <summary>
    /// 攻撃されたときに呼ぶ。hpが減り、吹っ飛びやすくなる。
    /// </summary>
    public void Attacked()
    {
        status.hp--;
        SetMass();
    }

    public void Pause()
    {
        isPause = true;
    }

    public void Play()
    {
        isPause = false;
    }

    public void SlowDown()
    {
        isPause = true;
        StartCoroutine(SlowMove());
    }

    private IEnumerator SlowMove()
    {
        while (true)
        {
            yield return null;
            gameObject.transform.Translate(before_velocity.x * 0.2f * Time.deltaTime, 0, before_velocity.y * 0.2f * Time.deltaTime, Space.World);
        }

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
                    var temp = manager.team.Where(value =>  field.AdaptInversePosition(value.transform.position).z > field.AdaptInversePosition(transform.position).z);
                    var to = Random.Range(0, temp.Count() + 1);
                    if (to == temp.Count())
                    {
                        ball.Shoot(gameObject, status);
                    }
                    else
                    {
                        ball.Pass(gameObject.ToVector2Int(), temp.Skip(to).First().ToVector2Int(), (PassHeight)Random.Range(0, 3),status);
                    }
                }
                else
                {
                    var temp = manager.opp.Where(value => field.AdaptInversePosition(value.transform.position).z < field.AdaptInversePosition(transform.position).z);
                    var to = Random.Range(0, temp.Count() + 1);
                    if (to == temp.Count())
                    {
                        ball.Shoot(gameObject, status);
                    }
                    else
                    {
                        ball.Pass(gameObject.ToVector2Int(), temp.Skip(to).First().ToVector2Int(),(PassHeight)Random.Range(0, 3), status);
                    }

                }
            }
            catch (Exception) { }
        }

        AllMove(dest.ToVector2Int());

    }

    private void AllMove(Vector2 dest)
    {
        Vector2 vec = dest - gameObject.ToVector2Int();
        vec = vec.normalized * 10;

        try
        {
            if (status.ally)
            {
                if(vec.sqrMagnitude > status.seelen * status.seelen * 100 && field.AdaptInversePosition(ball.gameObject.transform.position).z > field.AdaptInversePosition(gameObject.transform.position).z + 10)
                {
                    vec = Vector2.zero;
                }

                if (gameObject != manager.near_team)
                {

                    manager.team.ForEach(value =>
                    {
                        if (value != gameObject)
                        {
                            Vector2 t = gameObject.ToVector2Int() - value.ToVector2Int();
                            float tmp = status.seelen - t.magnitude > 0 ? status.seelen - t.magnitude : 0;
                            vec += t.normalized * tmp * 2;
                        }
                    });
                }
            }
            else
            {
                if (vec.sqrMagnitude > 40000 && field.AdaptInversePosition(ball.gameObject.transform.position).z < field.AdaptInversePosition(gameObject.transform.position).z - 10)
                {
                    vec = Vector2.zero;
                }

                if (gameObject != manager.near_opp)
                {
                    manager.opp.ForEach(value =>
                    {
                        if (value != gameObject)
                        {
                            Vector2 t = gameObject.ToVector2Int() - value.ToVector2Int();
                            float tmp = status.seelen - t.magnitude > 0 ? status.seelen - t.magnitude : 0;
                            vec += t.normalized * tmp * 2;
                        }
                    });

                }
            }
           
        }
        catch (Exception)
        {


        }
        float dis = vec.magnitude;

        if (dis > 9)
        {
            vec = vec.normalized * status.fast;
        }
        else
        {
            vec = vec.normalized * Mathf.Log10(dis + 1) * status.fast;
        }
        rot_chain.AddLast(vec);
        if (rot_chain.Count > 30)
        {
            rot_chain.RemoveFirst();
        }
        vec = CalcNextPoint(vec);
        
        gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, CalcRotAverage()));
        gameObject.transform.Translate(vec.x, 0, vec.y, Space.World);
    }

    private Vector2 CalcNextPoint(Vector2 vec)
    {
        var realVec = CalcRealVec(vec);
        var move = realVec * Time.deltaTime;
        var next = move + new Vector2(transform.position.x, transform.position.z);

        if(next.x < 0 || transform.position.x < 0)
        {
            move.x = (-transform.position.x + 5) * Time.deltaTime;
            rot_chain.RemoveLast();
            rot_chain.AddLast(move);
        }
        else if(next.x > Constants.Width || transform.position.x > Constants.Width)
        {
            move.x = (Constants.Width - transform.position.x - 5) * Time.deltaTime;
            rot_chain.RemoveLast();
            rot_chain.AddLast(move);
        }

        if(next.y < 0 || transform.position.z < 0)
        {
            move.y = (-transform.position.z + 5) * Time.deltaTime;
            rot_chain.RemoveLast();
            rot_chain.AddLast(move);
        }
        else if(next.y > Constants.G2G || transform.position.z > Constants.G2G)
        {
            move.y = (Constants.G2G - transform.position.z - 5) * Time.deltaTime;
            rot_chain.RemoveLast();
            rot_chain.AddLast(move);
        }
        before_velocity = move / Time.deltaTime;

        return move;
    }

    private Vector2 CalcRealVec(Vector2 vec)
    {
        var diff = vec - before_velocity;
        float deg = Vector2.Dot(before_velocity, diff) / before_velocity.magnitude / diff.magnitude;
        float coeff = (deg + 1) / 2 * field.info.GetAccUpCoeff(transform.position) + (1 - deg) / 2 * field.info.GetAccDownCoeff(transform.position);
        if(diff.sqrMagnitude > status.fast * status.fast * coeff * coeff / 180 / 180)
        {
            diff = diff.normalized * status.fast * coeff / 180;
        }

        return before_velocity + diff;
    }

    private float CalcRotAverage()
    {
        Vector2 vec = default;
        foreach(var v in rot_chain)
        {
            vec += v;
        }

        return Mathf.Atan2(vec.x, vec.y) * Mathf.Rad2Deg;
    }
}
