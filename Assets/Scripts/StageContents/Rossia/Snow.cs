using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using SamuraiSoccer.Event;
using UniRx;
using Cysharp.Threading.Tasks;

namespace SamuraiSoccer.StageContents.Rossia
{
    /// <summary>
    /// 雪を制御する．
    /// </summary>
    public class Snow : MonoBehaviour
    {
        public Transform mainCamera;
        public Transform snow;
        public Transform samurai;
        public ParticleSystem particle;
        public Image gameover;
        public AudioSource audioSource;
        [SerializeField]
        private float damage = 0;
        private Vector3 beforePoint = Vector3.zero;
        private float time = 3;
        private bool isPlaying = false;

        // Start is called before the first frame update
        void Start()
        {
            var emission = particle.emission;
            emission.rateOverTimeMultiplier = 4000;
            beforePoint = samurai.position;
            audioSource.volume = 0.05f;
            InGameEvent.Play.Subscribe(OnPlaySnow).AddTo(this);
            InGameEvent.Pause.Subscribe(OnPauseSnow).AddTo(this);
            InGameEvent.Reset.Subscribe(OnReset).AddTo(this);
            InGameEvent.UpdateDuringPlay.Subscribe(OnUpdateDuringPlay).AddTo(this);
        }

        public void TimerReset()
        {
            time = 3;
        }

        void OnPlaySnow(Unit unit)
        {
            var shape = particle.shape;
            var emission = particle.emission;
            shape.scale = new Vector3(60, 50, 1);
            emission.rateOverTimeMultiplier = 800 / 3.0f * Mathf.Exp(damage);
            particle.Play(true);
            isPlaying = true;
        }

        void OnPauseSnow(bool isPause)
        {
            if (isPause)
            {
                particle.Play(false);
                isPlaying = false;
                return;
            }
            OnPlaySnow(Unit.Default);
        }

        void OnReset(Unit unit)
        {
            var shape = particle.shape;
            var emission = particle.emission;
            snow.position = new Vector3(30, 0, 59.6f);
            shape.scale = new Vector3(150, 100, 1);
            emission.rateOverTimeMultiplier = 4000;
            isPlaying = true;
        }

        void OnUpdateDuringPlay(long __)
        {
            time -= Time.deltaTime;
            var diff = samurai.position - beforePoint;
            if (time < 0)
            {
                damage += 4.0f / 30 * Time.deltaTime;
            }

            if (diff.sqrMagnitude < 5 * Time.deltaTime * Time.deltaTime)
            {
                damage += 4.0f / 30 * Time.deltaTime;
                if (damage > 4.8)
                {
                    var lose = new InMemoryDataTransitClient<Result>();
                    lose.Set(StorageKey.KEY_WINORLOSE, Result.Lose);
                    var message = new InMemoryDataTransitClient<string>();
                    message.Set(StorageKey.KEY_RESULTMESSAGE, "凍ってしまった!");
                    InGameEvent.FinishOnNext();
                    _ = GameOver();
                }
            }
            else if (diff.sqrMagnitude > 15 * Time.deltaTime * Time.deltaTime)
            {
                if (damage > 0 && time > 0)
                {
                    damage -= 4.0f / 30 * Time.deltaTime;
                }
            }

            beforePoint = samurai.position;

            var emission = particle.emission;
            emission.rateOverTimeMultiplier = 800 / 3.0f * Mathf.Exp(damage);
            // TODO: 遅くなるスピード反映．
            //pad.speed = 10 - (damage < 1.6 ? 0 : (damage - 1.6f) * 2);
            var volume = damage / 4.8f;
            audioSource.volume = (volume < 0.2) ? 0 : volume;
            SoundMaster.Instance.BGMBolume = (volume < 0.05) ? 1 : 1 - volume;
        }

        // Update is called once per frame
        void Update()
        {
            var pos = snow.position;
            pos.z = mainCamera.position.z;
            snow.position = pos;
        }

        /// <summary>
        /// 凍ってゲームオーバー時の処理．
        /// </summary>
        /// <returns></returns>
        private async UniTask GameOver()
        {
            gameover.gameObject.SetActive(true);
            float time = 0;
            while (time < 2)
            {
                gameover.color = new Color(1, 1, 1, time / 2);
                await UniTask.Yield();
                time += Time.deltaTime;
            }

            await UniTask.Delay(2000);
            time = 0;

            while (time < 1)
            {
                gameover.color = new Color(1 - time, 1 - time, 1 - time, 1);
                await UniTask.Yield();
                time += Time.deltaTime;
            }

            SceneManager.LoadScene("Result");
        }
    }
}