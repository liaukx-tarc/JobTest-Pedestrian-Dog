using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DogNPC : NPC
{
    enum State
    {
        Idle,
        Recovering,
        Walk,
        Running
    }

    private State state = State.Idle;
    private State prevState;

    //Stamina
    [SerializeField] private bool hasStamina;

    [SerializeField] private float runSpeed = 25;
    [SerializeField] private float staminaReduce = 0.1f;
    [SerializeField] private float staminaRecover = 0.2f;

    public float stamina = 100;
    private Coroutine staminaCoroutine;

    [SerializeField] private RectTransform staminaFieldTrans;

    private void Start()
    {
        if (hasStamina)
        {
            state = State.Running;
        }
        else
        {
            state = State.Walk;
            agent.speed = normalSpeed;
        }
    }

    private void Update()
    {
        if (prevState == state)
            return;

        prevState = state;

        switch (state)
        {
            case State.Idle:
                Idle();
                break;

            case State.Walk:
                Patrolling();
                break;

            case State.Running:
                Running();
                break;

            case State.Recovering:
                Recovering();
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

    //Running
    private void Running()
    {
        agent.isStopped = false;
        agent.speed = runSpeed;

        //Start Coroutine to check destination reach
        if (reachCoroutine != null)
            StopCoroutine(reachCoroutine);

        reachCoroutine = StartCoroutine(WaitUntilReach());

        if (staminaCoroutine != null)
            StopCoroutine(staminaCoroutine);

        staminaCoroutine = StartCoroutine(StaminaCheck());
    }

    IEnumerator StaminaCheck()
    {
        do
        {
            stamina = Mathf.Max(stamina - staminaReduce, 0); ;
            staminaFieldTrans.sizeDelta = new Vector2(Mathf.Lerp(0, 6, stamina / 100f), 0);
            yield return new WaitForFixedUpdate();

        } while (stamina > 0);

        //Stop moving & Recover stamina
        state = State.Recovering;
    }

    void Recovering()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        if (staminaCoroutine != null)
            StopCoroutine(staminaCoroutine);

        staminaCoroutine = StartCoroutine(StaminaRecovers());
    }

    IEnumerator StaminaRecovers()
    {
        do
        {
            stamina = Mathf.Min(stamina + staminaRecover, 100);
            staminaFieldTrans.sizeDelta = new Vector2(Mathf.Lerp(0, 6, stamina / 100f), 0);
            yield return new WaitForFixedUpdate();

        } while (stamina != 100);

        //Return to patrol
        state = State.Running;
    }

    //Variable Set
    public void Set_NormalSpeed(float speed)
    {
        normalSpeed = speed;

        if (!hasStamina)
            agent.speed = normalSpeed;
    }

    public void Set_RunSpeed(float speed)
    {
        runSpeed = speed;

        if (hasStamina)
            agent.speed = runSpeed;
    }

    public void Set_StaminaReduce(float staminaReduce)
    {
        this.staminaReduce = staminaReduce;
    }

    public void Set_StaminaRecover(float staminaRecover)
    {
        this.staminaRecover = staminaRecover;
    }
}
