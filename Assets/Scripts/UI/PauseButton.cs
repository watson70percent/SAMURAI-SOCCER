using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.SceneManagement;
using SamuraiSoccer.Event;

namespace SamuraiSoccer.UI
{
    public class PauseButton : MonoBehaviour
    {
        [SerializeField]
        GameObject m_pausePanel;
        [SerializeField]
        bool m_enablePause = false;


        private void Start()
        {
            InGameEvent.Goal.Subscribe(x => { m_enablePause = false; }).AddTo(this);
            InGameEvent.Play.Subscribe(x => { m_enablePause = true; }).AddTo(this);
            InGameEvent.Finish.Subscribe(x => { m_enablePause = false; }).AddTo(this);
            InGameEvent.Standby.Subscribe(x => { m_enablePause = false; }).AddTo(this);
            InGameEvent.Pause.Subscribe(isPause => { m_enablePause = !isPause; }).AddTo(this);
        }

        public void OnClick()
        {
            if (!m_enablePause) { return; }
        
            //ポーズパネルを表示して自身は非表示
            m_pausePanel.SetActive(true); 
            this.gameObject.SetActive(false); 
       
            InGameEvent.PauseOnNext(true);
            Time.timeScale = 1e-10f; //時を止める...

        }

        /// <summary>
        /// "ゲームに戻る"ボタンを押したときの処理
        /// </summary>
        public void ContinueButton()
        {
            Time.timeScale = 1;
            InGameEvent.PauseOnNext(false);
            m_pausePanel.SetActive(false);
            this.gameObject.SetActive(true);
        }

        /// <summary>
        /// "ゲームをやり直す"ボタンを押したときの処理
        /// </summary>
        public void RestartButton()
        {
            Time.timeScale = 1;

            InGameEvent.ResetOnNext();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        /// <summary>
        /// "メニューに戻る"ボタンを押したときの処理
        /// </summary>
        public void MenuBackButton()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("StageSelect");
        }

    }

}