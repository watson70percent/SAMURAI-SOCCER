using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using SamuraiSoccer.StageContents.Conversation;

namespace SamuraiSoccer.StageContents.Result
{
    public class StageDataSave : MonoBehaviour
    {
        [SerializeField]
        private ConversationManager m_conversationManager;

        private void Start()
        {
            Save(this.GetCancellationTokenOnDestroy()).Forget();
        }

        async UniTask Save(CancellationToken cancellation_token)
        {
            GameResult result;
            while ((result = GetComponent<ResultManager>().ResultState) == GameResult.Undefined)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, cancellation_token);
            }

            if (result == GameResult.Win)
            {
                InMemoryDataTransitClient<int> stageNumberTransitionClient = new InMemoryDataTransitClient<int>();
                int stageNumber = stageNumberTransitionClient.Get(StorageKey.KEY_STAGENUMBER);
                // ステージ情報を保存
                SaveData saveData = new SaveData();
                saveData.m_stageData = stageNumber;
                new InFileTransmitClient<SaveData>().Set(StorageKey.KEY_STAGENUMBER, saveData);
                // ステージ番号に対応したお話を開始
                // お話の番号=クリアしたステージ番号を3で割った商×4+ステージ番号を3で割った余り+1
                await m_conversationManager.PlayConversation((stageNumber / 3) * 4 + stageNumber % 3 + 1);
            }
        }
    }
}

