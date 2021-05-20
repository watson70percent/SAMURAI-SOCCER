using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISafeAreaSupport : MonoBehaviour
{
    private void Awake()
    {
        var safeArea = Screen.safeArea;
        var screenArea = new Rect(0, 0, Screen.width, Screen.height);
        if (safeArea != screenArea)
        {
            var scale = new Vector2(safeArea.width / screenArea.width, safeArea.height / screenArea.height);
            foreach (var obj in FindObjectsOfType<CanvasScaler>())
            {
                var diff = new Vector2(safeArea.x, safeArea.y);
                diff.x = diff.x * obj.referenceResolution.x / screenArea.width;
                diff.y = diff.y * obj.referenceResolution.y / screenArea.height;
                SetChild(obj.gameObject, diff, scale);
            }
        }
    }

    private void SetChild(GameObject parent, Vector2 diff, Vector2 scale)
    {
        foreach(var child in parent.GetComponentsInChildren<RectTransform>())
        {
            var anchorMin = child.anchorMin;
            anchorMin.x = anchorMin.x * scale.x + diff.x;
            anchorMin.y = anchorMin.y * scale.y + diff.y;
            
            var anchorMax = child.anchorMax;
            anchorMax.x = anchorMax.x * scale.x + diff.x;
            anchorMax.y = anchorMax.y * scale.y + diff.y;

            child.anchorMin = anchorMin;
            child.anchorMax = anchorMax;

            SetChild(child.gameObject, diff, scale);
        }
    }
}
