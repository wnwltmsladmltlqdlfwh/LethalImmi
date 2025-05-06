using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

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
            InvokeRepeating("CheckPlayerDistance", 0, 0.5f); // ���� �ֱ�� �÷��̾���� �Ÿ��� Ȯ��
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

        if (agent.isOnNavMesh && !agent.pathPending && agent.remainingDistance < 1f)
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

            if (agent.isOnNavMesh)
            {
                agent.destination = p.transform.position;
                agent.isStopped = false;

                if (distanceToPlayer <= stopDistance)
                {
                    agent.isStopped = true;
                }
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
			agent.isStopped = true;
			animator.speed = 0f;
        }
    }

    void SetRandomDestination()
    {
        if (agent.isOnNavMesh)
        {
            animator.SetBool("isMove", true);
            Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1);
            Vector3 finalPosition = hit.position;
            agent.destination = finalPosition;
            agent.isStopped = false;
        }
    }

    [PunRPC]
    public void SetPlayerVisible(bool visible)
    {
        isPlayerVisible = visible;
        if (isPlayerVisible)
        {
            currentState = MonsterState.Stopped;
			agent.isStopped = true;
			animator.speed = 0f;
		}
        else
        {
			currentState = MonsterState.Patrolling;
			agent.isStopped = false;
			animator.speed = 1f;
		}
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            if(other.GetComponent<Damageable>() != null)
            {
                other.GetComponent<Damageable>().GetComponent<PhotonView>().RPC("PlayerDamaged", RpcTarget.AllBuffered, 20f);
            }
        }
    }
}