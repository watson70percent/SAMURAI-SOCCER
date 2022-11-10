using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using SamuraiSoccer.Event;
using System;

/// <summary>
/// 自由の女神の状態
/// </summary>
public enum StatueMode { Idle, Rise, FallDown };

namespace SamuraiSoccer.StageContents.USA
{
    /// <summary>
    /// 自由の女神が上がってきて倒れる動きの監視+実効
    /// </summary>
    public class StatueMove : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_bodyObj; //自由の女神像本体

        [SerializeField]
        private GameObject m_shadeObj; //自由の女神の影

        [SerializeField]
        private SpriteRenderer m_spriteRenderer; //影についているSpriteRenderer

        /// <summary>
        /// 自由の女神の状態を管理する内部変数
        /// </summary>
        public StatueMode CurrentStatueMode { get; private set; } = StatueMode.Idle;

        private float m_time = 0f; //影のために経過時間を保存

        private void Start()
        {
            m_shadeObj.transform.position = new Vector3(m_shadeObj.transform.position.x, 0.1f, m_shadeObj.transform.position.z);
            //ゴールが入ったらStandbyで削除
            InGameEvent.Standby.Subscribe(_ =>
            {
                Destroy(gameObject);
            }).AddTo(this);
            //アップデートで0.1秒ごとに呼び出す処理
            InGameEvent.UpdateDuringPlay.ThrottleFirst(System.TimeSpan.FromSeconds(0.1)).Subscribe(_ =>
            {
                MoveStatue();
            }).AddTo(this);
        }

        /// <summary>
        /// 自由の女神が移動して倒れ、その場で制止する。併せて影が点滅する。終了後削除される。
        /// </summary>
        private void MoveStatue()
        {
            m_time += Time.deltaTime;
            //自由の女神が上昇、影が点滅
            if (m_bodyObj.transform.position.y < 0.95f)
            {
                CurrentStatueMode = StatueMode.Rise;
                m_bodyObj.transform.position += new Vector3(0f, 1.0f, 0f);
                m_spriteRenderer.color = new Color(1f, 1f, 1f, 0.4f * (1f + Mathf.Sin(1000 * m_time * m_time)));
                return;
            }
            //影を一定値で固定、自由の女神が倒れる
            if (m_bodyObj.transform.eulerAngles.z < 90)
            {
                CurrentStatueMode = StatueMode.FallDown;
                m_spriteRenderer.color = new Color(1f, 1f, 1f, 0.8f);
                m_bodyObj.transform.eulerAngles += new Vector3(0f, 0f, 1.0f);
                return;
            }
            //3秒静止して削除
            if (CurrentStatueMode != StatueMode.Idle)
            {
                CurrentStatueMode = StatueMode.Idle;
                Observable.Timer(System.TimeSpan.FromSeconds(3))
                    .Subscribe(_ => Destroy(gameObject));
            }
        }
    }
}

