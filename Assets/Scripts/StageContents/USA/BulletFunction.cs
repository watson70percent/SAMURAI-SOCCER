using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using SamuraiSoccer.Event;

namespace SamuraiSoccer.StageContents.USA
{
    /// <summary>
    /// 弾丸との衝突の処理,自滅処理
    /// </summary>
    public class BulletFunction : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody m_rb; //このオブジェクトのRigidbody

        [SerializeField]
        private int m_SEIndex; //衝突時のSE

        private float m_timer = 0f; //自動削除タイマー

        private Vector3 m_tmpVelocity = Vector3.zero; //一時的に速度を保持する

        void Start()
        {
            //20秒後に自動で削除
            InGameEvent.UpdateDuringPlay.Subscribe(_ => DestroyTimer()).AddTo(this);
            //ゴールが入ったら削除
            InGameEvent.Goal.Subscribe(_ => Destroy(gameObject)).AddTo(this);
            //一時停止処理
            InGameEvent.Pause.Subscribe(x => PauseMove(x)).AddTo(this);
            //衝突時の処理
            this.OnTriggerEnterAsObservable().Subscribe(other =>
            {
                //プレイヤ―との衝突処理
                if (other.gameObject.tag == "Player")
                {
                    InGameEvent.FinishOnNext();
                    //衝突音の再生
                    SoundMaster.Instance.PlaySE(m_SEIndex);
                }
            });
        }

        private void DestroyTimer()
        {
            m_timer += Time.deltaTime;
            if (m_timer > 20)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// ポーズ時の処理
        /// </summary>
        /// <param name="ispause">true:一時停止, false:解除</param>
        private void PauseMove(bool ispause)
        {
            if (ispause)
            {
                //一時停止時は弾丸の速度を0にする
                m_tmpVelocity = m_rb.velocity;
                m_rb.velocity = Vector3.zero;
            }
            else
            {
                //解除時は元に戻す
                m_rb.velocity = m_tmpVelocity;
                m_tmpVelocity = Vector3.zero;
            }
        }
    }
}


