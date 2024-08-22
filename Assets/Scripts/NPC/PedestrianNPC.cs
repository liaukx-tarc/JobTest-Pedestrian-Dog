using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

public class PedestrianNPC : NPC
{
    public enum State
    {
        Idle,
        Patrolling,
        Fleeing
    }

    [SerializeField] private State state = State.Idle;
    private State prevState;

    //Flee
    [Header("Flee")]
    [SerializeField] private float fleeDuration;
    [SerializeField] private float fleeSpeed;
    [SerializeField] private float fleeDistance;

    private Transform fleeTarget;
    private Coroutine fleeCoroutine;

    [Header("FOV")]
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshCollider meshCollider;
    [SerializeField] private LayerMask blockingMask;
    [SerializeField] private LayerMask detectLayer;
    [SerializeField] private Transform lookingDir;
    [SerializeField] private CustomMaterialColor fovColor;
    [SerializeField] private CustomMaterialColor nearbyViewColor;

    [Space]
    [SerializeField] private float fov = 90f;
    [SerializeField] private float viewDistance = 50f;
    [SerializeField] private int rayCount = 2;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color fleeingColor;

    Mesh mesh;

    private void Start()
    {
        state = State.Patrolling;
        agent.speed = normalSpeed;

        mesh = new Mesh();
        meshFilter.mesh = mesh;
    }

    private void Update()
    {
        dogDetect();

        if (prevState == state)
            return;

        prevState = state;
        UIController.instance.ChangedControlBtn(state);

        switch (state)
        {
            case State.Idle:
                Idle();
                break;

            case State.Patrolling:
                Patrolling();
                break;

            case State.Fleeing:
                Fleeing(fleeTarget);
                break;

            default:
                break;
        }
    }

    //Idle
    private void Idle()
    {
        StopAllCoroutines();
        fleeTarget = null;

        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        fovColor.color = normalColor;
        fovColor.ApplyColor();

        nearbyViewColor.color = normalColor / 2;
        nearbyViewColor.ApplyColor();

    }

    public void ChangedIdle()
    {
        if (state != State.Idle)
            state = State.Idle;
        else
            BackToPatrol();
    }

    //Patrolling
    protected override void Patrolling()
    {
        base.Patrolling();

        fovColor.color = normalColor;
        fovColor.ApplyColor();

        nearbyViewColor.color = normalColor / 2;
        nearbyViewColor.ApplyColor();

    }

    protected override IEnumerator WaitUntilReach()
    {
        yield return new WaitForSeconds(0.025f);
        yield return new WaitUntil(() => agent.remainingDistance <= agent.stoppingDistance);

        if (state == State.Fleeing)
        {
            if (Vector3.Distance(transform.position, fleeTarget.position) > fleeDistance * 2)
            {
                BackToPatrol();
            }

            else
            {
                Fleeing(fleeTarget);
            }
        }

        else
        {
            NextPoint();
        }
    }

    //Flee
    private void dogDetect()
    {
        Vector3 npcDir = lookingDir.position - transform.position;
        npcDir = npcDir.normalized;
        float npcViewAngle = Mathf.Atan2(npcDir.z, npcDir.x) * Mathf.Rad2Deg;
        if (npcViewAngle < 0)
            npcViewAngle += 360;

        float angle = npcViewAngle + fov / 2f;
        float angleIncrease = fov / rayCount;
        Vector3 origin = transform.position;

        Vector3[] vertices = new Vector3[rayCount + 2];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            float angleRed = angle * (Mathf.PI / 180f);
            Vector3 angleVertex = new Vector3(Mathf.Cos(angleRed), 0, Mathf.Sin(angleRed));

            Vector3 vertex;
            vertex = origin + angleVertex * viewDistance;

            //View Block
            RaycastHit hit;
            Physics.Raycast(origin, angleVertex, out hit, viewDistance, blockingMask);
            if (hit.collider == null)
            {
                vertex = origin + angleVertex * viewDistance;
            }

            else
            {
                vertex = hit.point;
            }

            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }

