using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SamuraiSoccer.Event;
using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine.SceneManagement;
using SamuraiSoccer;
using UnityEngine;
using SamuraiSoccer.StageContents.Conversation;

namespace SamuraiSoccer.StageContents.BattlerDome
{


    public class BattleDomeManager : MonoBehaviour
    {
        public AudioSource audioSource;
        public AudioClip goalSound;
        public AudioClip startSound;
        public ConversationManager conversationManager;
        int clearPoint = 3;
        int score = 0;
        

        // Start is called before the first frame update
        void Start()
        {
            InGameEvent.TeammateScore.Subscribe(score => this.score = score).AddTo(this);
            InGameEvent.Goal.Where(t => t == GoalEventType.CutSceneOpponentGoal || t == GoalEventType.CutSceneTeammateGoal).Subscribe(async t => await GoalAction(t)).AddTo(this);
        }

        // Update is called once per frame
        void Update()
        {
        
        }



        void Clear()
        {
            InGameEvent.FinishOnNext();
            var win = new InMemoryDataTransitClient<GameResult>();
            win.Set(StorageKey.KEY_WINORLOSE, GameResult.Win);
            SceneManager.LoadScene("Result");
        }

        private async UniTask GoalAction(GoalEventType t)
        {
            if (score >= clearPoint)
            {
                Clear();
                return;
            }

            audioSource.PlayOneShot(goalSound);
            
            
            await Conversation(score);
            
            await UniTask.Delay(4000);
            UIEffectEvent.BlackOutOnNext(5f);
            InGameEvent.StandbyOnNext(t == GoalEventType.NormalOpponentGoal);
            await UniTask.Delay(3000);
            audioSource.PlayOneShot(startSound);
            InGameEvent.PlayOnNext();
            
        }

        private async UniTask Conversation(int score)
        {
            int convNum = -1;
            switch (score)
            {
                case 1: convNum = 30; break;
                case 2: convNum = 31;break;
                default: break;
            }
            
            await conversationManager.PlayConversation(conversationNum:convNum);
        }
    }
}
