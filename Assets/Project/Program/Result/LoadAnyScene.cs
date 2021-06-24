using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        SceneManager.LoadScene(targetSceneName);
    }


    public void LoadStartScene(string targetSceneName)
    {
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
