using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private WorldName current;

    void Start()
    {
        var cleared = StageDataManager.LoadStageData();
        var all = new AudioSource[] { start, jp, gb, cn, us, ru };
        foreach(var bgm in all)
        {
            bgm.volume = 1;
            bgm.time = 0;
            bgm.loop = true;
        }

        if (cleared.WorldName == WorldName.UK && cleared.StageNumber < 0)
        {
            start.Play();
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
        }
    }

    public void ChangeBGM(WorldName next, float delayTime)
    {
        if(next == WorldName.WholeMap)
        {
            switch (current)
            {
                case WorldName.UK:      StartCoroutine(FadeOut(gb, delayTime)); break;
                case WorldName.China:   StartCoroutine(FadeOut(cn, delayTime)); break;
                case WorldName.America: StartCoroutine(FadeOut(us, delayTime)); break;
                case WorldName.Russia:  StartCoroutine(FadeOut(ru, delayTime)); break;
            }
        }
        else
        {
            switch (next)
            {
                case WorldName.UK:      StartCoroutine(FadeIn(gb, delayTime)); break;
                case WorldName.China:   StartCoroutine(FadeIn(cn, delayTime)); break;
                case WorldName.America: StartCoroutine(FadeIn(us, delayTime)); break;
                case WorldName.Russia:  StartCoroutine(FadeIn(ru, delayTime)); break;
            }
        }

        current = next;
    }

    private IEnumerator FadeOut(AudioSource source, float delayTime)
    {
        var totalTime = 0.0f;
        while(totalTime < delayTime)
        {
            source.volume = (delayTime - totalTime) / delayTime;
            totalTime += Time.deltaTime;
            yield return null;
        }
        source.volume = 0;
    }

    private IEnumerator FadeIn(AudioSource source, float delayTime)
    {
        var totalTime = 0.0f;
        while (totalTime < delayTime)
        {
            source.volume = totalTime / delayTime;
            totalTime += Time.deltaTime;
            yield return null;
        }
        source.volume = 1;
    }

}
