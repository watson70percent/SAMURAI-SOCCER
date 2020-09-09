using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Linq;

/// <summary>
///　フィールド情報を管理する。
/// </summary>
[DefaultExecutionOrder(-1)]
public class FieldManager : MonoBehaviour
{
    public GameObject root;

    [NonSerialized]
    public FieldInfo info = default;
    public EasyCPUManager manager;
    public GameManager gm;

    public GameObject ball;
    private Rigidbody ball_rb = default;

    private WindInfoBase wind = default;
    public FieldRotationBase rotation = default;

    private bool isPlaying = false;

    void Awake()
    {
        info = JsonConvert.DeserializeObject<FieldInfo>(File.ReadAllText(Application.streamingAssetsPath + "/Field_" + FieldNumber.no + ".json"));
        ball_rb = ball.GetComponent<Rigidbody>();
        gameObject.AddComponent(typeof(NonWind));
        wind = GetComponents<WindInfoBase>().First();
        gameObject.AddComponent(typeof(NonRotation));
        rotation = GetComponents<FieldRotationBase>().First();
        gm.StateChange += StateChanged;
    }

    private void Update()
    {
        root.transform.rotation = rotation.rotation;
    }

    private void FixedUpdate()
    {
        if (isPlaying)
        {
            manager.team.ForEach(member =>
            {
                manager.rbs[member].AddForce(wind.WindEffect(member.transform.position));
            });

            manager.opp.ForEach(member =>
            {
                manager.rbs[member].AddForce(wind.WindEffect(member.transform.position));
            });

            ball_rb.AddForce(wind.WindEffect(ball.transform.position));
        }
    }

    /// <summary>
    /// 回転後の位置に変換
    /// </summary>
    /// <param name="pos">元の場所</param>
    /// <returns>変換後の場所</returns>
    public Vector3 AdaptPosition(Vector3 pos)
    {
        return rotation.AdaptPosition(pos);
    }

    private void StateChanged(StateChangedArg e)
    {
        if (e.gameState == GameState.Pause)
        {
            isPlaying = false;
        }

        if (e.gameState == GameState.Playing)
        {
            isPlaying = true;
        }
    }
}
