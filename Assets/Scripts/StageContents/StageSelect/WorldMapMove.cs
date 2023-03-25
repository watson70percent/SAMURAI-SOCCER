using SamuraiSoccer.Event;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

using Cysharp.Threading.Tasks;
using System.Linq;

namespace SamuraiSoccer.StageContents.StageSelect
{
    /// <summary>
    /// ワールドマップを移動するクラス．関数を呼んで移動させる．
    /// </summary>
    public class WorldMapMove : MonoBehaviour
    {
        [SerializeField]
        private float moveTime = 1.0f;

        [SerializeField]
        private float accMax = 10;

        [SerializeField]
        private float displaySize;

        [SerializeField]
        private RectTransform worldMap;

        private Stage m_selectState = Stage.World;
        private Vector2 m_currentPoint = m_worldPoint[Stage.World];
        private Queue<Vector2> m_pointHistory = new Queue<Vector2>();
        private float m_currentSize = m_worldSize[Stage.World];
        private Stack<Stage> m_moving = new Stack<Stage>();
        private Vector2 m_velocity = Vector2.zero;
        private bool m_isFloatingStop = false;

        private static readonly IReadOnlyDictionary<Stage, Vector2> m_worldPoint = new Dictionary<Stage, Vector2>() {
            { Stage.Japan, new Vector2(745, 690) },
            { Stage.UK, new Vector2(90, 572)},
            { Stage.China, new Vector2(600, 705) },
            { Stage.USA, new Vector2(1310, 685)},
            { Stage.Russian, new Vector2(550, 450) },
            { Stage.World, new Vector2(925, 650) }
        };

        private static readonly IReadOnlyDictionary<Stage, float> m_worldSize = new Dictionary<Stage, float>()
        {
            { Stage.Japan, 120 },
            { Stage.UK, 80},
            { Stage.China, 160 },
            { Stage.USA, 180 },
            { Stage.Russian, 300 },
            { Stage.World, 850 }
        };

        private void Awake()
        {
            m_pointHistory.Enqueue(m_currentPoint);
        }

        private void FixedUpdate()
        {
            if (m_pointHistory.Count > 1)
            {
                m_pointHistory.Dequeue();
            }
            else
            {
                m_velocity = Vector2.zero;
            }
        }

        private async void Start()
        {
            // さいしょのデータを取得する．デフォルトでワールドの位置にしてる．
            SetPosition(m_worldPoint[Stage.World], m_worldSize[Stage.World]);
        }

