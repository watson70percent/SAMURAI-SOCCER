using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extentions
{
   public static Vector2Int ToVector2Int(this GameObject obj)
    {
        return new Vector2Int(Mathf.FloorToInt(obj.transform.position.x), Mathf.FloorToInt(obj.transform.position.z));
    }
}
