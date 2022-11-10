using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Linq;
using SamuraiSoccer.SoccerGame.AI;
using SamuraiSoccer.SoccerGame;

public class TeamMaker : EditorWindow
{
    private string path_name = "";
    private Team team = default;

    private float min_hp = 1;
    private float max_hp = 2;

    private float min_power = 1;
    private float max_power = 2;

    private bool ally = false;

    private float min_seelen = 10;
    private float max_seelen = 20;

    private float min_fast = 0.1f;
    private float max_fast = 3;

    private int generate_num = 1;

    private Vector2 pos = Vector2.zero;

    [MenuItem("Custom/TeamMaker")]
    static void OpenWindow()
    {
        GetWindow<TeamMaker>();
    }

    private void OnGUI()
    {
        path_name = EditorGUILayout.TextField("Team name", path_name);

        if (GUILayout.Button("Load"))
        {
            LoadTeam();
        }

        if (GUILayout.Button("Save"))
        {
            SaveTeam();
        }

        EditorGUILayout.Space();
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
        EditorGUILayout.Space();

        min_hp = EditorGUILayout.FloatField("Min max HP", min_hp);
        max_hp = EditorGUILayout.FloatField("Max max HP", max_hp);
        EditorGUILayout.MinMaxSlider("Max HP", ref min_hp, ref max_hp, 1, 10);
        min_power = EditorGUILayout.FloatField("Min kick power", min_power);
        max_power = EditorGUILayout.FloatField("Max kick power", max_power);
        EditorGUILayout.MinMaxSlider("Kick power", ref min_power, ref max_power, 1, 25);
        ally = EditorGUILayout.Toggle("味方", ally);
        min_seelen = EditorGUILayout.FloatField("Min see len", min_seelen);
        max_seelen = EditorGUILayout.FloatField("Max see len", max_seelen);
        EditorGUILayout.MinMaxSlider("See len", ref min_seelen, ref max_seelen, 1, 50);
        EditorGUILayout.HelpBox("See len は15あたりが最適", MessageType.None);
        min_fast = EditorGUILayout.FloatField("Min fast", min_fast);
        max_fast = EditorGUILayout.FloatField("Max fast", max_fast);
        EditorGUILayout.MinMaxSlider("Fast", ref min_fast, ref max_fast, 0, 10);

        EditorGUILayout.Space();

        generate_num = EditorGUILayout.IntField("Generate number", generate_num);

        if (GUILayout.Button("Generate"))
        {
            Generate();
        }

        EditorGUILayout.Space();
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
        EditorGUILayout.Space();

        if (team != null)
        {
            pos = EditorGUILayout.BeginScrollView(pos);
            for(int i = 0; i < team.member.Count; i++)
            {
                var member = team.member[i];
                EditorGUILayout.BeginVertical(GUI.skin.box);
                member.MAX_HP = EditorGUILayout.IntField("Max HP", member.MAX_HP);
                member.power = EditorGUILayout.FloatField("Kick power", member.power);
                member.ally = EditorGUILayout.Toggle("味方", member.ally);
                member.seelen = EditorGUILayout.FloatField("See len", member.seelen);
                member.fast = EditorGUILayout.FloatField("Fast", member.fast);
                if (GUILayout.Button("Delete"))
                {
                    Delete(member);
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();
        }
    }

    private void LoadTeam()
    {
        try
        {
            team = JsonConvert.DeserializeObject<Team>(File.ReadAllText(Application.streamingAssetsPath + "/" + path_name + ".json"));
        }
        catch (Exception)
        {
            EditorUtility.DisplayDialog("エラー", "そのようなファイルはありません。", "OK");
        }

    }

    private void SaveTeam()
    {
        if (path_name == "")
        {
            EditorUtility.DisplayDialog("エラー", "名前を入力してください。", "OK");
            return;
        }

        foreach(var member in team.member)
        {
            member.hp = member.MAX_HP;
        }

        File.WriteAllText(Application.streamingAssetsPath + "/" + path_name + ".json", JsonConvert.SerializeObject(team, Formatting.Indented));

        EditorUtility.DisplayDialog("成功", "チームを上書きしました。", "OK");
    }

    private void Generate()
    {
        if (team == null)
        {
            team = new Team();
        }
        try
        {
            if (team.member.First().ally == ally)
            {
                EditorUtility.DisplayDialog("エラー", "裏切者がいます。", "OK");
                return;
            }
        }
        catch (Exception)
        {

        }

        for (int i = 0; i < generate_num; i++)
        {
            int maxHP = Mathf.FloorToInt(UnityEngine.Random.Range(min_hp, max_hp));
            float power = UnityEngine.Random.Range(min_power, max_power);
            float seelen = UnityEngine.Random.Range(min_seelen, max_seelen);
            float fast = UnityEngine.Random.Range(max_fast, min_fast);

            var member = new PersonalStatus();

            member.ally = ally;
            member.MAX_HP = maxHP;
            member.power = power;
            member.seelen = seelen;
            member.fast = fast;

            team.member.Add(member);
        }
    }

    private void Delete(PersonalStatus member)
    {
        team.member.Remove(member);
    }
}
