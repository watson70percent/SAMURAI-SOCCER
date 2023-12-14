using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using SamuraiSoccer.Event;
using SamuraiSoccer.StageContents;
using Cysharp.Threading.Tasks;

namespace SamuraiSoccer.SoccerGame
{
    public class Penalty : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] m_yellowCard = new GameObject[2];

        [SerializeField]
        private GameObject m_gameOverPanel;

        
        private int m_yellowcardSENumber =12;

        
        private int m_redcardSENumber =12;

        [SerializeField]
        private string m_resultSceneName = "Result";


        void Start()
        {
            InGameEvent.Reset.Subscribe(OnReset).AddTo(this);
            InGameEvent.Penalty.Subscribe(YellowCard).AddTo(this);
        }

        private void OnReset(Unit _)
        {
            m_yellowCard[0].SetActive(false);
            m_yellowCard[1].SetActive(false);
        }

        private void YellowCard(int penaltynumber)
        {
            m_yellowCard[penaltynumber].SetActive(true);
            if (penaltynumber == 1)
            {
                var client = new InMemoryDataTransitClient<GameResult>();
                if (client.TryGet(StorageKey.KEY_WINORLOSE, out var outvalue))
                {
                    client.Set(StorageKey.KEY_WINORLOSE, outvalue);
                    return;
                }
                LoseEffect().Forget();
            }
            else
            {
                SoundMaster.Instance.PlaySE(m_yellowcardSENumber).Forget();
            }
        }

        private async UniTask LoseEffect()
        {
            SoundMaster.Instance.PlaySE(m_redcardSENumber).Forget();
            Time.timeScale = 0.3f;
            InMemoryDataTransitClient<GameResult> lose = new InMemoryDataTransitClient<GameResult>();
            lose.Set(StorageKey.KEY_WINORLOSE, GameResult.Lose);
            InMemoryDataTransitClient<string> message = new InMemoryDataTransitClient<string>();
            message.Set(StorageKey.KEY_RESULTMESSAGE, "反則負け！");
            InGameEvent.FinishOnNext();
            Instantiate(m_gameOverPanel);
            await UniTask.Delay(1000);
            Time.timeScale = 1.0f;
            SceneManager.LoadScene(m_resultSceneName);
        }
    }
}
