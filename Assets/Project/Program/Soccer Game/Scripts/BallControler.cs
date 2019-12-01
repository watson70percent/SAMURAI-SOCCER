using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class BallControler : MonoBehaviour
{
    private Rigidbody rb;
    private readonly float sqrt3 = Mathf.Sqrt(3);
    private readonly float sqrt2 = Mathf.Sqrt(2);
    public bool last_touch;
    public delegate void GoalEventHandler(object sender, GoalEventArgs e);
    public delegate void OutBallEventHandler(object sender, OutBallEventArgs e);

    public event GoalEventHandler Goal;
    public event OutBallEventHandler OutBall;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// トラップするとき呼ぶ関数
    /// </summary>
    /// <param name="self">能力値</param>
    public void Trap(PersonalStatus self = default)
    {
        if (SuccessTrap(self))
        {
            rb.velocity = Vector3.zero;
        }
    }

    /// <summary>
    /// トラップが成功するかの判定
    /// </summary>
    /// <param name="self">能力値</param>
    /// <returns>成功のときtrue</returns>
    private bool SuccessTrap(PersonalStatus self)
    {
        //ToDo: トラップ判定
        return true;
    }

    /// <summary>
    /// パスをするとき呼ぶ関数
    /// </summary>
    /// <param name="sender">パス出す人</param>
    /// <param name="recever">パス受け取る人</param>
    /// <param name="height">パスの高さ</param>
    /// <param name="self">自分の能力</param>
    public void Pass(GameObject sender, GameObject recever, PassHeight height = PassHeight.Middle, PersonalStatus self = default)
    {
        if(self == default)
        {
            self = new PersonalStatus(_power: 30);
        }
        switch (height)
        {
            case PassHeight.Low:    CalcLowPass(sender, recever, self); break;
            case PassHeight.Middle: CalcMiddlePass(sender, recever, self); break;
            case PassHeight.High:   CalcHighPass(sender, recever, self); break;
        }
    }

    private void CalcLowPass(GameObject sender, GameObject recever, PersonalStatus self)
    {
        Vector3 dest = (recever.transform.position - sender.transform.position).normalized;

        rb.AddForceAtPosition(self.Power * dest, 0.3f * new Vector3(-dest.x / 2, 0.4f, -dest.z / 2), ForceMode.Impulse);
    }

    private void CalcMiddlePass(GameObject sender, GameObject recever, PersonalStatus self)
    {
        Vector3 dest = (recever.transform.position - sender.transform.position);
        float distance = dest.magnitude;
        float power = Mathf.Sqrt(3 * 9.8f * distance) / 2;
        if(power > self.Power)
        {
            power = self.Power;
        }
        dest = dest.normalized;
        rb.AddForceAtPosition(power * new Vector3(2 / sqrt3 * dest.x, 1.0f / sqrt3, 2 / sqrt3 * dest.z), 0.3f * new Vector3(-2 / sqrt3 * dest.x, -1.0f / sqrt3, -2 / sqrt3 * dest.z), ForceMode.Impulse);
        Debug.Log(power * new Vector3(2 / sqrt3 * dest.x, 1.0f / sqrt3, 2 / sqrt3 * dest.z));
    }

    private void CalcHighPass(GameObject sender, GameObject recever, PersonalStatus self)
    {
        Vector3 dest = recever.transform.position - sender.transform.position;
        float distance = dest.magnitude;
        float power = Mathf.Sqrt(9.8f * distance);
        if (power > self.Power)
        {
            power = self.Power;
        }
        dest = dest.normalized;
        rb.AddForceAtPosition(power * new Vector3(dest.x / sqrt2, 1.0f / sqrt2, dest.z / sqrt2), 0.3f * new Vector3(-dest.x / sqrt2, -1.0f / sqrt2, -dest.z / sqrt2), ForceMode.Impulse);

    }

    public void Shoot(GameObject sender, PersonalStatus self = default)
    {
        if(self == default)
        {
            self = new PersonalStatus(_power: 30);
        }

        Vector3 dest;
        if (self.ally)
        {
            dest = (new Vector3(Random.Range(-10, 10), Random.Range(0.0f, 2.0f), 50) - sender.transform.position).normalized;
        }
        else
        {
            dest = (new Vector3(Random.Range(-10, 10), Random.Range(0.0f, 2.0f), -50) - sender.transform.position).normalized;
        }

        rb.AddForceAtPosition(self.Power * dest, 0.3f * new Vector3(-dest.x, -dest.y, dest.z),ForceMode.Impulse);
        Debug.Log(self.Power * dest);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Goal")
        {
            var e = new GoalEventArgs();
            e.Ally = last_touch;
            OnGoal(e);
        }else if(collision.gameObject.tag == "OutWall")
        {
            var e = new OutBallEventArgs();
            e.Ally = last_touch;
            e.Point = new Vector2(transform.position.x, transform.position.z);
            OnOutBall(e);
        }

    }

    private void OnGoal(GoalEventArgs e)
    {
        Goal?.Invoke(this, e);
    }

    private void OnOutBall(OutBallEventArgs e)
    {
        OutBall?.Invoke(this, e);
    }

}

public enum PassHeight
{
    Low,
    Middle,
    High
}

public class GoalEventArgs : EventArgs {
    public bool Ally;
}

public class OutBallEventArgs : EventArgs {
    public bool Ally;
    public Vector2 Point;
}

