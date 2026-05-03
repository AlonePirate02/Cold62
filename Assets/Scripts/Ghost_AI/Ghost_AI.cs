using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ghost_AI : MonoBehaviour
{
    public float speed = 3f;
    public float detectionRange = 10f;
    public float roamRadius = 15f; 
    public float roamInterval = 3f; // Time between choosing new roam destinations
    public float waitAtDestination = 2f;
    public NavMeshAgent agent;
    public Transform player;

    private float roamTimer;
    private Vector3 roamDestination;
    public bool isChasingAllowed = true; // false when QTE is active
    private bool isWaiting = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        roamTimer = roamInterval;
        FindPlayer();
        SetNewRoamDestination();
    }

    private void Update()
    {
        if (!isChasingAllowed)
        {
            // Stop all movement when chasing is not allowed
            agent.ResetPath();
            return;
        }

        if (player == null)
        {
            FindPlayer();
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            if (!isWaiting && (roamTimer >= roamInterval || ReachedDestination()))
            {
                StartCoroutine(WaitBeforeRoaming());
            }
            else
            {
                roamTimer += Time.deltaTime;
            }
        }
    }

    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    private void SetNewRoamDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, roamRadius, NavMesh.AllAreas))
        {
            roamDestination = hit.position;
            agent.SetDestination(roamDestination);
        }
    }

    private bool ReachedDestination()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                return true;
        }
        return false;
    }

    private IEnumerator WaitBeforeRoaming()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitAtDestination);
        SetNewRoamDestination();
        roamTimer = 0f;
        isWaiting = false;
    }

    // Call when QTE starts
    public void StopChasing()
    {
        isChasingAllowed = false;
        agent.ResetPath(); // Immediately stop movement
    }


    // Needed for new GlobalVolumeManager to calculate distance to player
    public float DistanceToPlayer()
    {
        if (player == null) return Mathf.Infinity;
        return Vector3.Distance(transform.position, player.position);
    }
}
