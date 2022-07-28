using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using UniRx.Triggers;
using SamuraiSoccer.Event;

namespace SamuraiSoccer.StageContents.UK
{
    public class ShotObjectScript : MonoBehaviour
    {

        [SerializeField] float velocity = 30;//速さ
        float movedLength;//動いた距離
        float groundWidth = 120;//グラウンドの幅
        bool isEnd, _isActive = true;
        [SerializeField] AudioSource audio;
        [SerializeField] GameObject player;
        [SerializeField] 

        // Start is called before the first frame update
        void Start()
        {
            //移動距離の初期化
            movedLength = 0;
            InGameEvent.Standby.Subscribe(_ =>
            {
                Destroy(this.gameObject);
            }).AddTo(this);
            InGameEvent.UpdateDuringPlay.Subscribe(_ =>
            {
                Move();
            }).AddTo(this);
            this.OnTriggerEnterAsObservable().Where(x => x.gameObject.tag == "Player")
            .Subscribe(async _ => await BlowAway(this.GetCancellationTokenOnDestroy())).AddTo(this);
        }

        // Update is called once per frame
        void Move()
        {
            //一定スピードで動かす
            gameObject.transform.position += transform.forward * Time.deltaTime * velocity;
            movedLength += Time.deltaTime * velocity;
            //グラウンドを通り過ぎたら消す
            if ((movedLength) > (groundWidth + 2) && !isEnd)
            {
                Destroy(this.gameObject);
            }
        }

        //スロー演出からのシーン遷移


        async UniTask BlowAway(CancellationToken cancellationToken = default)
        {
            Time.timeScale = 0.2f;
            audio.Play();
            InGameEvent.FinishOnNext();
            float velocity0 = 300;
            float ang = 20 * Mathf.Deg2Rad;
            Vector3 rotateVec = new Vector3(4, 7, 5);
            for (int i = 0; i < 100; i++)
            {
                Vector3 pos = player.transform.position;
                pos.x -= velocity0 * Mathf.Cos(ang) * Time.deltaTime;
                pos.y += velocity0 * Mathf.Sin(ang) * Time.deltaTime;
                player.transform.position = pos;
                player.transform.Rotate(rotateVec * velocity * velocity * Time.deltaTime);
                await UniTask.Yield( PlayerLoopTiming.Update, cancellationToken );
            }
        }
    }
}