using UnityEngine;
using UnityEngine.SceneManagement;

namespace SamuraiSoccer.StageContents.Result
{
    public class LoadAnyScene : MonoBehaviour
    {
        /// <summary>
        /// Scene遷移
        /// </summary>
        /// <param name="targetSceneName">
        /// ロード先のScene
        /// </param>
        public void LoadScene(string targetSceneName)
        {
            SoundMaster.Instance.PlaySE(0);
            SceneManager.LoadScene(targetSceneName);
        }

        /// <summary>
        /// Scene遷移
        /// </summary>
        /// <param name="targetSceneName">
        /// ロード先のScene
        /// </param>
        public void LoadSceneNoSE(string targetSceneName)
        {
            SceneManager.LoadScene(targetSceneName);
        }

        public void LoadStartScene(string targetSceneName)
        {
            SoundMaster.Instance.PlaySE(0);
            if (PlayerPrefs.GetInt("DoneTutorial") == 0)
            {
                LoadScene("Tutorial");
            }
            else
            {
                LoadScene(targetSceneName);
            }
        }
    }
}

