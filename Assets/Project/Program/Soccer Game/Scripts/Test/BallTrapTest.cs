using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallTrapTest : MonoBehaviour
{
    public BallControler ball;
    void OnCollisionEnter(Collision collision)
    {
        ball.Trap(gameObject);
    }
}
