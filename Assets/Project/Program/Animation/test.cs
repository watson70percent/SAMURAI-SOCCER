using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    ParticleSystem.MainModule particle;
    float time;
    float alpha;
    public float banishrate;

    private void Start()
    {
        particle = GetComponent<ParticleSystem>().main;
        alpha = particle.startColor.color.a;
    }

    private void Update()
    {
        time += Time.deltaTime;
        particle.startColor = new Color(particle.startColor.color.r, particle.startColor.color.g, particle.startColor.color.b, alpha-time);
    }

    private void OnCollisionEnter(Collision collision)
    {
        print("hit!");
        var contact = collision.contacts[0];
        var dir = contact.point - transform.position;
        collision.gameObject.GetComponent<Rigidbody>().AddForce(dir* 1000);
        
    }

    private void OnTriggerEnter(Collider other)
    {
        print("B");
    }
}
