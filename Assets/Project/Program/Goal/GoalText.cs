﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalText : MonoBehaviour
{
    public Text text;

    private void OnEnable()
    {
        StartCoroutine(ColorChange());
    }

    private IEnumerator ColorChange()
    {
        float time = 0;
        while (time < 4)
        {
            text.color = new Color(0, 0, 0, (4 - time) / 2);
            yield return null;
            time += Time.deltaTime;
        }
    }
}