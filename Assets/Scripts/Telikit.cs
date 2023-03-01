using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
/// <summary>
/// This scripts holds all the variables and objects related to the Telikit. Easily accessible from other scripts.
/// </summary>
public class Telikit : MonoBehaviour
{
    [System.NonSerialized]
    public Rigidbody rigid;
    [System.NonSerialized]
    public TrailRenderer trail;
    [System.NonSerialized]
    public ParentConstraint constraint;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();
        constraint = GetComponent<ParentConstraint>();
    }
}