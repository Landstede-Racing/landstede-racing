using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DamagablePart : MonoBehaviour
{
    public int maxDamage;
    public int currentDamage;
    public Part part;

    private void OnCollisionEnter()
    {
        Debug.Log("A bonk happened");
    }
}
