using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 定数
/// </summary>
public static class Constants
{
    /// <summary>
    /// 自陣のゴール
    /// </summary>
    public static readonly Vector3 OurGoalPoint = new Vector3(30, 0, 0);

    /// <summary>
    /// 相手のゴール
    /// </summary>
    public static readonly Vector3 OppornentGoalPoint = new Vector3(30, 0, 100);

    /// <summary>
    /// フィールドの幅
    /// </summary>
    public static float Width
    {
        get
        {
            return 2 * OurGoalPoint.x;
        }
    }

    /// <summary>
    /// ゴール間の距離
    /// </summary>
    public static float G2G
    {
        get
        {
            return (OurGoalPoint - OppornentGoalPoint).magnitude;
        }
    }

    /// <summary>
    /// 味方のスポーン地点
    /// </summary>
    public static Vector3 TeammateSpornPoint
    {
        get
        {
            var vec = OurGoalPoint - OppornentGoalPoint;
            return OurGoalPoint + vec * 0.1f+Vector3.up;
        }
    }

    /// <summary>
    /// 相手のスポーン地点
    /// </summary>
    public static Vector3 OppornentSpornPoint
    {
        get
        {
            var vec = OppornentGoalPoint - OurGoalPoint;
            return OppornentGoalPoint + vec * 0.1f+Vector3.up;
        }
    }

    /// <summary>
    /// 味方ボールのときの味方の最初のスポーン場所
    /// </summary>
    public static Vector3[] TeammateInitialSpornPointCenterTeam
    {
        get
        {
            return teammateInitialSpornPointCenterTeam.Select(value => new Vector3(value.x * Width, 0.5f, value.z * G2G)).ToArray();
        }
    }

    /// <summary>
    /// 相手ボールのときの味方の最初のスポーン場所
    /// </summary>
    public static Vector3[] TeammateInitialSpornPointCenterOppornent
    {
        get
        {
            return teammateInitialSpornPointCenterOppornent.Select(value => new Vector3(value.x * Width, 0.5f, value.z * G2G)).ToArray();
        }
    }

    /// <summary>
    /// 味方ボールのときの相手の最初のスポーン場所
    /// </summary>
    public static Vector3[] OpprnentInitialSpornPointCenterTeam
    {
        get
        {
            return oppornentInitialSpornPointCenterTeam.Select(value => new Vector3(value.x * Width, 0.5f, value.z * G2G)).ToArray();
        }
    }

    /// <summary>
    /// 相手ボールのときの相手の最初のスポーン場所
    /// </summary>
    public static Vector3[] OpprnentInitialSpornPointCenterOppornent
    {
        get
        {
            return oppornentInitialSpornPointCenterOppornent.Select(value => new Vector3(value.x * Width, 0.5f, value.z * G2G)).ToArray();
        }
    }

    private static readonly IEnumerable<Vector3> teammateInitialSpornPointCenterTeam = new Vector3[]
    {
        new Vector3(0.5f,0,0.5f),
        new Vector3(0.8f,0,0.1f),new Vector3(0.2f,0,0.1f),new Vector3(0.6f,0,0.1f),new Vector3(0.4f,0,0.1f),
        new Vector3(0.8f,0,0.3f),new Vector3(0.2f,0,0.3f),new Vector3(0.6f,0,0.3f),new Vector3(0.4f,0,0.3f),
        new Vector3(0.75f,0,0.4f),new Vector3(0.25f,0,0.4f)
    };

    private static readonly IEnumerable<Vector3> teammateInitialSpornPointCenterOppornent = new Vector3[]
    {
        new Vector3(0.8f,0,0.1f),new Vector3(0.2f,0,0.1f),new Vector3(0.6f,0,0.1f),new Vector3(0.4f,0,0.1f),
        new Vector3(0.8f,0,0.3f),new Vector3(0.2f,0,0.3f),new Vector3(0.6f,0,0.3f),new Vector3(0.4f,0,0.3f),
        new Vector3(0.75f,0,0.4f),new Vector3(0.25f,0,0.4f),new Vector3(0.5f,0,0.4f)
    };

    private static readonly IEnumerable<Vector3> oppornentInitialSpornPointCenterTeam = new Vector3[]
    {
        new Vector3(0.8f,0,0.9f),new Vector3(0.2f,0,0.9f),new Vector3(0.6f,0,0.9f),new Vector3(0.4f,0,0.9f),
        new Vector3(0.8f,0,0.7f),new Vector3(0.2f,0,0.7f),new Vector3(0.6f,0,0.7f),new Vector3(0.4f,0,0.7f),
        new Vector3(0.75f,0,0.6f),new Vector3(0.25f,0,0.6f),new Vector3(0.5f,0,0.6f)
    };

    private static readonly IEnumerable<Vector3> oppornentInitialSpornPointCenterOppornent = new Vector3[]
    {
        new Vector3(0.5f,0,0.5f),
        new Vector3(0.8f,0,0.9f),new Vector3(0.2f,0,0.9f),new Vector3(0.6f,0,0.9f),new Vector3(0.4f,0,0.9f),
        new Vector3(0.8f,0,0.7f),new Vector3(0.2f,0,0.7f),new Vector3(0.6f,0,0.7f),new Vector3(0.4f,0,0.7f),
        new Vector3(0.75f,0,0.6f),new Vector3(0.25f,0,0.6f)
    };

}
