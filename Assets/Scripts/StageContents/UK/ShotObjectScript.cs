using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using SamuraiSoccer;
using SamuraiSoccer.Event;
using SamuraiSoccer.StageContents;

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
            InMemoryDataTransitClient<GameResult> lose = new InMemoryDataTransitClient<GameResult>();
            lose.Set(StorageKey.KEY_WINORLOSE, GameResult.Lose);
            InGameEvent.FinishOnNext();
            float ang = 20 * Mathf.Deg2Rad;
            float elapsedTime=0;
            while (elapsedTime<1)
            {
                Vector3 pos = player.transform.position;
                pos.x -= velocity0 * Mathf.Cos(ang) * Time.deltaTime;
                pos.y += velocity0 * Mathf.Sin(ang) * Time.deltaTime;
                player.transform.position = pos;
                player.transform.Rotate(rotateVec * velocity * velocity * Time.deltaTime);
                elapsedTime+=Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update,cancellationToken);
            }
            Time.timeScale = 1f;
            SceneManager.LoadScene("Result");
        }
    }
}