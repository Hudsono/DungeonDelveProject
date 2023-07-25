using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGManager : MonoBehaviour
{
    /// <summary>
    /// Make this class a singleton; only one can exist.
    /// </summary>
    public static RPGManager instance;

    [Tooltip("The Prefab used to represent Damage Number UI objects.")]
    public GameObject DamageNumberPrefab;


    // Start is called before the first frame update
    void Start()
    {
        // Record this instance as a singleton.
        if (instance == null)
            instance = this;
        else
            Debug.LogError("Duplicate RPGMangers should not exist!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
