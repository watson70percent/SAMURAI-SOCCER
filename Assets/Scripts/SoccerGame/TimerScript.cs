using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using SamuraiSoccer.Event;
using SamuraiSoccer.StageContents;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace SamuraiSoccer.UI
{
    public class TimerScript : MonoBehaviour
    {
        [SerializeField]
        private string m_resultSceneName = "Result";
        [SerializeField]
        private int m_longWhistleNum;
        [SerializeField] 
        private float m_limitTimeSec = 300; //制限時間
        [SerializeField]
        private Text m_timeText; //時間を表示させるもの

        private bool m_isPlaying; //試合中のフラグ
        private bool m_end = false; //試合終了のフラグ
        private float m_elapsedTime = 0; //経過時間

        private void Start()
        {
            InGameEvent.Reset.Subscribe(_ =>
            {
                m_elapsedTime = 0;
                int displayTime = (int)(m_limitTimeSec - m_elapsedTime);
                m_timeText.text = ((int)(displayTime / 60)).ToString("0") + ":" + Mathf.CeilToInt(displayTime % 60).ToString("00");
            }).AddTo(this);
            InGameEvent.Pause.Subscribe(_ =>
            {
                m_isPlaying = false;
            }).AddTo(this);
            InGameEvent.Play.Subscribe(_ =>
            {
                m_isPlaying = true;
            }).AddTo(this);
            InGameEvent.Finish.Subscribe(_ =>
            {
                m_isPlaying = false;
                m_end = true;
            }).AddTo(this);
        }

        // Update is called once per frame
        async void Update()
        {
            if (m_isPlaying && !m_end)
            {
                if (m_limitTimeSec > m_elapsedTime)
                {
                    m_elapsedTime += Time.deltaTime;
                    //Debug.Log(elapsedTime);
                    int displayTime = (int)(m_limitTimeSec - m_elapsedTime);
                    m_timeText.text = ((int)(displayTime / 60)).ToString("0") + ":" + Mathf.CeilToInt(displayTime % 60).ToString("00");
                }
                else
                {
                    InGameEvent.FinishOnNext();
                    await FinishProcess();
                }
            }
        }

        async UniTask FinishProcess()
        {
            InMemoryDataTransitClient<GameResult> gameResultTransitClient = new InMemoryDataTransitClient<GameResult>();
            gameResultTransitClient.Set(StorageKey.KEY_WINORLOSE, GameResult.Lose);
            Time.timeScale = 0.2f;
            SoundMaster.Instance.PlaySE(m_longWhistleNum);
            await UniTask.Delay(1000);
            Time.timeScale = 1f;
            SceneManager.LoadScene(m_resultSceneName);
        }
    }
}