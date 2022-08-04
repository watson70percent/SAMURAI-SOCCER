using UnityEngine;
using UnityEngine.SceneManagement;
using SamuraiSoccer.Event;
using UniRx;
using SamuraiSoccer.StageContents.Result;
using SamuraiSoccer.StageContents;

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
        private SkinnedMeshRenderer m_skin;
        [SerializeField]
        private GameObject m_gameOverPanel;
        [SerializeField]
        private AudioClip m_hitSound;
        [SerializeField]
        private GameObject m_blood;
        [SerializeField]
        private GameObject m_player;

        enum State
        {
            Active,
            Non_Interference,
            Stop,
            Vanish
        }

        State m_state = State.Stop;
        State m_tempState = State.Stop;
        private void Start()
        {

            m_anim = GetComponent<Animator>();

            m_player = GameObject.FindGameObjectWithTag("Player");

            InGameEvent.Pause.Subscribe(isPause => {
                if (isPause)
                {
                    m_tempState = m_state;
                    m_state = State.Stop;
                }
                else
                {
                    m_state = m_tempState;
                }
            });
            InGameEvent.Play.Subscribe(x => { m_state = State.Active; });
            InGameEvent.Standby.Subscribe(x => { m_state = State.Vanish; });
            InGameEvent.Goal.Subscribe(x => { m_state = State.Non_Interference; });

        }

        private void Update()
        {
            switch (m_state)
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

            }


        }

        private void OnTriggerEnter(Collider other)
        {
            //プレイヤーとぶつかったらゲームオーバー
            if (other.tag == "Player" && !m_hit && m_state == State.Active)
            {
                m_hit = true;
                Invoke("GameOver", 0.2f);
            }
        }


        public void GameOver()
        {
            SceneManager.sceneLoaded += GameSceneLoaded;
            InGameEvent.FinishOnNext();
            SoundBoxUtil.SetSoundBox(transform.position, m_hitSound);

            Instantiate(m_blood, m_player.transform.position + Vector3.up * 0.1f, Quaternion.identity);
            Instantiate(m_gameOverPanel);

        }

        void GameSceneLoaded(Scene next, LoadSceneMode mode)
        {
            ResultManager resultManager = GameObject.Find("ResultManager").GetComponent<ResultManager>();
            resultManager.SetResult(GameResult.Lose, "パンダに潰されてしもた!");

            SceneManager.sceneLoaded -= GameSceneLoaded;
        }

    }
}