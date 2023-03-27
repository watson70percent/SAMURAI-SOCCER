using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;

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

        public int BgmIndex { get => bgmSelectedIndex; }

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
        public void PlayBGM(int soundIndex)
        {
            bgmSelectedIndex = soundIndex;
            bgmAudioSource.volume = soundDatabase.soundDatas.First(x => x.soundIndex == soundIndex).soundVolume;
            bgmAudioSource.clip = soundDatabase.soundDatas.First(x => x.soundIndex == soundIndex).baseSound;
            bgmAudioSource.Play();
        }

        public void StopBGM()
        {
            bgmSelectedIndex = -1;
            bgmAudioSource.Stop();
        }

        private void OnDestroy()
        {
            instance = null;
        }
    }
}


