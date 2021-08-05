using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultBGM : MonoBehaviour
{
    [SerializeField]
    private ResultManager rm;
    [SerializeField]
    private AudioSource source;
    [SerializeField]
    private AudioClip loseSE;
    [SerializeField]
    private AudioClip lose;
    [SerializeField]
    private AudioClip winSE;
    [SerializeField]
    private AudioClip win;

    // Start is called before the first frame update
    void Start()
    {
        if(rm.ResultState == Result.Win)
        {
            StartCoroutine(PlayWinBGM());
        }
        else
        {
            StartCoroutine(PlayLoseBGM());
        }
    }

    private IEnumerator PlayWinBGM()
    {
        source.PlayOneShot(winSE);
        yield return new WaitForSeconds(winSE.length);
        source.clip = win;
        source.loop = true;
        source.Play();
    }

    private IEnumerator PlayLoseBGM()
    {
        source.PlayOneShot(loseSE);
        yield return new WaitForSeconds(loseSE.length);
        source.clip = lose;
        source.loop = true;
        source.Play();
    } 
}
