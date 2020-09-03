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
    [NonSerialized]
    public FieldInfo info = default;
    public EasyCPUManager manager;
    public GameManager gm;

    public GameObject ball;
    private Rigidbody ball_rb = default;

    private WindInfoBase wind = default;

    private bool isPlaying = false;


    void Awake()
    {
        info = JsonConvert.DeserializeObject<FieldInfo>(File.ReadAllText(Application.streamingAssetsPath + "/Field_" + FieldNumber.no + ".json"));
        ball_rb = ball.GetComponent<Rigidbody>();
        gameObject.AddComponent(typeof(NonWind));
        wind = GetComponents<WindInfoBase>().First();
        gm.StateChange += StateChanged;
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
