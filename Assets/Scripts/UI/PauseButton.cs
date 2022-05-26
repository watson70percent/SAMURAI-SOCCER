using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.SceneManagement;
using SamuraiSoccer.Event;

public class PauseButton : MonoBehaviour
{
    public GameObject pausePanel;

    

    public bool enablePause = false;


    int countTest=0;





    private void Start()
    {
        InGameEvent.Goal.Subscribe(x => { enablePause = false; });
        InGameEvent.Play.Subscribe(x => { enablePause = true; });
        InGameEvent.Finish.Subscribe(x => { enablePause = false; });
        InGameEvent.Standby.Subscribe(x => { enablePause = false; });
        InGameEvent.Pause.Subscribe(isPause => { enablePause = !isPause; });

    }

    public void OnClick()
    {
        if (!enablePause) { return; }

        pausePanel.SetActive(true);
        this.gameObject.SetActive(false);

        InGameEvent.PauseOnNext(true);

        Time.timeScale = 1e-10f;

    }

    public void ContinueButton()
    {
        Time.timeScale = 1;
        InGameEvent.PauseOnNext(false);
        pausePanel.SetActive(false);
        this.gameObject.SetActive(true);
    }

    public void RestartButton()
    {
        Time.timeScale = 1;
        
        InGameEvent.ResetOnNext();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MenuBackButton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("StageSelect");
    }

}
