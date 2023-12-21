using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;
using SamuraiSoccer.Event;

namespace SamuraiSoccer.UI
{
    public class RemainGoalPoint : MonoBehaviour
    {
        [SerializeField]
        private Text m_remainText;

        [SerializeField]
        private Text m_remainScoreText;

        [SerializeField]
        private int m_GameClearScore;

        private void Start()
        {
            InGameEvent.TeammateScore.Where(score => score == 1).Subscribe(_ => m_remainText.text = "Žc‚è“_”").AddTo(this);
            InGameEvent.TeammateScore.Subscribe(score => m_remainScoreText.text = (m_GameClearScore - score).ToString()).AddTo(this);
        }
    }
}
