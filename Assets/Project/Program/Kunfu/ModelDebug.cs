using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelDebug : MonoBehaviour
{

    public string prefabNameAndjsonName;
    private void Awake()
    {
        OpponentName.name = prefabNameAndjsonName;
    }
}
