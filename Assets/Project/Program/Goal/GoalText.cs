using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GoalText : MonoBehaviour
{
    public TextMeshProUGUI text;

    private float time = 100;

    public string TextContent 
    { 
        set 
        { 
            text.text = value; 
            time = 0; 
        } 
    }

    private void Update()
    {
        text.color = new Color(0, 0, 0, (4 - time) / 2);
        time += time > 100 ? 0 : Time.deltaTime;
    }
}
