using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using SamuraiSoccer.StageContents;



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
                int fieldNumber = stageNumberTransitionClient.Get(StorageKey.KEY_FIELDNUMBER);
                int stageNumber = stageNumberTransitionClient.Get(StorageKey.KEY_STAGENUMBER);


                new InFileTransmitClient<int>().Set(StorageKey.KEY_FIELDNUMBER, stageNumber);

            }
        }
    }
}

