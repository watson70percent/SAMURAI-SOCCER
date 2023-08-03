using UnityEngine;
using UnityEngine.SceneManagement;
using SamuraiSoccer.Event;
using UniRx;
using SamuraiSoccer.StageContents.Result;
using SamuraiSoccer.StageContents;
using SamuraiSoccer;
using UnityEditor;

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
        private SceneAsset m_resultScene;

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
            //プレイヤーとぶつかったらゲームオーバー
            if (other.tag == "Player" && !m_hit && m_state.Value == State.Active)
            {
                m_hit = true;
                Invoke("GameOver", 0.2f);
            }
        }


        public void GameOver()
        {
            InGameEvent.FinishOnNext();
            SoundBoxUtil.SetSoundBox(transform.position, m_hitSound);

            InMemoryDataTransitClient<GameResult> inMemoryDataTransitClient = new InMemoryDataTransitClient<GameResult>();
            inMemoryDataTransitClient.Set(StorageKey.KEY_WINORLOSE, GameResult.Lose);
            Instantiate(m_blood, m_player.transform.position + Vector3.up * 0.1f, Quaternion.identity);
            Instantiate(m_gameOverPanel);
            SceneManager.LoadScene(m_resultScene.name);
        }


    }
}