        /// <summary>
        /// フローティング状態にする．
        /// </summary>
        /// <param name="token">キャンセル用トークン．</param>
        public async UniTask ToFloating()
        {
            m_isFloatingStop = false;
            for (var i = 0; i < 30; i++)
            {
                var nextScale = m_worldSize[m_selectState] * (1.0f + i * 0.5f / 30.0f);
                SetPosition(m_worldPoint[m_selectState], nextScale);
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
                if (m_isFloatingStop)
                {
                    return;
                }
            }

            await UniTask.DelayFrame(10, PlayerLoopTiming.FixedUpdate);

            float time = 0.0f;
            while (true)
            {
                var nextPoint = m_worldPoint[m_selectState];
                nextPoint.y -= m_worldSize[m_selectState] / 15.0f * Mathf.Sin(time * 3);
                SetPosition(nextPoint, m_worldSize[m_selectState] * 1.5f);
                await UniTask.Yield();
                time += Time.deltaTime;
                if (m_isFloatingStop)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// 指定した国へ移動．
        /// </summary>
        /// <param name="country">指定した国</param>
        public async UniTask GoTo(Stage country)
        {
            m_isFloatingStop = true;
            m_moving.Clear();
            m_moving.Push(country);
            var startSize = m_currentSize;
            var goal = GetObjectPoint(m_currentPoint, m_worldPoint[country]);
            var distance = (m_currentPoint - goal).magnitude;

            float moved = 0.0f;
            List<float> velocity = new List<float>();
            var acc = 1;
            while (moved < distance)
            {
                var v = acc * accMax;
                if (v * v > m_velocity.sqrMagnitude)
                {
                    break;
                }
                
                velocity.Insert(0, v);
                moved += v;
                velocity.Insert(1, v - accMax / 2.0f);
                moved += v - accMax / 2.0f;
                acc++;
            }

            var idx = 0;
            while (true)
            {
                var v = acc * accMax;
                if (v * Mathf.Max(0, moveTime * 60 - velocity.Count) + moved > distance)
                {
                    break;
                }
                velocity.Insert(idx, v);
                velocity.Insert(idx + 1, v);
                velocity.Insert(idx + 2, v - accMax / 2.0f);
                moved += 2 * v;
                moved += v - accMax / 2.0f;
                acc++;
                idx++;
            }

            var linerCount = moveTime * 60 - velocity.Count;
            for (var i = 0; i < linerCount; i++)
            {
                velocity.Insert(idx, acc * accMax);
            }

            List<float> t = new List<float>();
            float tSum = 0.0f;
            foreach(var v in velocity)
            {
                tSum += v;
                t.Add(tSum);
            }

            t = t.Select(val => val / tSum).ToList();

            var inertia = m_velocity * (m_velocity.sqrMagnitude < 0.1 ? 1000 : 30);
            Debug.Log(inertia);
            var path = new Bezier2Order3Points(m_currentPoint, m_currentPoint + inertia, goal);

            foreach(var val in t)
            {
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
                if (m_moving.Peek() != country)
                {
                    return;
                }
                SetPosition(path.GetPoint(val), m_currentSize);
            }

            for (var i = 0; i < 5; i++)
            {
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
                if (m_moving.Peek() != country)
                {
                    return;
                }
            }

            for (var i = 0; i < 30; i++)
            {
                var nextScale = Mathf.Lerp(m_worldSize[country],  startSize, 1 - i / 30.0f);
                SetPosition(m_worldPoint[country], nextScale);
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
                if (m_moving.Peek() != country)
                {
                    return;
                }
            }
            m_selectState = country;
        }

        /// <summary>
        /// 目的地を取得．
        /// </summary>
        /// <param name="src">出発点．</param>
        /// <param name="dst">目的地．</param>
        /// <returns>目的地．</returns>
        private Vector2 GetObjectPoint(Vector2 src, Vector2 dst)
        {
            var possible = new List<Vector2>();
            for (var i = -1; i < 2; i++)
            {
                possible.Add(new Vector2(dst.x + 1850 * i, dst.y));
                possible.Add(new Vector2(-dst.x + 1850 * (i + 1), -dst.y));
            }

            float distance = float.MaxValue;
            Vector2 ret = Vector2.zero;
            foreach(var p in possible)
            {
                var d = (src - p).sqrMagnitude;
                if (d < distance)
                {
                    distance = d;
                    ret = p;
                }
            }

            return ret;
        }

        /// <summary>
        /// マップを移動．
        /// </summary>
        /// <param name="localPos">マップ上のピクセル位置．</param>
        /// <param name="localSize">表示マップの1辺のピクセル．</param>
        private void SetPosition(Vector2 localPos, float localSize)
        {
            var normalizedLocalPos = localPos;
            normalizedLocalPos.y = Mathf.Abs(normalizedLocalPos.y);
            normalizedLocalPos.x = (normalizedLocalPos.x + 1850) % 1850;
            m_velocity = normalizedLocalPos - m_currentPoint;
            m_currentPoint = normalizedLocalPos;
            m_pointHistory.Enqueue(normalizedLocalPos);
            m_currentSize = localSize;

            var sizeRatio = displaySize / localSize;
            worldMap.offsetMax = m_worldPoint[Stage.World] * sizeRatio;
            worldMap.offsetMin = -m_worldPoint[Stage.World] * sizeRatio;
            var diffPos = m_worldPoint[Stage.World] - normalizedLocalPos;
            worldMap.localPosition = new Vector3(diffPos.x, -diffPos.y) * sizeRatio / 3.0f;
        }
    }
}
