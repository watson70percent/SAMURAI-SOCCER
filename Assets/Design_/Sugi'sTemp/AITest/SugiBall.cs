using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SugiBall : MonoBehaviour
{
    Rigidbody rig;
    public float rate;
    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rig.AddForce(-rig.velocity * rate);
    }
}
