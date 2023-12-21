using System.Collections;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using SamuraiSoccer;
using SamuraiSoccer.Event;
using SamuraiSoccer.StageContents;
using UnityEngine.SceneManagement;

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
        private int m_damageSEIndex; //衝突時のSE

        [SerializeField]
        private string m_resultSceneName = "Result";

        private bool m_isActive = true; //true:稼働状態, false:衝突判定停止

        // Start is called before the first frame update
        void Start()
        {
            //ゴール時に機能停止
            InGameEvent.Goal.Subscribe(_ =>
            {
                m_isActive = false;
            }).AddTo(this);
            //衝突時の処理
            this.OnTriggerEnterAsObservable().Subscribe(async other =>
            {
                //プレイヤ―との衝突処理
                if (m_statueMove.CurrentStatueMode == StatueMode.FallDown && m_isActive)
                {
                    if (other.gameObject.tag == "Player")
                    {
                        InMemoryDataTransitClient<GameResult> gameresultDataTransitClient = new InMemoryDataTransitClient<GameResult>();
                        gameresultDataTransitClient.Set(StorageKey.KEY_WINORLOSE, GameResult.Lose);
                        //衝突音の再生
                        SoundMaster.Instance.PlaySE(m_damageSEIndex);
                        InGameEvent.FinishOnNext();
                        Time.timeScale = 0.2f;
                        await UniTask.Delay(1000);
                        Time.timeScale = 1f;
                        SceneManager.LoadScene(m_resultSceneName);
                    }
                }
                //ボールとの衝突処理
                if (other.gameObject.tag == "Ball")
                {
                    await ResetBall(other.gameObject);
                }
            });
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
                Ball.transform.position = new Vector3(Ball.transform.position.x, 10f, Ball.transform.position.z);
            }
        }
    }
}

