using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppSettings : MonoBehaviour
{
    private void Awake()
    {
        //フレームレートの設定
        Application.targetFrameRate = 60;
    }
}
