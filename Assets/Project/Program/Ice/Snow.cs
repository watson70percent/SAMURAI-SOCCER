using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Snow : MonoBehaviour
{
    public Transform mainCamera;
    public Transform snow;
    public Transform samurai;
    public slidepad pad;
    public GameManager gm;
    public ParticleSystem particle;
    public Image gameover;
    public AudioSource audioSource;
    [SerializeField]
    private float damage = 0;
    private Vector3 beforePoint = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        var emission = particle.emission;
        emission.rateOverTimeMultiplier = 4000;
        gm.StateChange += OnStateCanged;
        beforePoint = samurai.position;
        audioSource.volume = 0.05f;
    }
    
    private void OnStateCanged(StateChangedArg e)
    {
        var shape = particle.shape;
        var emission = particle.emission;
        switch (e.gameState)
        {
            case GameState.Playing :
                shape.scale = new Vector3(30, 50, 1);
                emission.rateOverTimeMultiplier = 800 / 3.0f * Mathf.Exp(damage);
                particle.Play(true);
                break;
            case GameState.Pause:
                particle.Stop(true);
                break;
            case GameState.Reset:
                snow.position = new Vector3(30, 0, 59.6f);
                shape.scale = new Vector3(150, 100, 1);
                emission.rateOverTimeMultiplier = 4000;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        var pos = snow.position;
        pos.z = mainCamera.position.z;
        snow.position = pos;

        if (gm.CurrentGameState == GameState.Playing)
        {
            var diff = samurai.position - beforePoint;
            if(diff.sqrMagnitude < 5 * Time.deltaTime * Time.deltaTime)
            {
                damage += 4.0f / 30 * Time.deltaTime;
                if(damage > 4.8)
                {
                    SceneManager.sceneLoaded += GameSceneLoaded;
                    gm.StateChangeSignal(GameState.Finish);
                    StartCoroutine(GameOver());
                }
            }
            else if(diff.sqrMagnitude > 15 * Time.deltaTime * Time.deltaTime)
            {
                if(damage > 0)
                {
                    damage -= 4.0f / 30 * Time.deltaTime;
                }
            }

            beforePoint = samurai.position;

            var emission = particle.emission;
            emission.rateOverTimeMultiplier = 800 / 3.0f * Mathf.Exp(damage);
            pad.speed = 10 - (damage < 1.6 ? 0 : (damage - 1.6f) * 2);
            var volume = damage / 4.8f;
            audioSource.volume = (volume < 0.05) ? 0.05f : volume;
        }
    }

    private void GameSceneLoaded(Scene next, LoadSceneMode mode)
    {
        ResultManager resultManager = GameObject.Find("ResultManager").GetComponent<ResultManager>();
        resultManager.SetResult(Result.Lose, "凍ってしまった!");

        SceneManager.sceneLoaded -= GameSceneLoaded;
    }

    private IEnumerator GameOver()
    {
        gameover.gameObject.SetActive(true);
        float time = 0;
        while(time < 2)
        {
            gameover.color = new Color(1, 1, 1, time / 2);
            yield return null;
            time += Time.deltaTime;
        }

        yield return new WaitForSeconds(2);
        time = 0;

        while(time < 1)
        {
            gameover.color = new Color(1 - time, 1 - time, 1 - time, 1);
            yield return null;
            time += Time.deltaTime;
        }

        SceneManager.LoadScene("Result");
    }
}
