using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public enum MonsterState
{
    Patrolling,
    Chasing,
    Stopped
}

public class MageGhostAI : MonoBehaviourPunCallbacks
{
    public float patrolRadius = 20f;
    public float chaseDistance = 10f;
    public float stopDistance = 0.1f;
    private NavMeshAgent agent;
    private MonsterState currentState;
    private bool isPlayerVisible = false;
    Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (PhotonNetwork.IsMasterClient)
        {
            currentState = MonsterState.Patrolling;
            InvokeRepeating("CheckPlayerDistance", 0, 0.5f); // 일정 주기로 플레이어와의 거리를 확인
            SetRandomDestination();
        }
    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        switch (currentState)
        {
            case MonsterState.Patrolling:
                Patrol();
                break;

            case MonsterState.Chasing:
                ChasePlayer();
                break;

            case MonsterState.Stopped:
                Stop();
                break;
        }
    }

    void CheckPlayerDistance()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach(GameObject p in players)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, p.transform.position);

            if(distanceToPlayer > chaseDistance)
            {
                return;
            }
            else if(distanceToPlayer <= chaseDistance)
            {
                currentState = MonsterState.Chasing;
            }
            else if (currentState == MonsterState.Chasing && distanceToPlayer > chaseDistance)
            {
                currentState = MonsterState.Patrolling;
                SetRandomDestination();
            }
        }
    }

    void Patrol()
    {
        if (isPlayerVisible)
        {
            currentState = MonsterState.Stopped;
            agent.isStopped = true;
            return;
        }

        if (!agent.pathPending && agent.remainingDistance < 1f)
        {
            SetRandomDestination();
        }
    }

    void ChasePlayer()
    {
        if (isPlayerVisible)
        {
            currentState = MonsterState.Stopped;
            agent.isStopped = true;
            return;
        }

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject p in players)
        {
            if (p.transform == null) return;

            float distanceToPlayer = Vector3.Distance(transform.position, p.transform.position);

            if (distanceToPlayer > chaseDistance)
            {
                currentState = MonsterState.Patrolling;
                SetRandomDestination();
                return;
            }

            agent.destination = p.transform.position;
            agent.isStopped = false;

            if (distanceToPlayer <= stopDistance)
            {
                agent.isStopped = true;
            }
        }
    }

    void Stop()
    {
        if (!isPlayerVisible)
        {
            currentState = MonsterState.Patrolling;
            agent.isStopped = false;
            SetRandomDestination();
            animator.speed = 1f;
        }
        else
        {
            animator.speed = 0f;
        }
    }

    void SetRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1);
        Vector3 finalPosition = hit.position;
        agent.destination = finalPosition;
        agent.isStopped = false;
    }

    public void SetPlayerVisible(bool visible)
    {
        isPlayerVisible = visible;
        if (isPlayerVisible)
        {
            currentState = MonsterState.Stopped;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.GetComponent<Damageable>() != null)
        {
            other.GetComponent<Damageable>().PlayerDamaged(20f);
        }
    }
}