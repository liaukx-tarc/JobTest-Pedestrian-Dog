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
        Patrolling,
    }

    private State state = State.Idle;
    private State prevState;

    //Stamina
    [SerializeField] private bool hasStamina;

    [SerializeField] private float runSpeed = 25;
    [SerializeField] private float staminaDecrease = 0.1f;
    [SerializeField] private float staminaIncrease = 0.2f;

    public float stamina = 100;
    private Coroutine staminaCoroutine;

    [SerializeField] private RectTransform staminaFieldTrans;

    private void Start()
    {
        state = State.Patrolling;

        if (hasStamina)
        {
            agent.speed = runSpeed;
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

            case State.Patrolling:
                Patrolling();
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

    protected override void Move(Vector3 destination)
    {
        base.Move(destination);

        if (hasStamina)
        {
            if (staminaCoroutine != null)
                StopCoroutine(staminaCoroutine);

            staminaCoroutine = StartCoroutine(StaminaCheck());
        }
    }

    IEnumerator StaminaCheck()
    {
        do
        {
            stamina = Mathf.Max(stamina - staminaDecrease, 0); ;
            staminaFieldTrans.sizeDelta = new Vector2(Mathf.Lerp(0, 6, stamina / 100f), 0);
            yield return null;

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
            stamina = Mathf.Min(stamina + staminaIncrease, 100);
            staminaFieldTrans.sizeDelta = new Vector2(Mathf.Lerp(0, 6, stamina / 100f), 0);
            yield return null;

        } while (stamina != 100);

        //Return to patrol
        state = State.Patrolling;
    }
}
