using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "ScrptableObject/SoundDataBase", fileName = "SoundDataBase")]
public class SoundDatabase : ScriptableObject
{
    public List<SoundData> soundDatas = new List<SoundData>();
}

[Serializable]
public class SoundData 
{
    public int soundIndex;
    public AudioClip baseSound;
    [Range(0f,1f)]
    public float soundVolume;
}

