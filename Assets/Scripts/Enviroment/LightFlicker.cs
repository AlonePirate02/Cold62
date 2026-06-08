using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public GameObject[] lights;

    private void Start()
    {
        StartCoroutine(Flicker());
    }

    IEnumerator Flicker()
    {
        yield return new WaitForSeconds(Random.Range(0.05f, 0.4f));
        
        for(int i = 0; i < lights.Length; i++)
        {
            lights[i].SetActive(false);
        }

        yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));

        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].SetActive(true);
        }


        StartCoroutine(Flicker());
    }
}
