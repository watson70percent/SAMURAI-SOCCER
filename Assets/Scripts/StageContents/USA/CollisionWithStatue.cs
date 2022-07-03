using System.Collections;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using SamuraiSoccer.Event;

namespace SamuraiSoccer.StageContents.USA
{
    /// <summary>
    /// 自由の女神との衝突の処理
    /// </summary>
    public class CollisionWithStatue : MonoBehaviour
    {
        [SerializeField]
        private StatueMove m_statueMove; //親オブジェクトについているStatueMove

        [SerializeField]
        private int m_soundIndex; //衝突時のSE

        private bool m_isActive = true; //true:稼働状態, false:衝突判定停止

        public string ResultSceneName = "Result"; //リザルトシーン名

        // Start is called before the first frame update
        void Start()
        {
            //ゴール時に機能停止
            InGameEvent.Goal.Subscribe(_ =>
            {
                m_isActive = false;
            }).AddTo(this);
        }

        /// <summary>
        /// 自由の女神の衝突判定部とオブジェクトとの衝突処理
        /// </summary>
        /// <param name="other"></param>
        private async void OnTriggerEnter(Collider other)
        {
            //プレイヤ―との衝突処理
            if (m_statueMove.CurrentStatueMode == StatueMode.FallDown && m_isActive)
            {
                if (other.gameObject.tag == "Player")
                {
                    //衝突音の再生
                    SoundMaster.Instance.PlaySE(m_soundIndex);
                }
            }
            //ボールとの衝突処理
            if (other.gameObject.tag == "Ball")
            {
               await ResetBall(other.gameObject);
            }
        }

        /// <summary>
        /// ボールが押しつぶされて貫通した場合にコート内に戻す
        /// </summary>
        /// <param name="Ball"></param>
        /// <returns></returns>
        async UniTask ResetBall(GameObject Ball)
        {
            await UniTask.Delay(1000);
            if (Ball.transform.position.y < 0)
            {
                Ball.transform.position = new Vector3(Ball.transform.position.x, 5f, Ball.transform.position.z);
            }
        }
    }
}

