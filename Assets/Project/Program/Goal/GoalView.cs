using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalView : MonoBehaviour
{
    public Image image;

    private void OnEnable()
    {
        StartCoroutine(ColorChange());
    }

    private IEnumerator ColorChange()
    {
        float time = 0;
        while (time < 5)
        {
            image.color = new Color(0, 0, 0, 1 - Mathf.Abs(4 - time) * 2);
            yield return null;
            time += Time.deltaTime;
        }
    }
}
