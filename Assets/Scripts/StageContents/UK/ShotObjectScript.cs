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
        bool isEnd, isActive = true;
        [SerializeField] AudioSource soundEffect;
        //[SerializeField] GameObject player;
        Vector3 rotateVec {get{return new Vector3(4, 7, 5);}}
        float velocity0 = 300;

        // Start is called before the first frame update
        void Start()
        {
            //移動距離の初期化
            movedLength = 0;
            InGameEvent.Standby.Subscribe(_ =>
            {
                Destroy(this.gameObject);
            }).AddTo(this);
            InGameEvent.Goal.Subscribe(_=>
            {
                isActive=false;
            }).AddTo(this.gameObject);
            InGameEvent.Finish.Subscribe(_ =>
            {
                isActive = false;
            });
            InGameEvent.UpdateDuringPlay.Subscribe(_ =>
            {
                Move();
            }).AddTo(this);
            this.OnTriggerEnterAsObservable().Where(x => x.gameObject.tag == "Player")
            .Subscribe(async _ => {
                if (isActive) await BlowAway(_.gameObject,this.GetCancellationTokenOnDestroy());
                }).AddTo(this);
        }

        // Update is called once per frame
        void Move()
        {
            //一定スピードで動かす
            gameObject.transform.position += transform.forward * Time.deltaTime * velocity;
            movedLength += Time.deltaTime * velocity;
            //グラウンドを通り過ぎたら消す
            if ((movedLength) > (groundWidth + 2) && !isEnd)//ぶつかった後には消えないように
            {
                Destroy(this.gameObject);
            }
        }

        //スロー演出からのシーン遷移
        async UniTask BlowAway(GameObject player,CancellationToken cancellationToken = default)
        {
            isEnd=true;
            Time.timeScale = 0.2f;
            soundEffect.Play();
            InGameEvent.FinishOnNext();
            float ang = 20 * Mathf.Deg2Rad;
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