using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    Animator animator;
    public GameObject slash;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Attack()
    {
        animator.SetTrigger("Attack");
        Instantiate(slash, transform.position, transform.rotation);

    }

    public void Run()
    {

        animator.SetBool("Run", true);

    }

    public void Stay()
    {
        animator.SetBool("Run", false);
    }
}
