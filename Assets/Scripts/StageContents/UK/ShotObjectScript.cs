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

        [SerializeField] float velocity = 30;//����
        float movedLength;//����������
        float groundWidth = 120;//�O���E���h�̕�
        bool isEnd, isActive = true;
        [SerializeField] AudioSource soundEffect;
        //[SerializeField] GameObject player;
        Vector3 rotateVec {get{return new Vector3(4, 7, 5);}}
        float velocity0 = 300;

        // Start is called before the first frame update
        void Start()
        {
            //�ړ������̏�����
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
            //���X�s�[�h�œ�����
            gameObject.transform.position += transform.forward * Time.deltaTime * velocity;
            movedLength += Time.deltaTime * velocity;
            //�O���E���h��ʂ�߂��������
            if ((movedLength) > (groundWidth + 2) && !isEnd)//�Ԃ�������ɂ͏����Ȃ��悤��
            {
                Destroy(this.gameObject);
            }
        }

        //�X���[���o����̃V�[���J��
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