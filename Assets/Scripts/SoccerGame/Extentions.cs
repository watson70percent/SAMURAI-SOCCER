using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiSoccer.SoccerGame
{
    public static class Extentions
    {
        public static Vector2Int ToVector2Int(this GameObject obj)
        {
            return new Vector2Int(Mathf.FloorToInt(obj.transform.position.x), Mathf.FloorToInt(obj.transform.position.z));
        }

        public static Vector2 ToVector2(this Transform t)
        {
            return new Vector2(t.position.x, t.position.z);
        }
    }
}