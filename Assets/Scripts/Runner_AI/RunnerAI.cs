using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class RunnerAI : MonoBehaviour
{
    private enum AIState { Roaming, Chasing, Attacking }
    [SerializeField] private AIState currentState = AIState.Roaming;

    [Header("Movement & Speeds")]
    [SerializeField] private float roamSpeed = 3.5f;
    [SerializeField] private float chaseSpeed = 6f;
    [SerializeField] private float roamRadius = 15f; // Random roam radius

    [Header("Ranges")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 2f;

    [Header("Combat")]
    [SerializeField] private float damageAmount = 15f;
    [SerializeField] private float attackCooldown = 1.5f;
    private float nextAttackTime = 0f;
    [SerializeField] private Animator anim;

    // Referanslar
    private NavMeshAgent agent;
    private Transform playerTransform;
    private PlayerHealth playerHealth;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            playerHealth = playerObj.GetComponent<PlayerHealth>();
        }
        else
        {
            Debug.LogError("RunnerAI: Sahnede 'Player' tagýna sahip bir obje bulunamadý!");
        }

        agent.speed = roamSpeed;
        SetRandomRoamDestination();
    }

    private void Update()
    {
        if (playerTransform == null || playerHealth == null) return;

        if (playerHealth.currentHealth <= 0)
        {
            if (currentState != AIState.Roaming)
            {
                currentState = AIState.Roaming;
                agent.speed = roamSpeed;
                SetRandomRoamDestination();
            }
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // State Machine
        switch (currentState)
        {
            case AIState.Roaming:
                HandleRoaming(distanceToPlayer);
                break;
            case AIState.Chasing:
                HandleChasing(distanceToPlayer);
                break;
            case AIState.Attacking:
                HandleAttacking(distanceToPlayer);
                break;
        }
    }

    // --- STATE BEHAVIOURS ---

    private void HandleRoaming(float distanceToPlayer)
    {
        // Player in range
        if (distanceToPlayer <= detectionRange)
        {
            currentState = AIState.Chasing;
            agent.speed = chaseSpeed;
            return;
        }

        // Choose a random path
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            SetRandomRoamDestination();
        }
    }

    private void HandleChasing(float distanceToPlayer)
    {
        // Stop chasing if the player is out of detection range
        if (distanceToPlayer > detectionRange)
        {
            currentState = AIState.Roaming;
            agent.speed = roamSpeed;
            SetRandomRoamDestination();
            return;
        }

        // Attack
        if (distanceToPlayer <= attackRange)
        {
            currentState = AIState.Attacking;
            agent.ResetPath(); // Stop to attack (optional)
            return;
        }

        // Follow plyr
        agent.SetDestination(playerTransform.position);
    }

    private void HandleAttacking(float distanceToPlayer)
    {
        // Follow if player out of range
        if (distanceToPlayer > attackRange)
        {
            currentState = AIState.Chasing;
            return;
        }

        // Look at the player
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        direction.y = 0; // Stop Y rotation
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 10f);
        }

        // Cooldown
        if (Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    // --- OTHER ---

    private void Attack()
    {
        Debug.Log("RunnerAI saldýrdý!");
        anim.SetTrigger("Attack");
        StartCoroutine(waitabit());
    }

    private void SetRandomRoamDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, roamRadius, 1))
        {
            agent.SetDestination(hit.position);
        }
    }

    private IEnumerator waitabit()
    {
        yield return new WaitForSeconds(0.3f);
        playerHealth.TakeDamage(damageAmount);
    }

    // See ranges in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}