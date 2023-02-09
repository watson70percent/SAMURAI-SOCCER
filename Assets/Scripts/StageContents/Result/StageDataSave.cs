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
            InMemoryDataTransitClient<int> stageNumberTransitionClient = new InMemoryDataTransitClient<int>();
            int clearNumber = stageNumberTransitionClient.Get(StorageKey.KEY_STAGENUMBER);
            InFileTransmitClient<SaveData> fileTransitClient = new InFileTransmitClient<SaveData>();
            int savedNumber;
            if (fileTransitClient.TryGet(StorageKey.KEY_STAGENUMBER, out var save))
            {
                savedNumber = save.m_stageData;
            }
            else
            {
                savedNumber = 0;
                SaveData data = new SaveData();
                data.m_stageData = clearNumber;
                fileTransitClient.Set(StorageKey.KEY_STAGENUMBER, data);
            }
            if (result == GameResult.Win && clearNumber >= savedNumber)
            {
                // ステージ情報を保存
                SaveData saveData = new SaveData();
                saveData.m_stageData = clearNumber+1;
                new InFileTransmitClient<SaveData>().Set(StorageKey.KEY_STAGENUMBER, saveData);
                // ステージ番号に対応したお話を開始
                // お話の番号=クリアしたステージ番号を3で割った商×4+ステージ番号を3で割った余り+1
                await m_conversationManager.PlayConversation((clearNumber / 3) * 4 + clearNumber % 3 + 1);
            }
        }
    }
}

