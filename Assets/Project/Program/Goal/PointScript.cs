using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointScript : MonoBehaviour
{

    Text text;

    int[] point= {0,0}; 


    // Start is called before the first frame update
    void Start()
    {
        text = gameObject.GetComponent<Text>();
        text.text = "0-0";
    }

    // Update is called once per frame
    void Update()
    {
        text.text = point[0].ToString() + " - " + point[1].ToString();
    }

    public void GainPoint(GoalEventArgs e)
    {
        if (e.Ally)
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
