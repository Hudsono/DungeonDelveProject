using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AgentInput
{

}
public class PlayerAgent : Agent
{
    public override float Input_GetAxis(string axis)
    {
        return GameManager.instance.Input_GetAxis(axis);
    }

    public override bool Input_GetButton(string button)
    {
        return GameManager.instance.Input_GetButton(button);
    }

    public override bool Input_GetButtonDown(string button)
    {
        return GameManager.instance.Input_GetButtonDown(button);
    }

    public override bool Input_GetButtonUp(string button)
    {
        return GameManager.instance.Input_GetButtonUp(button);
    }
}
