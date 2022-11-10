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
                            //CanvasScalerの直下にあるオブジェクトのサイズを変更
                            //(孫オブジェクトは親オブジェクトのスケール変更の影響を受けるので孫オブジェクトのサイズはいじらない)
                        {
                            var pos = child.localPosition;
                            pos.x *= scale.x;
                            pos.y *= scale.y;

                            child.localPosition = pos;
                            var localScale = child.localScale;
                            localScale.x = localScale.x * s;
                            localScale.y = localScale.y * s;
                            child.localScale = localScale;
                        }
                    }
                }
            }
        }
    }
}
