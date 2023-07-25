using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item
{
    [Tooltip("List of colliders and an associated DamageType used for melee damage.")]
    public MeleeCollider[] meleeColliders;

    [Tooltip("List of ProjectileEmitters used for ranged damage using Particle Systems.")]
    public ProjectileEmitter[] projectileEmitters;

    private void Start()
    {
        // Automatically search for MeleeColliders.
        meleeColliders = GetComponentsInChildren<MeleeCollider>();

        projectileEmitters = GetComponentsInChildren<ProjectileEmitter>();
    }
}
