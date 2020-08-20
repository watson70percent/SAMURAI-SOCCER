using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : MonoBehaviour
{

    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }


    // Update is called once per frame
    void Update()
    {

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Finish")) {
            Destroy(transform.root.gameObject);
        } 
    }

    private void OnTriggerEnter(Collider other)
    {
        print("fhjaskh");
        Destroy(other.gameObject);
    }
    private void OnCollisionEnter(Collision collision)
    {
        print("collision");
        Destroy(collision.gameObject);
    }
}
