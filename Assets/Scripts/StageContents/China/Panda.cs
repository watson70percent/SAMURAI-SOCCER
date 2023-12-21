using UnityEngine;
using UnityEngine.SceneManagement;
using SamuraiSoccer.Event;
using UniRx;
using UnityEditor;
using Cysharp.Threading.Tasks;

namespace SamuraiSoccer.StageContents.China
{
    public class Panda : MonoBehaviour
    {
        private float m_expandAmount = 400.0f;
        [SerializeField]
        private float m_speed;
        private bool m_hit;
        private Animator m_anim;
        [SerializeField]
        private GameObject m_gameOverPanel;
        [SerializeField]
        private AudioClip m_hitSound;
        [SerializeField]
        private GameObject m_blood;
        private GameObject m_player;

        [SerializeField]
        private string m_resultSceneName;

        enum State
        {
            Active,
            Non_Interference,
            Stop,
            Vanish
        }



        private IReadOnlyReactiveProperty<State> m_state =
            Observable.CombineLatest(
                    Observable.Merge( //ストリームの合流
                       InGameEvent.Play.Select(_ => State.Active),
                       InGameEvent.Standby.Select(_ => State.Vanish),
                       InGameEvent.Goal.Select(_ => State.Non_Interference)
                    ).StartWith(State.Active),//パンダは途中から場に出るので最初にState.Activeを勝手に発行してしまう
                    InGameEvent.Pause.StartWith(false), //Pauseイベントとも合流
                    (state, isPause) => isPause ? State.Stop : state //PauseがtrueのときはState.Stop
            ).ToReactiveProperty<State>(State.Stop);
            
        private void Start()
        {

            m_anim = GetComponent<Animator>();

            m_player = GameObject.FindGameObjectWithTag("Player");


        }

        private void Update()
        {
            print(m_state.Value);
            switch (m_state.Value)
            {
                case State.Active:
                    var pos = transform.position;
                    pos.y -= m_speed * Time.deltaTime;
                    transform.position = pos;
                    if (pos.y < -50) { Destroy(gameObject); }
                    m_anim.speed = 1;
                    break;
                case State.Vanish:
                    Destroy(gameObject);
                    break;
                case State.Stop: //ポーズ時は静止
                    m_anim.speed = 0; break;
                case State.Non_Interference:
                    m_anim.speed = 1;
                    break;
            }


        }

        private void OnTriggerEnter(Collider other)
        {
            //すでにゲームが終了しているときは何もせずにreturn
            var client = new InMemoryDataTransitClient<GameResult>();
            if (client.TryGet(StorageKey.KEY_WINORLOSE, out var outvalue))
            {
                client.Set(StorageKey.KEY_WINORLOSE, outvalue);
                return;
            }
            //プレイヤーとぶつかったらゲームオーバー
            if (other.tag == "Player" && !m_hit && m_state.Value == State.Active)
            {
                m_hit = true;
                GameOver().Forget();
            }
        }


        private async UniTask GameOver()
        {
            InGameEvent.FinishOnNext();
            SoundBoxUtil.SetSoundBox(transform.position, m_hitSound);
            Time.timeScale = 0.3f;
            InMemoryDataTransitClient<GameResult> inMemoryDataTransitClient = new InMemoryDataTransitClient<GameResult>();
            inMemoryDataTransitClient.Set(StorageKey.KEY_WINORLOSE, GameResult.Lose);
            Instantiate(m_blood, m_player.transform.position + Vector3.up * 0.2f, Quaternion.identity);
            Instantiate(m_gameOverPanel);
            await UniTask.Delay(1000);
            Time.timeScale = 1.0f;
            SceneManager.LoadScene(m_resultSceneName);
        }


    }
}