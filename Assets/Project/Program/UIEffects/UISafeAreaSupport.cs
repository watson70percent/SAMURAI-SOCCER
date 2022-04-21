using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(1)]
public class UISafeAreaSupport : MonoBehaviour
{
    private void Start()
    {
        var safeArea = Screen.safeArea;
        var screenArea = new Rect(0, 0, Screen.width, Screen.height);
        if (safeArea != screenArea)
        {
            var scale = new Vector2(safeArea.width / screenArea.width, safeArea.height / screenArea.height);
            var s = scale.x < scale.y ? scale.x : scale.y;
            Debug.Log("Scaled to " + s);
            var diff = new Vector2(safeArea.x, safeArea.y);
            foreach (var obj in FindObjectsOfType<CanvasScaler>())
            {
                foreach(var child in obj.gameObject.GetComponentsInChildren<RectTransform>())
                {
                    if (child.gameObject != obj.gameObject)
                    {
                        if (child.parent.gameObject == obj.gameObject)
                        {
                            var pos = child.localPosition;
                            pos.x *= scale.x;
                            pos.y *= scale.y;

                            child.localPosition = pos;
                        }
                        var delta = child.sizeDelta;
                        delta.x = delta.x * s;
                        delta.y = delta.y * s;
                        child.sizeDelta = delta;
                    }
                }
            }
        }
    }
}
