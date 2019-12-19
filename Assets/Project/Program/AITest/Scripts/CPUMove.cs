using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

[RequireComponent(typeof(PersonalStatus))]
public class CPUMove : MonoBehaviour
{
    public CPUAction action;
    public Vector2Int destination;
    public Position position;
    public bool ally;
    public BallControler ball;
    private bool can_change = true;
    private IEnumerator dribble;
    private int counter = 0;
    private PersonalStatus self;
    private float velocity = 0;

    void Start()
    {
        self = GetComponent<PersonalStatus>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (action)
        {
            case CPUAction.Dribble: Dribble(); break;
            case CPUAction.Move:    Move(); break;
            case CPUAction.GetBall: GetBall(); break;
            case CPUAction.CutBall: CutBall(); break;
            case CPUAction.Pass:    Pass(); break;
            case CPUAction.Shoot:   Shoot(); break;
            case CPUAction.Steal:   Steal(); break;
        }
    }

    private void Dribble()
    {
        AllMove();
        if(dribble == null || counter == 40)
        {
            dribble = ball.Dribble(self);
            counter = 0;
        }
        else
        {
            dribble.MoveNext();
            counter++;
        }
    }

    private void Move()
    {
        var dis = AllMove();
        if(dis < 1)
        {
            SetState(CPUAction.Steal, ball.gameObject.ToVector2Int());
        }
        dribble = null;
    }

    private float AllMove(float max = 5, Vector2 dest = default)
    {
        if(dest == default)
        {
            dest = destination;
        }
        Vector2 vec = dest - gameObject.ToVector2Int();
        float dis = vec.magnitude;
        if(dis > 10)
        {
            if(velocity < max)
            {
                velocity += 0.05f;
            }
        }
        else
        {
            if(velocity < Mathf.Log10(dis + 1) * max)
            {
                velocity += 0.05f;
            }
            else
            {
                velocity = Mathf.Log10(dis + 1) * max;
            }
        }


        Vector3 pos = gameObject.transform.position;
        gameObject.transform.rotation = Quaternion.Euler(0, Mathf.Atan2(vec.y, vec.x), 0);
        vec = vec.normalized * velocity / 60;
        pos += new Vector3(vec.x, 0, vec.y);
        gameObject.transform.position = pos;


        return dis;
    }

    private void GetBall()
    {
        AllMove(3);
        dribble = null;
    }
    
    private void CutBall()
    {
        Vector2 dest = destination + (ball.gameObject.ToVector2Int() - destination).nomalize();
        AllMove(dest: dest);
        dribble = null;
    }

    private void Pass()
    {
        float min = AIManager.MinimamEnemy(gameObject.ToVector2Int(), destination);
        if(min < 5)
        {
            ball.Pass(ball.gameObject.ToVector2Int(), destination, PassHeight.Low, gameObject.GetComponent<PersonalStatus>());
        }
        else if((gameObject.ToVector2Int() - destination).sqrMagnitude < 900)
        {
            ball.Pass(ball.gameObject.ToVector2Int(), destination, PassHeight.High, gameObject.GetComponent<PersonalStatus>());
        }
        else
        {
            ball.Pass(ball.gameObject.ToVector2Int(), destination,self: gameObject.GetComponent<PersonalStatus>());
        }
        dribble = null;
    }

    private void Shoot()
    {
        ball.Shoot(gameObject);
        dribble = null;
    }

    private void Steal()
    {
        ball.Steal(gameObject);
        dribble = null;
    }

    public void SetState(CPUAction act, Vector2Int dest)
    {
        if (action != act)
        {
            if (can_change)
            {
                action = act;
                destination = dest;

                can_change = false;
                StartCoroutine(CoolTime());
            }
        }
    }

    private IEnumerator CoolTime()
    {
        yield return new WaitForSeconds(1);
        can_change = true;
    }
}

public enum Position
{
    FW,
    MF,
    DF
}

public enum CPUAction
{
    Dribble,
    Move,
    GetBall,
    CutBall,
    Pass,
    Shoot,
    Steal,
}
