using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class FieldManager : MonoBehaviour
{
    public FieldInfo info = default;

    void Awake()
    {
        info = JsonConvert.DeserializeObject<FieldInfo>(File.ReadAllText(Application.streamingAssetsPath + "/Field_" + FieldNumber.no + ".json"));
    }

}
