using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyQTETrigger : MonoBehaviour
{
    //  References
    private Transform player;
    private QuickTimeEvent qte;
    private Ghost_AI ghostAI;

    // Settings
    public float triggerDistance = 10f;
    private bool triggered = false;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        qte = GetComponent<QuickTimeEvent>();
        ghostAI = GetComponent<Ghost_AI>();
    }

    void Update()
    {
        if (triggered) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance < triggerDistance)
        {
            TriggerQTE();
        }
    }

    void TriggerQTE()
    {
        triggered = true;
        ghostAI.StopChasing();
        qte.enabled = true;
    }
}