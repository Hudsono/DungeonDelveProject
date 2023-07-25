using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum RPGEntityTags
{

}

/// <summary>
/// Generic Entity class used to interact with the RPG environment.
/// </summary>
public class RPGEntity : MonoBehaviour
{
    [Tooltip("Meters associated with this RPGEntity")]
    public List<Meter> meters;

    // Start is called before the first frame update
    void Start()
    {
        // Reset all Meters to their initial values.
        for (int i = 0; i < meters.Count; i++)
            meters[i].ResetMeter();
    }

    // Update is called once per frame
    void Update()
    {
        // Update all meters.
        for (int i = 0; i < meters.Count; i++)
            meters[i].Update();
    }

    /// <summary>
    /// Get a named Meter from this RPG Entity.
    /// </summary>
    /// <param name="name">The name of the Meter to get.</param>
    /// <returns>The Meter of the given name.</returns>
    public Meter GetMeter(string name)
    {
        return meters.Find(i => i.meterName == name);
    }

    public void SpawnDamageNumber(Meter meter, DamageType damageType, int damage)
    {
        // Don't display damage numebrs when timed out.
        if (meter.UseTimeoutActive() && !damageType.ignoreCooldown)
            return;

        DamageNumber dm = Instantiate(RPGManager.instance.DamageNumberPrefab).GetComponent<DamageNumber>();
        dm.damageType = damageType;
        dm.damage = damage;
        dm.gameObject.transform.position = gameObject.transform.position;

        TMPro.VertexGradient grad;
        grad.topLeft = meter.meterColour;
        grad.topRight = meter.meterColour;
        grad.bottomLeft = new Color(meter.meterColour.r * 0.4f, meter.meterColour.g * 0.4f, meter.meterColour.b * 0.4f, meter.meterColour.a);
        grad.bottomRight = new Color(meter.meterColour.r * 0.4f, meter.meterColour.g * 0.4f, meter.meterColour.b * 0.4f, meter.meterColour.a);
        dm.textUI.colorGradient = grad;
    }

    /// <summary>
    /// Cause damage to this Entity.
    /// </summary>
    /// <param name="damageType">What type of damage was infliced.</param>
    /// <param name="direct">Whether this entity was damaged directly or via AoE.</param>
    public void Damage(DamageType damageType, bool direct, int aoeDamage = 0)
    {
        //Debug.Log("RPGEntity Damage!");

        // Check which meter to damage first--ignore if this entity doesn't have the requested meter.
        Meter damagedMeter = GetMeter(damageType.meterToDamage);
        if (damagedMeter == null)
            return;

        // Check if we should be dealing direct or AoE damage.
        // We leave direct and AoE damage for the Entity to calculate, should this logic be modified for different...
        // Entities, such as ones ignoring AoE damage, or dealing half damage, etc.
        if (direct)
        {
            SpawnDamageNumber(damagedMeter, damageType, damageType.damage);
            damagedMeter.AddValue(damageType.damage, damageType.ignoreCooldown);
        }
        else
        {
            SpawnDamageNumber(damagedMeter, damageType, aoeDamage);
            damagedMeter.AddValue(aoeDamage, damageType.ignoreCooldown);
        }
    }

    public void Knockback(DamageType damageType, bool direct, Vector3 kbDirection, float aoeKnockback = 0)
    {
        // First, test that the intended damage meter isn't timed out for /this/ Entity.
        Meter damagedMeter = GetMeter(damageType.meterToDamage);

        // Assume that not providing a Meter with defined knockback will affect the Entity regardless.
        // Useful for implementations purely used for Knockback without damage, e.g. a Strong Wind spell.
        
        // We can rely on the if-statement short-circuiting and thus never evaluating the possibly NULL Meter.
        // https://stackoverflow.com/questions/16711051/execution-order-of-conditions-in-c-sharp-if-statement
        if (damagedMeter != null && damagedMeter.UseTimeoutActive() && !damageType.ignoreCooldown)
        {
            // Exit early if the intended Meter is timed out and the Damage Type respects cooldown.
            return;
        }
        // Find all Rigidbody objects to knock back.
        Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();

        CharacterController chch = GetComponent<CharacterController>();
        CharacterMover cm = GetComponent<CharacterMover>();
        
        if (chch != null)
        {

            {
                //chch.Move(kbDirection * aoeKnockback);
            }
        }

        if (cm != null)
        {
            cm.ImpulseForce(kbDirection * aoeKnockback);
        }


        {
            //Debug.DrawRay(gameObject.transform.position, kbDirection, Color.green, 300);
            //Debug.Log("KB = " + gameObject.name);
        }

        // Check if we should be dealing direct or AoE damage.
        // We leave direct and AoE damage for the Entity to calculate, should this logic be modified for different
        // Entities, such as ones ignoring AoE damage, or dealing half damage, etc.
        if (direct)
        {
            for (int i = 0; i < rbs.Length; i++)
            {
                rbs[i].AddForce(kbDirection * damageType.knockback, ForceMode.Impulse);
                
            }
            //Debug.Log("Direct KB = " + damageType.knockback);
        }
        else
        {
            for (int i = 0; i < rbs.Length; i++)
            {
                rbs[i].AddForce(kbDirection * aoeKnockback, ForceMode.Impulse);
            }
            //Debug.Log("AoE KB = " + kbDirection * aoeKnockback);
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        ProjectileEmitter emitter = other.GetComponent<ProjectileEmitter>();
        if (emitter == null)
            return;

        emitter.damageType.DealDamage(gameObject);
    }
}
