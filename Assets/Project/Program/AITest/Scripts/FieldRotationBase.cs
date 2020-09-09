using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// フィールドの傾きを持ったクラス。ここから派生。
/// </summary>
public abstract class FieldRotationBase : MonoBehaviour
{
    /// <summary>
    /// 回転
    /// </summary>
    public Quaternion rotation;

    /// <summary>
    /// 初期化。ご自由に。
    /// </summary>
    protected virtual void Start()
    {
        rotation = Quaternion.identity;
    }

    public Vector3 AdaptPosition(Vector3 pos)
    {
        var center = new Vector3(Constants.Width / 2, 0, Constants.G2G / 2);
        var diff = pos - center;
        var x = (rotation.w * rotation.w + rotation.x * rotation.x - rotation.y * rotation.y - rotation.z * rotation.z) * diff.x + 2 * (rotation.x * rotation.y - rotation.w * rotation.z) * diff.y + 2 * (rotation.x * rotation.z + rotation.w * rotation.y) * diff.z;
        var y = 2 * (rotation.x * rotation.y + rotation.w * rotation.z) * diff.x + (rotation.w * rotation.w - rotation.x * rotation.x + rotation.y * rotation.y - rotation.z * rotation.z) * diff.y + 2 * (rotation.y * rotation.z - rotation.w * rotation.x) * diff.z;
        var z = 2 * (rotation.x * rotation.z - rotation.w * rotation.y) * diff.x + 2 * (rotation.y * rotation.z + rotation.w * rotation.x) * diff.y + (rotation.w * rotation.w - rotation.x * rotation.x - rotation.y * rotation.y + rotation.z * rotation.z) * diff.z;
        return new Vector3(x, y, z) + center;    
    
    }
}
