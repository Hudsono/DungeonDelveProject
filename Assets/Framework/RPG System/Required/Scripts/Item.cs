using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

/// <summary>
/// Types of items; what purposes they serve.
/// </summary>
public enum ItemTypes
{
    AMMO,           // If this item can be used as ammunition for something else.
    INGREDIENT,     // If this item is used to craft something else.
    QUEST,          // If this item is necessary for a quest.
    WEAPON,         // If this item is intended to be used as, or is, a weapon.
    TOOL,           // If this item is intended to be used for interactive purposes other than damage.
    ARMOUR,         // If this item can be worn for protection.
    ACCESSORY,      // If this item can be worn for other reasons, like cosmetic hats or enchanted jewelry.
    KEY             // If this item is used to open something, like a key, keycard, Indiana Jones artefact, etc.
}

/// <summary>
/// Types of inputs that can be performed.
/// </summary>
public enum InputTypes
{
    DOWN,   // GetKeyDown. True for the first frame of the input being pressed.
    HELD,   // GetKey. True while the input is pressed.
    UP      // GetKeyUp. True for the first frame of the input being released.
}

[System.Serializable]
public struct ControlInput
{
    [Tooltip("What input name does this control respond to?")]
    public string axisName;

    [Tooltip("How does this input behave? When pressed normally, held down, or released, respectively?")]
    public InputTypes inputType;

    [Tooltip("What function does this input serve?")]
    public UnityEvent inputEvent;
}

public class Item : MonoBehaviour
{
    [Tooltip("The name of this item.")]
    public string itemName;

    [Tooltip("What this item does.")]
    public string itemDescription;

    [Tooltip("The interactive controls this item posesses.")]
    public List<ControlInput> controlInputs;

    [Tooltip("The image shown in inventories.")]
    public Sprite previewImage;

    [Tooltip("The purposes this item serves.")]
    public List<ItemTypes> itemTypes;

    [Tooltip("In inventories, how many of these can be stacked on one-another in the same slot?")]
    public int maxStack = 1;

    [Tooltip("The Animator used to control this item's states.")]
    public Animator animator;

    [Tooltip("If this item is currently player-controlled.")]
    public bool controlled;

    [Tooltip("What Agent is currently in control of this weapon?")]
    public Agent agent;

    [Tooltip("Fire a trigger in the attached Animator of the given name.")]
    public void AnimatorFireTrigger(string parameterName)
    {
        animator.SetTrigger(parameterName);
    }

    //public void AnimatorSetFloat(string parameterName)
    //{
    //    animator.SetFloat(parameterName, maxStack);
    //}

    public void AnimatorSetBoolOn(string parameterName)
    {
        animator.SetBool(parameterName, true);
    }

    public void AnimatorSetBoolOff(string parameterName)
    {
        animator.SetBool(parameterName, false);
    }

    private void Update()
    {
        // Ignore control checks if not being controlled.
        if (agent == null)
            return;

        // Check all controls and enact them.
        for (int i = 0; i < controlInputs.Count; i++)
        {
            bool isInputTrue = false;

            switch (controlInputs[i].inputType)
            {
                case InputTypes.DOWN:
                    isInputTrue = agent.Input_GetButtonDown(controlInputs[i].axisName);
                    break;

                case InputTypes.HELD:
                    isInputTrue = agent.Input_GetButton(controlInputs[i].axisName);
                    break;

                case InputTypes.UP:
                    isInputTrue = agent.Input_GetButtonUp(controlInputs[i].axisName);
                    break;
            }

            if (isInputTrue)
                controlInputs[i].inputEvent.Invoke();
        }
    }
}