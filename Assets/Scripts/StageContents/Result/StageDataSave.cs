using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using SamuraiSoccer.StageContents.Conversation;

namespace SamuraiSoccer.StageContents.Result
{
    public class StageDataSave : MonoBehaviour
    {
        /// <summary>
        /// クリア番号が過去のセーブデータ以上だったらセーブデータを更新する
        /// </summary>
        /// <param name="clearNumber">クリア番号</param>
        /// <returns>セーブしたかどうか</returns>
        public bool Save(int clearNumber)
        {
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
            if (clearNumber >= savedNumber)
            {
                // ステージ情報を保存
                SaveData saveData = new SaveData();
                saveData.m_stageData = clearNumber + 1;
                new InFileTransmitClient<SaveData>().Set(StorageKey.KEY_STAGENUMBER, saveData);
                return true;
            }
            return false;
        }

        /// <summary>
        /// セーブデータを初期化する(ResetだとMonoBehaviourと名前が被るから命名変更)
        /// </summary>
        /// <returns></returns>
        public void ResetData()
        {
            InFileTransmitClient<SaveData> fileTransitClient = new InFileTransmitClient<SaveData>();
            SaveData data = new SaveData();
            data.m_stageData = 0;
            fileTransitClient.Set(StorageKey.KEY_STAGENUMBER, data);
            PlayerPrefs.SetInt("DoneTutorial", 0);
        }
    }
}

