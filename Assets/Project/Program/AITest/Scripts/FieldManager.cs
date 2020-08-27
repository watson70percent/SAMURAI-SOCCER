using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System;

/// <summary>
///　フィールド情報を管理する。
/// </summary>
public class FieldManager : MonoBehaviour
{
    [NonSerialized]
    public FieldInfo info = default;
    public EasyCPUManager manager;

    public GameObject ball;
    private Rigidbody ball_rb = default;

    private WindInfoBase wind = default;

    void Awake()
    {
        info = JsonConvert.DeserializeObject<FieldInfo>(File.ReadAllText(Application.streamingAssetsPath + "/Field_" + FieldNumber.no + ".json"));
        ball_rb = ball.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
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
