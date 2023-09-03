using SamuraiSoccer.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiSoccer.SoccerGame
{
    /// <summary>
    /// 得点した際の処理を管理するクラス。
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        private int m_teammateScore = 0;
        private int m_opponentScore = 0;

        private bool isLastStage = false;

        private void Start()
        {
            var client = new InMemoryDataTransitClient<int>();
            var stageNumber = client.Get(StorageKey.KEY_STAGENUMBER);
            client.Set(StorageKey.KEY_STAGENUMBER, stageNumber);
            isLastStage = stageNumber == 12;
        }

        /// <summary>
        /// ゴール時に呼ぶ。
        /// </summary>
        /// <param name="isTeammate">
        /// 自チームの得点か。
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

            if (isLastStage)
            {
                LastStage(isTeammate);
            }
            else
            {
                NormalStage(isTeammate);
            }
        }

        private void NormalStage(bool isTeammate)
        {
            if (isTeammate)
            {
                InGameEvent.GoalOnNext(GoalEventType.NormalTeammateGoal);
            }
            else
            {
                InGameEvent.GoalOnNext(GoalEventType.NormalOpponentGoal);
            }
        }

        private void LastStage(bool isTeammate)
        {
            var goalEventType = (isTeammate, m_teammateScore == 1, m_opponentScore == 1) switch
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
