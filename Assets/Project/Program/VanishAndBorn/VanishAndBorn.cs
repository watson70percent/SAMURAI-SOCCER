using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanishAndBorn : MonoBehaviour
{

    Vector3 center = new Vector3(30, 0, 50);
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Check", 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    void Check()
    {
        var dis = (transform.position-center).magnitude;
        if (dis > 100)
        {
            GameObject.Find("GameManager").GetComponent<EasyCPUManager>().kill(this.gameObject);
        }
    }
}
