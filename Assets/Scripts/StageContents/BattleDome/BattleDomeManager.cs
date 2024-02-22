using UnityEngine;
using SamuraiSoccer.Event;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine.SceneManagement;
using SamuraiSoccer.StageContents.Conversation;

namespace SamuraiSoccer.StageContents.BattlerDome
{
    public class BattleDomeManager : MonoBehaviour
    {
        public AudioSource audioSource;
        public AudioClip goalSound;
        public AudioClip startSound;
        public ConversationManager conversationManager;
        public string resultSceneName;
        int clearNum = 5;
        int scenarioNum = 0;
        

        // Start is called before the first frame update
        private void Start()
        {
            InGameEvent.TeammateScore.Subscribe(score => this.scenarioNum = score).AddTo(this);
            InGameEvent.Goal.Where(t =>t== GoalEventType.CutSceneOpponentGoal || t == GoalEventType.CutSceneTeammateGoal).Subscribe(async t => await GoalAction(t)).AddTo(this);
        }

        private void Clear()
        {
            InGameEvent.FinishOnNext();
            var win = new InMemoryDataTransitClient<GameResult>();
            win.Set(StorageKey.KEY_WINORLOSE, GameResult.Win);
            _ = SlowToWin();
        }

        /// <summary>
        /// Ÿ‚Á‚½‚ÉƒV[ƒ“‘JˆÚ‚Ü‚Å‚ä‚Á‚­‚è‚É‚µ‚Ä‘JˆÚ‚³‚¹‚é
        /// </summary>
        /// <returns></returns>
        private async UniTask SlowToWin()
        {
            Time.timeScale = 0.3f;
            await SoundMaster.Instance.PlaySE(11);
            Time.timeScale = 1;
            SceneManager.LoadScene(resultSceneName);
        }

        private async UniTask GoalAction(GoalEventType t)
        {
            if (scenarioNum >= clearNum)
            {
                Clear();
                return;
            }
            audioSource.PlayOneShot(goalSound);
            await Conversation(scenarioNum);
            InGameEvent.GoalOnNext(GoalEventType.NormalTeammateGoal);        
        }

        private async UniTask Conversation(int num)
        {
            int convNum = -1;
            switch (num)
            {
                case 1: 
                    convNum = 30; 
                    break;
                case 4: 
                    convNum = 31; 
                    break;
                default: break;
            }
            
            await conversationManager.PlayConversation(conversationNum:convNum, () => { });
        }
    }
}
