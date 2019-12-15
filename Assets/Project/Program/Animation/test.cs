using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        print("#A");
    }

    private void OnTriggerEnter(Collider other)
    {
        print("B");
    }
}
