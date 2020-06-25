using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
