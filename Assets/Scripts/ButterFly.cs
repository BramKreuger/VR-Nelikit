using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ButterFly : MonoBehaviour
{
    [NonSerialized]
    public float restingTimeCurrent;
    [NonSerialized]
    public Vector3 target;
    [NonSerialized]
    public bool flower;
    [NonSerialized]
    public Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
}
