using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
        if((dest.transform.position - transform.position).sqrMagnitude < 4)
        {
            if (status.ally)
            {
                var temp = manager.team.Where(value => value.transform.position.y > transform.position.y);
                var to = Random.Range(0, temp.Count() + 1);
                if(to == temp.Count())
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

        AllMove(dest.ToVector2Int());
    }

    private void AllMove(Vector2 dest, float max = 5)
    {
        Vector2 vec = dest - gameObject.ToVector2Int();
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
        gameObject.transform.rotation = Quaternion.Euler(0, Mathf.Atan2(vec.y, vec.x), 0);
        vec = vec.normalized;
        gameObject.transform.Translate(vec.x * Time.deltaTime, 0, vec.y * Time.deltaTime, Space.World);
        gameObject.transform.rotation = Quaternion.Euler(rot);
    }
}
