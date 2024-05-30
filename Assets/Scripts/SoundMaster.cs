using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;
using System;
using System.Collections.Generic;

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
                    var obs = instance.playSESubject.Pairwise().Share();
                    obs.Where(clips => clips.Current != clips.Previous).Select(clips => clips.Current).Subscribe(clip => instance.PlaySEInternal(clip)).AddTo(instance);
                    obs.Where(clips => clips.Current == clips.Previous).ThrottleFirst(TimeSpan.FromSeconds(0.5)).Select(clips => clips.Current).Subscribe(clip => instance.PlaySEInternal(clip)).AddTo(instance);
                    instance.playSESubject.OnNext(null);
                }
                return instance;
            }
        }

        public float seBolume = 1;

        private ReactiveProperty<int> bgmSelectedIndex = new ReactiveProperty<int>(-1);
        private int reserveIndex = -1;
        private Stack<(int, float)> requests = new();
        private float bgmBolume = 1;
        private Subject<AudioClip> playSESubject = new Subject<AudioClip>();

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
            playSESubject.OnNext(targetClip);
            await UniTask.Delay((int)(targetClip.length * 1000), true); //msなので1000をかけて単位変換
        }

        private void PlaySEInternal(AudioClip targetClip)
        {
            seAudioSource.PlayOneShot(targetClip);
        }

        /// <summary>
        /// BGMを流す
        /// </summary>
        /// <param name="soundIndex">音源番号</param>
        /// <param name="startTime">音源の開始時間</param>
        public void PlayBGM(int soundIndex, float startTime = 0)
        {
            if (reserveIndex >= 0)
            {
                if (reserveIndex != soundIndex)
                {
                    requests.Push((soundIndex, startTime));
                    return;
                }
                reserveIndex = -1;
                requests.Clear();
            }
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
        /// <param name="reserve">
        /// 次に開始するBGMを予約する。このBGM以外の音楽は3秒間再生を開始しない。
        /// 予約された音楽が再生されなければその間に再生しようとした音楽->元々流れていた音楽の優先度で再生。
        /// 再生されなかったら
        /// </param>
        /// <returns>
        /// BGMの停止時間。
        /// </returns>
        public float StopSound(int reserve = -1)
        {
            if (reserveIndex == BGMIndex)
            {
                reserveIndex = -1;
                requests.Clear();
            }
            if (reserve >= 0)
            {
                reserveIndex = reserve;
                if (BGMIndex >= 0)
                {
                    requests.Push((BGMIndex, bgmAudioSource.time));
                }
                _ = ReleaseBlocking(3000);
            }
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

        private async UniTask ReleaseBlocking(int millisec)
        {
            await UniTask.Delay(millisec);
            
            if (BGMIndex == -1 && requests.TryPop(out (int, float) req))
            {
                reserveIndex = -1;
                PlayBGM(req.Item1, req.Item2 + 3.0f);
            }
            requests.Clear();
        }
    }
}


