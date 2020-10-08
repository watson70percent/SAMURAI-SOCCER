using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PandaMaker : MonoBehaviour
{
    [SerializeField] GameObject panda;
    [SerializeField] float minSize, maxSize;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Spawn", 1, 0.4f);
        
    }

    // Update is called once per frame
    void Update()
    {
    }


    void Spawn()
    {
        Vector3 pos = new Vector3(58 * Random.value, 100, 100 * Random.value);
        GameObject p = Instantiate(panda, pos, Quaternion.Euler(Random.value * 360, Random.value * 360, Random.value * 360));
        p.transform.localScale = Vector3.one * (Random.value * (maxSize-minSize) + minSize);
    }
}
