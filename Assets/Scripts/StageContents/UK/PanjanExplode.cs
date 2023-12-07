using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SamuraiSoccer.Event;
using SamuraiSoccer.StageContents;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;

namespace SamuraiSoccer.UK
{
    public class PanjanExplode : MonoBehaviour
    {
        bool isBurn = true;
        PanjanMake panjanMake;
        GameObject fire;

        [SerializeField]
        private string m_resultSceneName = "Result";

        private int m_calledNum = 0; // ŒÄ‚Ño‚³‚ê‚½‰ñ”‚ğ‹L‰¯

        // Start is called before the first frame update
        void Start()
        {
            InGameEvent.Goal.Subscribe(_ =>
            {
                isBurn = false;
            }).AddTo(this);
            this.OnCollisionEnterAsObservable()
            .Select(hit => hit.gameObject.tag)
            .Where(tag => tag == "Player")
            .Subscribe(async _ =>
            {
                if (isBurn)
                {
                    // •¡”‰ñŒÄ‚Ño‚³‚ê‚é–â‘è‚ğ‰ğŒˆ
                    if (System.Threading.Interlocked.Increment(ref m_calledNum) != 1) return;
                    InMemoryDataTransitClient<GameResult> gameresultDataTransitClient = new InMemoryDataTransitClient<GameResult>();
                    gameresultDataTransitClient.Set(StorageKey.KEY_WINORLOSE, GameResult.Lose);
                    fire.SetActive(true);
                    InGameEvent.FinishOnNext();
                    Time.timeScale = 0.2f;
                    await UniTask.Delay(700);
                    Time.timeScale = 1f;
                    SceneManager.LoadScene(m_resultSceneName);
                }
            }).AddTo(this);
        }

        public void SetFireObject(GameObject fire)
        {
            this.fire = fire;
        }
    }
}