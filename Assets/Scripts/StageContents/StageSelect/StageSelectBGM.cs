using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

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

        private WorldName current = WorldName.WholeMap;
        private bool isNotCleared = true;

        void Start()
        {
            InFileTransmitClient<int> clearedStageNumFileTransitClient = new InFileTransmitClient<int>();
            var cleared = clearedStageNumFileTransitClient.Get(StorageKey.KEY_STAGENUMBER);
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
                isNotCleared = false;
            }
        }

        public void ChangeBGM(WorldName next, float delayTime)
        {
            if (isNotCleared)
            {
                return;
            }

            if (next == WorldName.WholeMap)
            {
                switch (current)
                {
                    case WorldName.UK: _ = FadeOut(gb, delayTime); break;
                    case WorldName.China: _ = FadeOut(cn, delayTime); break;
                    case WorldName.USA: _ = FadeOut(us, delayTime); break;
                    case WorldName.Russia: _ = FadeOut(ru, delayTime); break;
                }
            }
            else
            {
                switch (next)
                {
                    case WorldName.UK: _ = FadeIn(gb, delayTime); break;
                    case WorldName.China: _ = FadeIn(cn, delayTime); break;
                    case WorldName.USA: _ = FadeIn(us, delayTime); break;
                    case WorldName.Russia: _ = FadeIn(ru, delayTime); break;
                }
            }

            current = next;
        }

        private async UniTask FadeOut(AudioSource source, float delayTime)
        {
            var totalTime = 0.0f;
            while (totalTime < delayTime)
            {
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
                source.volume = totalTime / delayTime;
                totalTime += Time.deltaTime;
                await UniTask.Yield();
            }
            source.volume = 1;
        }

    }
}

