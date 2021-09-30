using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidCarScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Ball" && other.tag != "Player")
        {
            Rigidbody rb = other.transform.GetComponent<Rigidbody>();
            Vector3 v3 = Vector3.zero;
            v3.y += 300;
            if (rb!=null)
            {
                rb.AddForce(v3);
            }
        }
    }
}
