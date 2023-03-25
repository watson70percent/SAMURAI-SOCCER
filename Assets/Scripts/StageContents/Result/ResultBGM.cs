using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using Cysharp.Threading.Tasks;

namespace SamuraiSoccer.StageContents.Result
{
    public class ResultBGM : MonoBehaviour
    {
        [SerializeField]
        private ResultManager m_rm;
        [SerializeField]
        private int m_winSENum; //勝利時SE番号
        [SerializeField]
        private int m_winBGMNum; //勝利時BGM番号
        [SerializeField]
        private int m_loseSENum; //敗北時SE番号
        [SerializeField]
        private int m_loseBGMNum; //敗北時BGM番号

        // Start is called before the first frame update
        async void Start()
        {
            SceneManager.sceneLoaded += SceneLoaded;
            if (m_rm.ResultState == GameResult.Win)
            {
                await PlayWinBGM();
            }
            else
            {
                await PlayLoseBGM();
            }
        }

        private async UniTask PlayWinBGM()
        {
            await SoundMaster.Instance.PlaySE(m_winSENum);
            SoundMaster.Instance.PlayBGM(m_winBGMNum);
        }

        private async UniTask PlayLoseBGM()
        {
            await SoundMaster.Instance.PlaySE(m_loseSENum);
            SoundMaster.Instance.PlayBGM(m_loseBGMNum);
        }

        void SceneLoaded(Scene nextScene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= SceneLoaded;
        }
    }
}

