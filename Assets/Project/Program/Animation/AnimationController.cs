using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnimationController : MonoBehaviour
{
    Animator animator;
    public GameObject slash;
    public event EventHandler AttackEvent;

    GameManager gameManager;

    GameState state = GameState.Reset;


    void SwitchState(StateChangedArg a)
    {
        state = a.gameState;
    }
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.StateChange += SwitchState;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Attack()
    {
        if (state != GameState.Playing) { return; }
        animator.SetTrigger("Attack");
        Instantiate(slash, transform.position, transform.rotation);

        AttackEvent(this, EventArgs.Empty);
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
