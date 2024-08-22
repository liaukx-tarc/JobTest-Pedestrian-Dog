using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{

    [SerializeField] protected Transform[] waypoints;
    [SerializeField] protected float normalSpeed;

    protected NavMeshAgent agent;
    protected Queue<Vector3> waypointsQueue;

    protected Coroutine reachCoroutine;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        waypointsQueue = new Queue<Vector3>();

        foreach (Transform waypoint in waypoints)
        {
            waypointsQueue.Enqueue(waypoint.position);
        }

        //Set Nearby Point
        Move(NearbyWayPoint());
    }

    //Patrol
    protected virtual void Patrolling()
    {
        agent.isStopped = false;
        
        //Start Coroutine to check destination reach
        if (reachCoroutine != null)
            StopCoroutine(reachCoroutine);

        reachCoroutine = StartCoroutine(WaitUntilReach());
    }

    protected virtual Vector3 NearbyWayPoint()
    {
        Vector3 nearbyWaypoint = waypoints[0].position;

        //Search the nearby point
        foreach (Transform destination in waypoints)
        {
            if (Vector3.Distance(destination.position, transform.position) < Vector3.Distance(nearbyWaypoint, transform.position))
                nearbyWaypoint = destination.position;
        }

        //Skip the queue until the nearby point
        Vector3 tempDestination;
        do
        {
            tempDestination = waypointsQueue.Dequeue();
            waypointsQueue.Enqueue(tempDestination);

        } while (tempDestination != nearbyWaypoint);

        return tempDestination;
    }

    protected virtual void Move(Vector3 destination)
    {
        agent.destination = destination;
    }

    protected virtual void NextPoint()
    {
        Vector3 destination = waypointsQueue.Dequeue();
        waypointsQueue.Enqueue(destination);

        Move(destination);
        Patrolling();
    }

    protected virtual IEnumerator WaitUntilReach()
    {
        yield return new WaitForSeconds(0.025f);
        yield return new WaitUntil(() => agent.remainingDistance <= agent.stoppingDistance);
        NextPoint();
    }
}

