using SamuraiSoccer.Event;
using SamuraiSoccer.StageContents;
using UniRx;
using UnityEngine;

namespace SamuraiSoccer.SoccerGame
{
    public class TutorialPenalty : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] m_yellowCard = new GameObject[2];

        private int m_yellowcardSENumber = 12;

        private int m_redcardSENumber = 12;

        private void Start()
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
                message.Set(StorageKey.KEY_RESULTMESSAGE, "îΩë•ïâÇØÅI");
                InGameEvent.FinishOnNext();
            }
            else
            {
                SoundMaster.Instance.PlaySE(m_yellowcardSENumber);
            }
        }
    }
}
