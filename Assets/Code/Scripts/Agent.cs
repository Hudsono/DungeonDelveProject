using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents something sending input into a Controller class; likely CharacterControllerCustom/EntityController.
/// </summary>
public class Agent : MonoBehaviour
{
    public Vector3 facingPosition;
    public bool isFacing;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual float Input_GetAxis(string axis)
    {
        Debug.Log("Base Agent Input_GetAxis(" + axis + ")");
        return 0;
    }

    public virtual bool Input_GetButton(string button)
    {
        Debug.Log("Base Agent Input_GetButton(" + button + ")");
        return true;
    }

    public virtual bool Input_GetButtonDown(string button)
    {
        Debug.Log("Base Agent Input_GetButtonDown(" + button + ")");
        return false;
    }

    public virtual bool Input_GetButtonUp(string button)
    {
        Debug.Log("Base Agent Input_GetButtonUp(" + button + ")");
        return false;
    }
}
