using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using Cysharp.Threading.Tasks;
using SamuraiSoccer.Event;

namespace SamuraiSoccer.SoccerGame
{
    public class FinishProcess : MonoBehaviour
    {
        [SerializeField]
        private string m_resultSceneName = "Result";

        [SerializeField]
        private int m_delayMiliSec = 1000;

        // Start is called before the first frame update
        void Start()
        {
            InGameEvent.Finish.Subscribe(async _ =>
            {
                await FinishContents();
            });
        }

        public async UniTask FinishContents()
        {
            //シーンの移動処理
            await UniTask.Delay(m_delayMiliSec);
            Time.timeScale = 1;
            SceneManager.LoadScene(m_resultSceneName);
        }
    }
}

