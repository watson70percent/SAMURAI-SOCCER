using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall_temp : MonoBehaviour
{
    public Vector3 normalVec;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ball")
        {
            GameObject ball = other.gameObject;
            var velocity = ball.GetComponent<Rigidbody>().velocity;
            float nvelMag= Vector3.Dot(velocity, normalVec);
            if (nvelMag > 0) { return; }
            var nvel = normalVec*Vector3.Dot(velocity,normalVec);
            var hvel = velocity - nvel;
            var newvel = hvel - nvel;
            ball.GetComponent<Rigidbody>().velocity = newvel;
        }
    }
}