            vertexIndex++;
            angle -= angleIncrease;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        meshCollider.sharedMesh = mesh;

        meshFilter.transform.localPosition = -transform.position;
        meshFilter.transform.parent.localRotation = Quaternion.Euler(0, npcViewAngle - fov, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Dog"))
        {
            if (state == State.Fleeing)
            {
                KeepFleeing();

                if (other.transform != fleeTarget)
                {
                    fleeTarget = other.transform;
                    Fleeing(fleeTarget);
                }
            }

            else
            {
                state = State.Fleeing;
                fleeTarget = other.transform;
            }
        }
    }

    private void Fleeing(Transform target)
    {
        Vector3 fleeingDir = transform.position - target.position;
        Vector3 point = transform.position + fleeingDir.normalized * fleeDistance;

        NavMeshHit hit;
        Vector3 fleeingPoint;
        if (NavMesh.SamplePosition(point, out hit, fleeDistance * 2, NavMesh.AllAreas))
        {
            //Prevent Corner Stuck
            if (Vector3.Distance(transform.position, hit.position) < 1)
            {
                Vector3 cornerDirection;

                //Calculate Corner
                if (transform.position.x > 0 && transform.position.z > 0)
                    cornerDirection = new Vector3(1, 0, 1);
                else if (transform.position.x > 0 && transform.position.z < 0)
                    cornerDirection = new Vector3(1, 0, -1);
                else if (transform.position.x < 0 && transform.position.z < 0)
                    cornerDirection = new Vector3(-1, 0, -1);
                else
                    cornerDirection = new Vector3(-1, 0, 1);

                if (Mathf.Abs(fleeingDir.x) < Mathf.Abs(fleeingDir.z))
                {
                    point = transform.position - new Vector3(cornerDirection.x * fleeDistance * 2, 0, 0);
                }

                else
                {
                    point = transform.position - new Vector3(0, 0, cornerDirection.z * fleeDistance * 2); 
                }

                if (!NavMesh.SamplePosition(point, out hit, fleeDistance * 2, NavMesh.AllAreas))
                {
                    fleeingPoint = transform.position;
                }
            }

            fleeingPoint = hit.position;
        }

        else
        {
            fleeingPoint = transform.position;
        }

        agent.isStopped = false;
        agent.speed = fleeSpeed;
        agent.velocity = Vector3.zero;
        Move(fleeingPoint);

        //Start Coroutine to check destination reach
        if (reachCoroutine != null)
            StopCoroutine(reachCoroutine);

        reachCoroutine = StartCoroutine(WaitUntilReach());

        KeepFleeing();
    }

    private void KeepFleeing()
    {
        if (fleeCoroutine != null)
            StopCoroutine(fleeCoroutine);
        fleeCoroutine = StartCoroutine(FleeingCountdown());

        fovColor.color = fleeingColor;
        fovColor.ApplyColor();

        nearbyViewColor.color = fleeingColor / 2;
        nearbyViewColor.ApplyColor();
    }

    private void BackToPatrol()
    {
        if (fleeCoroutine != null)
            StopCoroutine(fleeCoroutine);

        fleeTarget = null;
        agent.speed = normalSpeed;

        Move(NearbyWayPoint());
        state = State.Patrolling;
    }

    IEnumerator FleeingCountdown()
    {
        yield return new WaitForSeconds(fleeDuration);
        BackToPatrol();
    }

    //Variable Set
    public void Set_NormalSpeed(float speed)
    {
        normalSpeed = speed;

        if (state  != State.Fleeing)
            agent.speed = normalSpeed;
    }

    public void Set_FleeingSpeed(float speed)
    {
        fleeSpeed = speed;

        if (state == State.Fleeing)
            agent.speed = fleeSpeed;
    }

    public void Set_FleeingDuration(float duration)
    {
        fleeDuration = duration;
    }

    public void Set_ViewDistance(float viewDistance)
    {
        this.viewDistance = viewDistance;
    }
}
