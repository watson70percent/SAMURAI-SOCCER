using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidCarScript : MonoBehaviour
{
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
        if (other.tag != "Ball" && other.tag != "Player")
        {
            //Debug.Log("colider.tag = "+other.tag);
            Rigidbody rb = other.transform.GetComponent<Rigidbody>();
            Vector3 v3 = Vector3.zero;
            v3.y += 300;
            rb.AddForce(v3);
        }

        //   if()
    }
}
