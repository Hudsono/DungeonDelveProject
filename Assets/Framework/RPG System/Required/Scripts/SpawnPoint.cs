using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + new Vector3(0, 1, 0), new Vector3(1, 2, 1));
        Gizmos.DrawRay(transform.position + new Vector3(0, 1, 0), transform.forward);
        Gizmos.DrawCube(transform.position - new Vector3(0, 0.005f, 0), new Vector3(1, 0.01f, 1));
    }
}
