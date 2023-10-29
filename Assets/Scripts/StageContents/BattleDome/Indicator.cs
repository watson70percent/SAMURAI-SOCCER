using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using System;
using Newtonsoft.Json;

namespace SamuraiSoccer.StageContents.BattleDome
{
    public class Indicator : MonoBehaviour
    {
        public GameObject indicator;
        public AudioSource bgm;
        public Vector3 center;
        public int h;
        public float width;
        public float height;
        public int w_div;
        public int h_div;
        public float quarter;
        public float floor;
        public float low;
        public float high;

        private GameObject[][] objs;
        private BGMData bgmData;
        private DateTime start_time;
        private List<float> level_table;
        private List<List<int>> f_table;

        // Start is called before the first frame update
        void Start()
        {
            var time = DateTime.Now;
            var textData = File.ReadAllText("D:\\Program\\VSCode\\fft\\侍サッカー2_ver_full_flavor.json");
            var dt = DateTime.Now - time;
            time = DateTime.Now;
            Debug.Log("t1 is:" + dt.TotalMilliseconds);
            bgmData = JsonConvert.DeserializeObject<BGMData>(textData);
            dt = DateTime.Now - time;
            time = DateTime.Now;
            Debug.Log("t2 is:" + dt.TotalMilliseconds);
            var i_width = width / w_div;
            var i_height = height / h_div;
            var z_scale = i_width;
            var xy_scale = i_height / 0.2f;
            var scale = new Vector3(xy_scale, xy_scale, z_scale);
            var w_start = center.z - width / 2 + i_width / 2;
            var h_start = h switch
            {
                0 => center.x - height / 2 + i_height / 2,
                1 => center.y - height / 2 + i_height / 2,
                _ => throw new System.Exception("invalid h"),
            };
            var w_grid = Enumerable.Range(0, w_div).Select(i => w_start + i * i_width).Reverse().ToArray();
            var h_grid = Enumerable.Range(0, h_div).Select(i => h_start + i * i_height).ToArray();
            var grid = w_grid.Select(w_p =>
            {
                var r = h_grid.Select(h_p =>
                {
                    var r_ = h switch
                    {
                        0 => new Vector3(h_p, center.y, w_p),
                        1 => new Vector3(center.x, h_p, w_p),
                        _ => throw new System.Exception("invalid h"),
                    };
                    return r_;
                }).ToArray();
                return r;
            }).ToArray();

            dt = DateTime.Now - time;
            time = DateTime.Now;
            Debug.Log("t3 is:" + dt.TotalMilliseconds);

            objs = grid.Select(gr =>
            {
                var c = gr.Select(g =>
                {
                    var o = Instantiate<GameObject>(indicator, g, Quaternion.identity);
                    o.transform.localScale = scale;
                    o.SetActive(false);
                    return o;
                }).ToArray();
                return c;
            }).ToArray();

            dt = DateTime.Now - time;
            time = DateTime.Now;
            Debug.Log("t4 is:" + dt.TotalMilliseconds);

            var tqn = h_div * 3 / 4;
            var qn = h_div - h_div * 3 / 4;

            level_table = Enumerable.Range(0, qn).Select(i => (float)i / qn * quarter + (float)(qn - i) / qn * floor).ToList();
            level_table.AddRange(Enumerable.Range(0, tqn).Select(i => (tqn - i + 0.5f) / tqn * quarter));

            var low_ln = Mathf.Log10(low);
            var high_ln = Mathf.Log10(high);
            var f_span_ln = (high_ln - low_ln) / (w_div - 1);
            var f_th = Enumerable.Range(0, w_div).Select(i => Mathf.Pow(10, i * f_span_ln + low_ln)).ToList();
            f_th.Add(44100);

            f_table = new List<List<int>>();

            {
                int i = 0;
                f_table = f_th.Select(th =>
                {
                    var idxes = new List<int>();
                    while (i < bgmData.f_table.Count && bgmData.f_table[i] < th)
                    {
                        idxes.Add(i);
                        i++;
                    }
                    return idxes;
                }).ToList();
            }

            dt = DateTime.Now - time;
            time = DateTime.Now;
            Debug.Log("t5 is:" + dt.TotalMilliseconds);

            start_time = DateTime.Now.AddSeconds(5.0);
        }

        private void Update()
        {
            var t = DateTime.Now - start_time;

            if (t.TotalMilliseconds < 0)
            {
                return;
            }

            if (bgm.time >= bgm.clip.length)
            {
                foreach (var o in objs)
                {
                    foreach (var o_ in o)
                    {
                        o_.SetActive(false);
                    }
                }
                return;
            }

            if (!bgm.isPlaying)
            {
                bgm.Play();
            }

            var t_idx = Mathf.FloorToInt(bgm.time / bgmData.time_span);

            if (t_idx >= bgmData.data.Count)
            {
                return;
            }

            foreach (var (f_idxes, obj) in f_table.Zip(objs, (i, o) => (i, o)))
            {
                var avg = f_idxes.Select(i => bgmData.data[t_idx][i]).Average();
                foreach (var (th, o) in level_table.Zip(obj, (l, o) => (l, o)))
                {
                    if (avg > th)
                    {
                        o.SetActive(true);
                    }
                    else
                    {
                        o.SetActive(false);
                    }
                }
            }
        }
    }
}
