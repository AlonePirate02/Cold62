using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleFollow : MonoBehaviour
{

    private GameObject player;

    public float followSpeed = 5f;

    private void Update()
    {
        player = GameObject.FindGameObjectWithTag("Player");


        if (player != null)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(player.transform.position.x, 
                player.transform.position.y + 15, player.transform.position.z), followSpeed * Time.deltaTime);
        }
    }
}
