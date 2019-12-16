using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        var contact = collision.contacts[0];
        var dir = contact.point - transform.position;
        collision.gameObject.GetComponent<Rigidbody>().AddForce(dir* 1000);
    }

    private void OnTriggerEnter(Collider other)
    {
        print("B");
    }
}
