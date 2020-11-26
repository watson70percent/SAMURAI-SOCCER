using UnityEngine;
using UnityEngine.SceneManagement;

public class Panda : MonoBehaviour
{
    private float m_expandAmount = 400.0f;
    [SerializeField] private float speed;
    bool hit;
    GameManager gameManager;
    [SerializeField] GameState state = GameState.Reset;
    Animator anim;
    [SerializeField] SkinnedMeshRenderer skin;

    void SwitchState(StateChangedArg a)
    {
        state = a.gameState;
    }

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.StateChange += SwitchState;
        state = gameManager.CurrentGameState;
        anim = GetComponent<Animator>();
        

        
    }

    private void Update()
    {
        switch (state)
        {
            case GameState.Playing:
                var pos = transform.position;
                pos.y -= speed * Time.deltaTime;
                transform.position = pos;
                if (pos.y < -50) { Destroy(gameObject); }
                anim.speed = 1;
                break;
            default: anim.speed = 0; break;
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !hit)
        {
            hit = true;
            //GameObject.Find("YellowCard").GetComponent<Penalty>().YellowCard();
            GameOver();
        }
    }


    public void GameOver()
    {

        SceneManager.sceneLoaded += GameSceneLoaded;
        gameManager.StateChangeSignal(GameState.Finish);


    }

    void GameSceneLoaded(Scene next, LoadSceneMode mode)
    {
        ResultManager resultManager = GameObject.Find("ResultManager").GetComponent<ResultManager>();
        resultManager.SetResult(Result.Lose, "パンダに潰されてしもた!");

        SceneManager.sceneLoaded -= GameSceneLoaded;
    }

}