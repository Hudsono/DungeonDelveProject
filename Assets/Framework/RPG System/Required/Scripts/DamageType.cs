using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Elements used for status effects/damage.
/// </summary>
public enum Elements
{
    NORMAL,
    FIRE,
    ICE,
    WATER,
    ELECTRIC,
    POISON
}

[CreateAssetMenu(fileName = "New Damage Type.asset", menuName = "RPG Data/Damage Type")]
public class DamageType : ScriptableObject
{
    [Tooltip("What meter should this deal damage to?")]
    public string meterToDamage = "HP";

    [Tooltip("How much damage this deals directly.")]
    public int damage = 10;

    [Tooltip("What element this damage is inflicted with.")]
    public Elements element = Elements.NORMAL;

    [Tooltip("Area-of-Effect radius size.")]
    public float aoeRadius = 0;

    [Tooltip("How much force to apply to directly-impacted objects.")]
    public float knockback = 5;

    [Tooltip("How damage multiplies based on distance from the AoE.")]
    public AnimationCurve aoeDamageFalloff;

    [Tooltip("How knockback multiplies based on distance from the AoE.")]
    public AnimationCurve aoeKnockbackFalloff;

    [Tooltip("In plain English via death messages, describes how a character died to this Damage Type.")]
    public string killMessage;

    [Tooltip("Whether this damage ignores damage cooldowns/\"invincibility frames\"")]
    public bool ignoreCooldown = false;

    [Tooltip("Should this damage be able to harm the user?")]
    public bool harmSelf = false;

    [Tooltip("Should this damage be able to knock the user back?")]
    public bool knockbackSelf = false;

    // Deal damage to the specified target.
    public void DealDamage(GameObject target, Vector3? source = null)
    {
        //Debug.Log("Damage dealt!");

        // Get a list of objects within the Area-of-Effect radius of this damage type.
        Collider[] overlapping = Physics.OverlapSphere(target.transform.position, aoeRadius);

        // DO NOT want to damage the same entity TWICE!
        // -- E.g.: a character has 20 Colliders used for fine collision detection/ragdoll physics.
        // We don't want to damage the same character 20 times for this arbitrary technicality!

        // So, record a list of unique RPGEntities (HashSet) and only damage if the next Entity is new to this HashSet.
        HashSet<RPGEntity> damagedEntities = new HashSet<RPGEntity>();

        // DIRECT DAMAGE.
        RPGEntity targetEnt = target.GetComponent<RPGEntity>();
        //Debug.Log(target.name);
        if (targetEnt != false)
        {
            //Debug.Log("DamageType: Direct");

            // Perform Knockback first as actual damage incurs a Meter use timeout which Knockback checks.
            // Knockback the target based on whether the damage source was supplied or not. 
            // No damage source means the knockback direction is simply behind the target object.
            if (source != null)
                targetEnt.Knockback(this, true, Vector3.Normalize((Vector3)(target.transform.position - source)));
            else
                targetEnt.Knockback(this, true, -target.transform.forward);

            targetEnt.Damage(this, true);

            damagedEntities.Add(targetEnt);
        }
        
        // AOE DAMAGE
        //SpawnDebugAoe(target.transform.position);

        for (int i = 0; i < overlapping.Length; i++)
        {
            // If there's no entity to damage, or we already damaged the entity, ignore.
            RPGEntity curEnt = overlapping[i].gameObject.GetComponent<RPGEntity>();
            if (curEnt == null)
                continue;
            if (damagedEntities.Contains(curEnt))
                continue;

            //Debug.Log(curEnt.gameObject.name);
            //Debug.Log("DamageType: AoE");

            // Get the distance between the target object and any overlapping objects.
            float dist = Vector3.Distance(target.transform.position, overlapping[i].transform.position);


            //overlapping[i].SendMessage("Damage", aoeFalloff.Evaluate(dist), SendMessageOptions.DontRequireReceiver);

            // Damage via AoE's falloff distance.
            float damageToDeal = damage * aoeDamageFalloff.Evaluate(dist);

            //curEnt.Damage(this, false, damage * (int)aoeDamageFalloff.Evaluate(dist * (aoeDamageFalloff[aoeDamageFalloff.length - 1].time / aoeRadius)));
            curEnt.Damage(this, false, (int)damageToDeal);

            float knockbackToDeal = knockback * aoeKnockbackFalloff.Evaluate(dist);

            // Knockback the target based on whether the damage source was supplied or not.
            // No damage source means the knockback direction is simply behind the target object.
            if (source != null)
                curEnt.Knockback(this, false, Vector3.Normalize((Vector3)(curEnt.transform.position - source)), knockbackToDeal);
            else
                curEnt.Knockback(this, false, -curEnt.transform.forward, knockback);

            //Debug.LogWarning(knockbackToDeal);
            //Debug.DrawLine(target.transform.position, curEnt.transform.position, Color.red, 300);

            // Record the damaged entity to avoid damaging it again in the same function call.
            damagedEntities.Add(curEnt);
        }
    }

    /// <summary>
    /// DEBUG: Spawn a translucent sphere representing an AoE.
    /// </summary>
    /// <param name="position">Position to spawn the sphere.</param>
    public void SpawnDebugAoe(Vector3 position)
    {
        // Create a new basic sphere GameObject.
        GameObject debugS = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        debugS.GetComponent<Collider>().enabled = false;    // Disable the default Collider.
        debugS.transform.position = position;   // Set sphere's position to the given position.
        debugS.transform.localScale = new Vector3(aoeRadius, aoeRadius, aoeRadius); // Set sphere's size to the AoE's radius.
        debugS.GetComponent<Renderer>().material = Resources.Load("DebugTransparent", typeof(Material)) as Material;    // Apply the debug translucent material.
    }
}