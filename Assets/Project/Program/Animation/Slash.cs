using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : MonoBehaviour
{

    public Animator animator;
    ParticleSystem.MainModule particle;
    float time;
    float alpha;
    // Start is called before the first frame update
    void Start()
    {
        particle = GetComponent<ParticleSystem>().main;
        alpha = particle.startColor.color.a;
    }


    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        particle.startColor = new Color(particle.startColor.color.r, particle.startColor.color.g, particle.startColor.color.b, alpha - time);


        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Finish")) {
            Destroy(transform.root.gameObject);
        } 


    }

    //private void OnCollisionEnter(Collision collision)
    //{

    //    if (collision.gameObject.GetComponent<EasyCPU>()?.status.ally==false)
    //    {
    //        var contact = collision.contacts[0];
    //        var dir = contact.point - transform.position;
    //        collision.gameObject.GetComponent<Rigidbody>().AddForce(dir * 1000);
    //    }

        

    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<EasyCPU>()?.status.ally == false)
        {
            
            var dir = other.transform.position - transform.position;
            other.gameObject.GetComponent<Rigidbody>()?.AddForce(dir * 1000);
        }
    }
}
