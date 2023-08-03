using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using SamuraiSoccer.Event;
using SamuraiSoccer.StageContents;


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
        private SceneAsset m_resultScene;


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

        public void YellowCard(int penaltynumber)
        {
            m_yellowCard[penaltynumber].SetActive(true);
            if (penaltynumber == 1)
            {
                SoundMaster.Instance.PlaySE(m_redcardSENumber);
                InMemoryDataTransitClient<GameResult> lose = new InMemoryDataTransitClient<GameResult>();
                lose.Set(StorageKey.KEY_WINORLOSE, GameResult.Lose);
                InMemoryDataTransitClient<string> message = new InMemoryDataTransitClient<string>();
                message.Set(StorageKey.KEY_RESULTMESSAGE, "反則負け！");
                InGameEvent.FinishOnNext();
                Instantiate(m_gameOverPanel);
                SceneManager.LoadScene(m_resultScene.name);
            }
            else
            {
                SoundMaster.Instance.PlaySE(m_yellowcardSENumber);
            }
        }
    }
}
