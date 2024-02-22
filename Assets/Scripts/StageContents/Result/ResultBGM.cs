using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace SamuraiSoccer.StageContents.Result
{
    public class ResultBGM : MonoBehaviour
    {
        [SerializeField]
        private int m_winSENum; //勝利時SE番号
        [SerializeField]
        private int m_winBGMNum; //勝利時BGM番号
        [SerializeField]
        private int m_loseSENum; //敗北時SE番号
        [SerializeField]
        private int m_loseBGMNum; //敗北時BGM番号

        public readonly CancellationTokenSource m_cancellationTokenSource = new CancellationTokenSource();

        // Start is called before the first frame update
        async void Start()
        {
            SceneManager.sceneLoaded += SceneLoaded;
        }

        public async UniTask PlayWinBGM()
        {
            await SoundMaster.Instance.PlaySE(m_winSENum);
            if (m_cancellationTokenSource.Token.IsCancellationRequested)
            {
                return;
            }
            SoundMaster.Instance.PlayBGM(m_winBGMNum);
        }

        public async UniTask PlayLoseBGM()
        {
            await SoundMaster.Instance.PlaySE(m_loseSENum);
            if (m_cancellationTokenSource.Token.IsCancellationRequested)
            {
                return;
            }
            SoundMaster.Instance.PlayBGM(m_loseBGMNum);
        }

        void SceneLoaded(Scene nextScene, LoadSceneMode mode)
        {
            m_cancellationTokenSource.Cancel();
            SceneManager.sceneLoaded -= SceneLoaded;
        }
    }
}

