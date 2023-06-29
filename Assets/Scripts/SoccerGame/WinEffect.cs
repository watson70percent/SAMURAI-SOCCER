using UnityEngine;
using UnityEngine.SceneManagement;
using SamuraiSoccer.Event;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEditor;

namespace SamuraiSoccer.SoccerGame
{
    public class WinEffect : MonoBehaviour
    {
        [SerializeField]
        private SceneAsset m_resultScene;

        // Start is called before the first frame update
        private void Start()
        {
            InGameEvent.Finish.Subscribe(x =>
            {
                _ = SlowToWin();
            }).AddTo(this);
        }

        /// <summary>
        /// ���������ɃV�[���J�ڂ܂ł������ɂ��đJ�ڂ�����
        /// </summary>
        /// <returns></returns>
        private async UniTask SlowToWin()
        {
            Time.timeScale = 0.3f;
            await SoundMaster.Instance.PlaySE(11);
            Time.timeScale = 1;
            SceneManager.LoadScene(m_resultScene.name);
        }
    }
}
