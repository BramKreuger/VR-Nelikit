using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootSteps : MonoBehaviour
{
    AudioSource footStep;
    private void Start()
    {
        footStep = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "terrain")
        {
            Debug.Log("Step");
            footStep.Play();
        }
    }
}
