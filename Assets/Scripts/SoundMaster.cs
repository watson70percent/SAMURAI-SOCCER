using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;
using System;

namespace SamuraiSoccer
{
    public class SoundMaster : MonoBehaviour
    {
        public readonly static int STAGE_SELECT_BGM_INDEX = int.MaxValue; 

        //BGMとSEが管理されている
        private static SoundDatabase soundDatabase;
        //BGMのAudioSource
        private static AudioSource bgmAudioSource;
        //SEのAudioSource
        private static AudioSource seAudioSource;

        private static SoundMaster instance;
        public static SoundMaster Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject soundObj = new GameObject("SoundObj");
                    instance = soundObj.AddComponent<SoundMaster>();
                    bgmAudioSource = soundObj.AddComponent<AudioSource>();
                    bgmAudioSource.loop = true;
                    seAudioSource = soundObj.AddComponent<AudioSource>();
                    seAudioSource.loop = false;
                    soundDatabase = Resources.Load<SoundDatabase>("SoundDataBase");
                }
                return instance;
            }
        }

        public float seBolume = 1;

        private ReactiveProperty<int> bgmSelectedIndex = new ReactiveProperty<int>(-1);
        private float bgmBolume = 1;

        /// <summary>
        /// 現在流れているBGMの番号。
        /// </summary>
        public int BGMIndex
        {
            get => bgmSelectedIndex.Value;
        }

        /// <summary>
        /// BGM番号のサブスクライブ先。
        /// </summary>
        public IObservable<int> BGMIndexObservable
        {
            get => bgmSelectedIndex;
        }

        public float BGMBolume
        {
            get
            {
                return bgmBolume;
            }
            set
            {
                bgmBolume = value;
                var bgm = soundDatabase.soundDatas.FirstOrDefault(x => x.soundIndex == bgmSelectedIndex.Value);
                if (bgm != default)
                {
                    bgmAudioSource.volume = bgm.soundVolume * bgmBolume;
                }
            }
        }

        /// <summary>
        /// SEを流す
        /// </summary>
        /// <param name="soundIndex">音源番号</param>
        public async UniTask PlaySE(int soundIndex)
        {
            seAudioSource.volume = soundDatabase.soundDatas.First(x => x.soundIndex == soundIndex).soundVolume * seBolume;
            var targetClip = soundDatabase.soundDatas.First(x => x.soundIndex == soundIndex).baseSound;
            seAudioSource.PlayOneShot(targetClip);
            await UniTask.Delay((int)(targetClip.length*1000),true); //msなので1000をかけて単位変換
        }

        /// <summary>
        /// BGMを流す
        /// </summary>
        /// <param name="soundIndex">音源番号</param>
        /// <param name="startTime">音源の開始時間</param>
        public void PlayBGM(int soundIndex, float startTime = 0)
        {
            bgmSelectedIndex.Value = soundIndex;
            if (soundIndex == STAGE_SELECT_BGM_INDEX)
            {
                bgmAudioSource.Stop();
                return;
            }
            bgmAudioSource.volume = soundDatabase.soundDatas.First(x => x.soundIndex == soundIndex).soundVolume;
            bgmAudioSource.clip = soundDatabase.soundDatas.First(x => x.soundIndex == soundIndex).baseSound;
            bgmAudioSource.time = startTime;
            bgmAudioSource.Play();
        }

        /// <summary>
        /// 音を止める。
        /// </summary>
        /// <returns>
        /// BGMの停止時間。
        /// </returns>
        public float StopSound()
        {
            bgmAudioSource.Stop();
            seAudioSource.Stop();
            bgmSelectedIndex.Value = -1;
            return bgmAudioSource.time;
        }

        private void OnDestroy()
        {
            instance = null;
            bgmSelectedIndex.Value = -1;
        }
    }
}


