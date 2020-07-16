using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointScript : MonoBehaviour
{

    public Text text;
    public bool leftSide;
    int[] point= {0,0};
    public BallControler ballControler;

    public void Point(object sender, GoalEventArgs e)
    {
        GainPoint(e);
    }

    // Start is called before the first frame update
    void Start()
    {
        text.text = "0-0";
    }

    private void Awake()
    {
        ballControler.Goal += Point;
    }
    // Update is called once per frame
    void Update()
    {
        text.text = point[0].ToString() + " - " + point[1].ToString();
    }

    public void GainPoint(GoalEventArgs e)
    {
        if (e.Ally == leftSide)
        {
            point[0]++;
        }
        else
        {
            point[1]++;
        }
        
    }

    public void ResetPoint(bool p)
    {
        //point[p] = 0;
    }

    public int GetPoint(int p)
    {
        return point[p];
    }
}
