using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SamuraiSoccer.SoccerGame.EditorExteision
{
    [CustomEditor(typeof(StagePrefabManager))]
    public class StagePrefabManagerEditor : Editor
    {

        private StagePrefabManager _target;
        private bool _isOpen;
        private int _num = 0;
        private Texture[] _textures = new Texture[10];
        bool crowd, useNormalMap;

        private void Awake()
        {
            _target = target as StagePrefabManager;

            for (int i = 0; i < 10; i++)
            {
                _textures[i] = (_target.crowdTextures?.Length > i) ? _target.crowdTextures[i] : null;
            }
        }

        public override void OnInspectorGUI()
        {


            serializedObject.Update();
            EditorGUI.indentLevel++;

            var flagTexture = serializedObject.FindProperty("flagTexture");
            flagTexture.objectReferenceValue = EditorGUILayout.ObjectField("旗の画像", _target.flagTexture, typeof(Texture), false) as Texture;


            serializedObject.FindProperty("groundTexture").objectReferenceValue = EditorGUILayout.ObjectField("グラウンドの画像", _target.groundTexture, typeof(Texture), false) as Texture;

            if (crowd = EditorGUILayout.Foldout(crowd, "観客の画像"))
            {
                EditorGUI.indentLevel++;
                _num = Mathf.Clamp(EditorGUILayout.IntField("Size", _target.crowdTextures.Length), 1, 10);
                Texture[] textures = new Texture[_num];
                for (int i = 0; i < _target.crowdTextures.Length && i < textures.Length; i++) { textures[i] = _target.crowdTextures[i]; }
                _target.crowdTextures = textures;
                SerializedProperty crowdTextures = serializedObject.FindProperty("crowdTextures");
                for (int i = 0; i < _num; i++)
                {
                    _textures[i] = EditorGUILayout.ObjectField(" ", _textures[i], typeof(Texture), false, GUILayout.MaxHeight(16)) as Texture;
                    if (crowdTextures.arraySize != _num) { return; }
                    crowdTextures.GetArrayElementAtIndex(i).objectReferenceValue = _textures[i];
                }
                EditorGUI.indentLevel--;

            }

            EditorGUILayout.LabelField("審判の動き");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("refereeMaxAng"), new GUIContent("視野角の半分"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("refereeAreaSize"), new GUIContent("視力"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("runningSpeed"), new GUIContent("スピード"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("lookAtSpeed"), new GUIContent("振り向くスピード"));
            if (serializedObject.FindProperty("useObstacles").boolValue = EditorGUILayout.Toggle("障害物によって審判の視界が遮られるようにする", _target.useObstacles))
            {
                EditorGUILayout.HelpBox("LayerMaskが8-Obstaclesに設定されたオブジェクトだけが遮られます", MessageType.Info);
            }
            EditorGUI.indentLevel--;


            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            _isOpen = EditorGUILayout.Foldout(_isOpen, "");
            if (_isOpen)
            {
                EditorGUI.indentLevel++;

                serializedObject.FindProperty("flagShader").objectReferenceValue = EditorGUILayout.ObjectField("Shader of Flag", _target.flagShader, typeof(Shader), false) as Shader;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("flagRenderers"), new GUIContent("Renderers of Flags"), true);
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                serializedObject.FindProperty("groundMaterial").objectReferenceValue = EditorGUILayout.ObjectField("Material of Ground", _target.groundMaterial, typeof(Material), false) as Material;
                serializedObject.FindProperty("groundRenderer").objectReferenceValue = EditorGUILayout.ObjectField("Render of Ground", _target.groundRenderer, typeof(Renderer), true) as Renderer;
                serializedObject.FindProperty("groundNormalMap").objectReferenceValue = EditorGUILayout.ObjectField("Normal Map of Ground", _target.groundNormalMap, typeof(Texture), false, GUILayout.MaxHeight(16)) as Texture;
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                serializedObject.FindProperty("crowdMaterial").objectReferenceValue = EditorGUILayout.ObjectField("Material of Crowd", _target.crowdMaterial, typeof(Material), false) as Material;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("crowdRenderers"), new GUIContent("Renderers of Crowd"), true);
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("refereeArea"), new GUIContent("RefereeArea"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("refereeMove"), new GUIContent("RefereeMove"), true);
                EditorGUI.indentLevel--;

                EditorGUI.indentLevel--;
            }




            serializedObject.ApplyModifiedProperties();
        }
    }
}

