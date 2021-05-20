using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISafeAreaSupport : MonoBehaviour
{
    public CanvasScaler scaler;

    private void Awake()
    {
        if(Screen.safeArea != new Rect(0, 0, Screen.width, Screen.height))
        {
            var reference = new Vector2(scaler.referenceResolution.x * Screen.width / Screen.safeArea.width, scaler.referenceResolution.y * Screen.height / Screen.safeArea.height);
            scaler.referenceResolution = reference;
        }
    }
}
