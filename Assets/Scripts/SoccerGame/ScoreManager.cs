using SamuraiSoccer.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiSoccer.SoccerGame
{
    /// <summary>
    /// ���_�����ۂ̏������Ǘ�����N���X�B
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField]
        private List<int> m_CutSceneScoreTeammate;
        [SerializeField]
        private List<int> m_CutSceneScoreOpponent;

        private int m_teammateScore = 0;
        private int m_opponentScore = 0;

        /// <summary>
        /// �S�[�����ɌĂԁB
        /// </summary>
        /// <param name="isTeammate">
        /// ���`�[���̓��_���B
        /// </param>
        public void Goal(bool isTeammate)
        {
            if (isTeammate)
            {
                m_teammateScore++;
                InGameEvent.TeammateScoreOnNext(m_teammateScore);
            }
            else
            {
                m_opponentScore++;
                InGameEvent.OpponentScoreOnNext(m_opponentScore);
            }

            var goalEventType = (isTeammate, m_CutSceneScoreTeammate.Contains(m_teammateScore), m_CutSceneScoreOpponent.Contains(m_opponentScore)) switch
            {
                (true, true, _) => GoalEventType.CutSceneTeammateGoal,
                (false, _, true) => GoalEventType.CutSceneOpponentGoal,
                (true, false, _) => GoalEventType.NormalTeammateGoal,
                (false, _, false) => GoalEventType.NormalOpponentGoal,
            };
            InGameEvent.GoalOnNext(goalEventType);
        }
    }
}
