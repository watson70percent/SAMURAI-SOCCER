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
}
