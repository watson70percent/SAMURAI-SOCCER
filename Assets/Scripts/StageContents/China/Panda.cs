using UnityEngine;
using UnityEngine.SceneManagement;
using SamuraiSoccer.Event;
using UniRx;
using SamuraiSoccer;

namespace SamuraiSoccer.StageContents.China
{
    public class Panda : MonoBehaviour
    {

        enum State
        {
            Moving, Stop
        }

        private float m_expandAmount = 400.0f;
        [SerializeField] private float speed;
        bool hit;

        State state = State.Moving;
        Animator anim;
        [SerializeField] SkinnedMeshRenderer skin;
        public GameObject gameOverPanel;
        public AudioClip hitSound;
        public GameObject blood;
        GameObject player;



        private void Start()
        {
            InGameEvent.Pause.Subscribe(isPause => { state = isPause ? State.Stop : State.Moving; });
            InGameEvent.Standby.Subscribe(x => { Destroy(gameObject); });

            anim = GetComponent<Animator>();

            player = GameObject.FindGameObjectWithTag("Player");

        }

        private void Update()
        {
            switch (state)
            {
                case State.Moving:
                    var pos = transform.position;
                    pos.y -= speed * Time.deltaTime;
                    transform.position = pos;
                    if (pos.y < -50) { Destroy(gameObject); }
                    anim.speed = 1;
                    break;
                case State.Stop:
                    default: anim.speed = 0; break;
            }


        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player" && !hit)
            {
                hit = true;
                Invoke("GameOver", 0.2f);
            }
        }


        public void GameOver()
        {

            InGameEvent.FinishOnNext();
            SoundBoxUtil.SetSoundBox(transform.position, hitSound);

            Instantiate(blood, player.transform.position + Vector3.up * 0.1f, Quaternion.identity);
            Instantiate(gameOverPanel);

        }

        void GameSceneLoaded(Scene next, LoadSceneMode mode)
        {
            ResultManager resultManager = GameObject.Find("ResultManager").GetComponent<ResultManager>();
            resultManager.SetResult(Result.Lose, "パンダに潰されてしもた!");

            SceneManager.sceneLoaded -= GameSceneLoaded;
        }

    }
}
