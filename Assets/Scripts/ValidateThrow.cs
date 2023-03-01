using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValidateThrow : MonoBehaviour
{
    public PlayPhaseManager playPhaseManager;
    public GameObject telikit;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == telikit)
        {
            playPhaseManager.validThrows++;
            Debug.Log("Valid throw: " + playPhaseManager.validThrows);
        }
    }
}
