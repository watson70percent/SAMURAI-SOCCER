using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidCarScript : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.tag != "Ball" && other.tag != "Player")
        {
            Rigidbody rb = other.transform.GetComponent<Rigidbody>();
           
            if (rb!=null)
            {
                Vector3 v3 = rb.velocity;
                v3.y = 10;
                rb.velocity = v3;
            }
        }
    }
}
