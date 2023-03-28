using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using SamuraiSoccer.Event;

namespace SamuraiSoccer.StageContents.StageSelect
{
    /// <summary>
    /// ステージセレクトのBGMを監理。
    /// </summary>
    public class StageSelectBGM : MonoBehaviour
    {
        [SerializeField]
        private AudioSource start;
        [SerializeField]
        private AudioSource jp;
        [SerializeField]
        private AudioSource gb;
        [SerializeField]
        private AudioSource cn;
        [SerializeField]
        private AudioSource us;
        [SerializeField]
        private AudioSource ru;
        [SerializeField]
        private Conversation.ConversationManager cvm;

        private Stage current = Stage.World;
        private bool isNotCleared = true;

        private bool isPlaying = false;

        void Start()
        {
            StartBGM();
            isPlaying = true;
        }

        private void Update()
        {
            if (cvm.stopReq == isPlaying)
            {
                if (isPlaying)
                {
                    StopBGM();
                    isPlaying = false;
                }
                else
                {
                    StartBGM();
                    isPlaying = true;
                }
            }
        }

        public void ChangeBGM(Stage next, float delayTime)
        {
            if (isNotCleared)
            {
                return;
            }

            switch (current)
            {
                case Stage.UK: _ = FadeOut(gb, delayTime); break;
                case Stage.China: _ = FadeOut(cn, delayTime); break;
                case Stage.USA: _ = FadeOut(us, delayTime); break;
                case Stage.Russian: _ = FadeOut(ru, delayTime); break;
            }

            switch (next)
            {
                case Stage.UK: _ = FadeIn(gb, delayTime); break;
                case Stage.China: _ = FadeIn(cn, delayTime); break;
                case Stage.USA: _ = FadeIn(us, delayTime); break;
                case Stage.Russian: _ = FadeIn(ru, delayTime); break;
            }
            current = next;
        }

        public void StartBGM()
        {
            InFileTransmitClient<SaveData> clearedStageNumFileTransitClient = new InFileTransmitClient<SaveData>();
            SaveData data;
            int cleared = 0;
            clearedStageNumFileTransitClient.TryGet(StorageKey.KEY_STAGENUMBER, out data);
            cleared = data.m_stageData;
            var all = new AudioSource[] { start, jp, gb, cn, us, ru };
            foreach (var bgm in all)
            {
                bgm.volume = 1;
                bgm.time = 0;
                bgm.loop = true;
            }

            if (cleared == 0)
            {
                start.Play();
                isNotCleared = true;
            }
            else
            {
                gb.volume = 0;
                cn.volume = 0;
                us.volume = 0;
                ru.volume = 0;
                jp.Play();
                gb.Play();
                cn.Play();
                us.Play();
                ru.Play();
                switch(current)
                {
                    case Stage.UK: gb.volume = 1; break;
                    case Stage.China: cn.volume = 1; break;
                    case Stage.USA: us.volume = 1; break;
                    case Stage.Russian: ru.volume = 1; break;
                }
                isNotCleared = false;
            }
        }

        public void StopBGM()
        {
            start.Stop();
            jp.Stop();
            gb.Stop();
            cn.Stop();
            us.Stop();
            ru.Stop();
        }

        private async UniTask FadeOut(AudioSource source, float delayTime)
        {
            var totalTime = 0.0f;
            while (totalTime < delayTime)
            {
                if (source == null)
                {
                    return;
                }
                source.volume = (delayTime - totalTime) / delayTime;
                totalTime += Time.deltaTime;
                await UniTask.Yield();
            }
            source.volume = 0;
        }

        private async UniTask FadeIn(AudioSource source, float delayTime)
        {
            var totalTime = 0.0f;
            while (totalTime < delayTime)
            {
                if (source == null)
                {
                    return;
                }
                source.volume = totalTime / delayTime;
                totalTime += Time.deltaTime;
                await UniTask.Yield();
            }
            source.volume = 1;
        }

    }
}

