using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Tooltip("To be applied to a GameObject containing any number of Colliders to deal damage.")]
public class MeleeCollider : MonoBehaviour
{
    [Header("Script deals the defined DamageType via Colliders defined directly on this GameObject.")]
    [Tooltip("How these Colliders deal damage.")]
    public DamageType damageType;

    // Colliders automatically found on this GameObject.
    private Collider[] m_colliders;

    [Tooltip("The RPGCharacter wielding this weapon.")]
    public RPGCharacter wielder;

    [SerializeField]
    [Tooltip("Show debug information related to this Collider and its DamageType.")]
    private bool debugView = false;

    private void Start()
    {
        // Automatically get all directly-attached Collider components.
        // Child components--and any other indirectly-attached Colliders--won't work as collision trigger events are done on this script only.
        m_colliders = GetComponents<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Try to call a Damage() function on the collided object.
        // If the object defines a way of being damaged, that will be handled.
        //other.gameObject.SendMessage("Damage", damageType, SendMessageOptions.DontRequireReceiver);

        damageType.DealDamage(other.gameObject, gameObject.transform.position);
    }

    // Draw the DamageType's properties.
    private void OnDrawGizmos()
    {
        if (damageType == null || !debugView)
            return;

        // DamageType radius.
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageType.aoeRadius);

        // AoE relative graph, represented by cubes; X position represents time, Y position represents value.
        // Red = Damage AoE key; Yellow = Damage AoE key interpolation.
        // Blue = Knockback AoE key; Cyan = Knockback AoE key interpolation.

        // DamageType damage Curve
        for (int i = 0; i < damageType.aoeDamageFalloff.length; i++)
        {
            Vector3 gizPos = new Vector3(transform.position.x + damageType.aoeDamageFalloff[i].time, transform.position.y + (damageType.aoeDamageFalloff[i].value / 2), transform.position.z);
            Vector3 gizSize = new Vector3(0.2f, damageType.aoeDamageFalloff[i].value, 0.2f);
            Gizmos.DrawCube(gizPos, gizSize);
        }

        // Intermediate Damage values.
        Gizmos.color = Color.yellow;
        float time = damageType.aoeDamageFalloff[0].time;
        float granularity = 0.05f;
        while (time < damageType.aoeDamageFalloff[damageType.aoeDamageFalloff.length - 1].time)
        {
            Vector3 gizPos = new Vector3(transform.position.x + time, transform.position.y + (damageType.aoeDamageFalloff.Evaluate(time) / 2), transform.position.z);
            Vector3 gizSize = new Vector3(0.05f, damageType.aoeDamageFalloff.Evaluate(time), 0.05f);
            Gizmos.DrawCube(gizPos, gizSize);
            time += granularity;
        }

        // DamageType knockback Curve
        Gizmos.color = Color.blue;
        for (int i = 0; i < damageType.aoeKnockbackFalloff.length; i++)
        {
            Vector3 gizPos = new Vector3(transform.position.x + damageType.aoeKnockbackFalloff[i].time, transform.position.y + (damageType.aoeKnockbackFalloff[i].value / 2), transform.position.z);
            Vector3 gizSize = new Vector3(0.02f, damageType.aoeKnockbackFalloff[i].value, 0.02f);
            Gizmos.DrawCube(gizPos, gizSize);
        }

        // Intermediate Knockback values.
        Gizmos.color = Color.cyan;
        time = damageType.aoeKnockbackFalloff[0].time;
        granularity = 0.05f;
        while (time < damageType.aoeKnockbackFalloff[damageType.aoeKnockbackFalloff.length - 1].time)
        {
            Vector3 gizPos = new Vector3(transform.position.x + time, transform.position.y + (damageType.aoeKnockbackFalloff.Evaluate(time) / 2), transform.position.z);
            Vector3 gizSize = new Vector3(0.015f, damageType.aoeKnockbackFalloff.Evaluate(time), 0.015f);
            Gizmos.DrawCube(gizPos, gizSize);
            time += granularity;
        }
    }
}
