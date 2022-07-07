﻿using UnityEngine;
using System.Linq;

namespace SamuraiSoccer
{
    public class SoundMaster : MonoBehaviour
    {
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

        private int bgmSelectedIndex = -1;
        private float bgmBolume = 1;

        public float BGMBolume
        {
            get
            {
                return bgmBolume;
            }
            set
            {
                bgmBolume = value;
                var bgm = soundDatabase.soundDatas.FirstOrDefault(x => x.soundIndex == bgmSelectedIndex);
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
        public void PlaySE(int soundIndex)
        {
            seAudioSource.volume = soundDatabase.soundDatas.First(x => x.soundIndex == soundIndex).soundVolume * seBolume;
            seAudioSource.PlayOneShot(soundDatabase.soundDatas.First(x => x.soundIndex == soundIndex).baseSound);
        }

        /// <summary>
        /// BGMを流す
        /// </summary>
        /// <param name="soundIndex">音源番号</param>
        public void PlayBGM(int soundIndex)
        {
            bgmSelectedIndex = soundIndex;
            bgmAudioSource.volume = soundDatabase.soundDatas.First(x => x.soundIndex == soundIndex).soundVolume;
            bgmAudioSource.clip = soundDatabase.soundDatas.First(x => x.soundIndex == soundIndex).baseSound;
            bgmAudioSource.Play();
        }

        private void OnDestroy()
        {
            instance = null;
        }
    }
}

