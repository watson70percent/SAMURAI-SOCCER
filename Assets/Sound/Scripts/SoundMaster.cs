using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    /// <summary>
    /// SEを流す
    /// </summary>
    /// <param name="soundIndex">音源番号</param>
    public void PlaySE(int soundIndex)
    {
        seAudioSource.volume = soundDatabase.soundDatas.Where(x => x.soundIndex == soundIndex).First().soundIndex;
        seAudioSource.PlayOneShot(soundDatabase.soundDatas.Where(x => x.soundIndex == soundIndex).First().baseSound);
    }

    /// <summary>
    /// BGMを流す
    /// </summary>
    /// <param name="soundIndex">音源番号</param>
    public void PlayBGM(int soundIndex)
    {
        bgmAudioSource.volume = soundDatabase.soundDatas.Where(x => x.soundIndex == soundIndex).First().soundVolume;
        bgmAudioSource.PlayOneShot(soundDatabase.soundDatas.Where(x => x.soundIndex == soundIndex).First().baseSound);
    }

    private void OnDestroy()
    {
        instance = null;
    }
}
