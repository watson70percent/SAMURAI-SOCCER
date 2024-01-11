using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using SamuraiSoccer.StageContents.Result;

namespace SamuraiSoccer.StageContents.Fiction
{
    public class Fiction : MonoBehaviour
    {
        [SerializeField]
        private Text m_fictionText;

        [SerializeField]
        private LoadAnyScene m_loadAnyScene;

        [SerializeField]
        private string m_startSceneName;

        private CancellationTokenSource m_tokenSource;

        // Start is called before the first frame update
        private void Start()
        {
            m_tokenSource = new CancellationTokenSource();
            FictionEffect().Forget();        
        }

        public void Cancel()
        {
            m_tokenSource.Cancel();
        }

        private async UniTask FictionEffect()
        {
            await StandardFade.FadeIn(m_fictionText, 2, m_tokenSource.Token);
            await StandardFade.FadeOut(m_fictionText, 2, m_tokenSource.Token);
            await UniTask.Delay(100);
            m_loadAnyScene.LoadSceneNoSE(m_startSceneName);
        }
    }
}
