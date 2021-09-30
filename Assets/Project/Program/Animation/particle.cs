using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particle : MonoBehaviour
{
    public void PA()
    {
        GetComponent<ParticleSystem>().Play();
    }

    private void OnParticleCollision(GameObject other)
    {
        print("JUXCG");
        other.GetComponent<Rigidbody>().AddForce(0, 0, 10000);
    }
}
