using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public string opponentLayer;
    public string side;
    private Collider collider;
    public bool dirty = false; //dirty==true impiedica collider de a fi activ, previne hitting multiple targets with one blow

    private void Start()
    {
        collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer(opponentLayer))
            return;
        var animator = other.GetComponentInParent<Animator>();
        animator.Play("TakeHit.takeHit" + side);
        dirty = true;
        collider.enabled = false;
    }
}