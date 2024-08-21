using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PedestrianNPC : NPC
{
    [SerializeField] private bool isIdle;
    
    enum State
    {
        Idle,
        Patrolling,
        Running
    }

    private State state = State.Idle;
    private State prevState;

    private void Start()
    {
        state = State.Patrolling;
    }

    private void Update()
    {
        if (isIdle)
            state = State.Idle;
        else
            state = State.Patrolling;

        if (prevState == state)
            return;

        prevState = state;

        switch (state)
        {
            case State.Idle:
                Idle();
                break;

            case State.Patrolling:
                Patrolling();
                break;

            case State.Running:
                break;

            default:
                break;
        }
    }

    //Idle
    private void Idle()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
    }
}
