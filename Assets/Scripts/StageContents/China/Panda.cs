using UnityEngine;
using UnityEngine.SceneManagement;
using SamuraiSoccer.Event;
using UniRx;

public class Panda : MonoBehaviour
{
    private float m_expandAmount = 400.0f;
    [SerializeField] private float speed;
    bool hit;

    Animator anim;
    [SerializeField] SkinnedMeshRenderer skin;
    public GameObject gameOverPanel;
    public AudioClip hitSound;
    public GameObject blood;
    GameObject player;

    enum State
    {
        Active,
        Non_Interference,
        Stop,
        Vanish
    }

    State state = State.Stop;
    State tempState = State.Stop;
    private void Start()
    {

        anim = GetComponent<Animator>();

        player = GameObject.FindGameObjectWithTag("Player");

        InGameEvent.Pause.Subscribe( isPause => {
            if (isPause)
            {
                tempState = state;
                state = State.Stop;
            }
            else
            {
                state = tempState;
            }
        });
        InGameEvent.Play.Subscribe(x => { state = State.Active; });
        InGameEvent.Standby.Subscribe(x => { state = State.Vanish; });
        InGameEvent.Goal.Subscribe(x => { state = State.Non_Interference; });

    }

    private void Update()
    {
        switch (state)
        {

            case State.Active:
                var pos = transform.position;
                pos.y -= speed * Time.deltaTime;
                transform.position = pos;
                if (pos.y < -50) { Destroy(gameObject); }
                anim.speed = 1;
                break;
            case State.Vanish:
                Destroy(gameObject);
                break;
            case State.Stop:
                anim.speed = 0; break;

        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !hit && state==State.Active)
        {
            hit = true;
            Invoke("GameOver", 0.2f);
        }
    }


    public void GameOver()
    {

        SceneManager.sceneLoaded += GameSceneLoaded;
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