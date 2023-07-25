using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RPGCharacter : RPGEntity
{

    public List<RPGStat> statistics;

    //[Tooltip("Transforms used to attach equipment to. Transform GameObject's name is used in the lookup process.")]
    //public List<Transform> equipSlots;


    public RPGStat GetStat(string name)
    {
        return statistics.Find(i => i.statisticName == name);
    }

    /// <summary>
    /// Respawns the character
    /// </summary>
    public void RespawnCharacter()
    {
        // Reset all meters' values.
        for (int i = 0; i < meters.Count; i++)
        {
            meters[i].ResetMeter();
        }
    }
}
