using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SamuraiSoccer.StageContents.China
{
    /// <summary>
    /// 音を鳴らして消えるオブジェクト
    /// </summary>
    public class SoundBox : MonoBehaviour
    {
        public void StartSoundBox(AudioClip clip)
        {
            var audioSource = this.gameObject.AddComponent<AudioSource>();
            audioSource.PlayOneShot(clip);

            Invoke("Vanish", 3);
        }

        void Vanish()
        {
            Destroy(gameObject);
        }
    }


    public class SoundBoxUtil : MonoBehaviour
    {
        public static void SetSoundBox(Vector3 position, AudioClip clip)
        {
            GameObject soundbox = new GameObject("SoundBox");
            soundbox.transform.position = position;
            soundbox.AddComponent<SoundBox>().StartSoundBox(clip);

        }
    }

    



}