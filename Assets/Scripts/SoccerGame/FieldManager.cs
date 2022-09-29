using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Linq;
using SamuraiSoccer.Event;
using UniRx;

namespace SamuraiSoccer.SoccerGame
{
    /// <summary>
    ///　フィールド情報を管理する。
    /// </summary>
    [DefaultExecutionOrder(-1)]
    public class FieldManager : MonoBehaviour
    {
        public GameObject root;

        [NonSerialized]
        public FieldInfo info = default;

        public GameObject ball;
        private Rigidbody ball_rb = default;

        private WindInfoBase wind = default;
        public FieldRotationBase rotation = default;

        private int groundNumber  = 0;
        public int  GroundNumber { get => groundNumber; }

        void Awake()
        {
            var client = new InMemoryDataTransitClient<int>();
            client.TryGet(StorageKey.KEY_GROUNDNUMBER, out groundNumber);
            StartCoroutine(LoadField());
            ball_rb = ball.GetComponent<Rigidbody>();
            gameObject.AddComponent(typeof(NonWind));
            wind = GetComponents<WindInfoBase>().First();
            gameObject.AddComponent(typeof(NonRotation));
            rotation = GetComponents<FieldRotationBase>().First();
        }

        private void Update()
        {
            root.transform.rotation = rotation.rotation;
        }

        private IEnumerator LoadField()
        {
            var file_path1 = Path.Combine(Application.streamingAssetsPath, "Field_" + groundNumber + ".json");
            string json = "";
            Debug.Log("filepath is " + file_path1);
            if (file_path1.Contains("://"))
            {
                var www1 = UnityEngine.Networking.UnityWebRequest.Get(file_path1);
                yield return www1.SendWebRequest();
                json = www1.downloadHandler.text;
            }
            else
            {
                json = File.ReadAllText(file_path1);
            }
            info = JsonConvert.DeserializeObject<FieldInfo>(json);

            yield return null;

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

        /// <summary>
        /// 開店前の位置に変換
        /// </summary>
        /// <param name="pos">元の場所</param>
        /// <returns>変換後の場所</returns>
        public Vector3 AdaptInversePosition(Vector3 pos)
        {
            return rotation.AdaptInversePosition(pos);
        }
    }
}