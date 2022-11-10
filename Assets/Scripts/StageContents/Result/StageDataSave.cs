using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using SamuraiSoccer.StageContents;
using SamuraiSoccer;

namespace SamuraiSoccer.StageContents.Result
{
    public class StageDataSave : MonoBehaviour
    {
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
                SaveData saveData = new SaveData();
                saveData.m_stageData = stageNumber;
                new InFileTransmitClient<SaveData>().Set(StorageKey.KEY_STAGENUMBER, saveData);

            }
        }
    }
}

