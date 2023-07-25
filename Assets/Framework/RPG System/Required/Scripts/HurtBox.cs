using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    public float meterAmount;  // TODO: Will be replaced with DamageType ScriptableObject instead.

    private Collider m_collider;    // The collider used to detect a character to harm.

    public string targetMeterName;  // What's the name of the meter we want to affect?

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log("H");
        RPGCharacter character = other.GetComponent<RPGCharacter>();
        if (character == null)
            return;

        //Debug.Log("A");

        Meter hpMeter = character.GetMeter(targetMeterName);
        if (hpMeter != null)
            hpMeter.AddValue(meterAmount);

        RPGStat targetStat = character.GetStat(targetMeterName);
        if (targetStat != null)
            targetStat.AddXP((int)meterAmount);

        //Debug.Log("D");
    }

    // Start is called before the first frame update
    void Start()
    {
        m_collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
