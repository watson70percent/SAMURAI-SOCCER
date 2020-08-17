﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json;

public class FieldMaker : EditorWindow
{
    [MenuItem("Coustom/FieldMaker")]
    static void ClickButton()
    {
        var info = new FieldInfo();
        info.acc_up_coeff = new float[60][];
        for(int i = 0; i < 60; i++)
        {
            info.acc_up_coeff[i] = new float[100];
            for(int j = 0; j < 100; j++)
            {
                info.acc_up_coeff[i][j] = 1f;
            }
        }

        info.acc_down_coeff = new float[60][];
        for (int i = 0; i < 60; i++)
        {
            info.acc_down_coeff[i] = new float[100];
            for (int j = 0; j < 100; j++)
            {
                info.acc_down_coeff[i][j] = 0.05f;
            }
        }

        var str = JsonConvert.SerializeObject(info);
        File.WriteAllText(Application.streamingAssetsPath + "/Field_1.json", str);
    }
}