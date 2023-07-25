using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A custom Meter regeneration formula, that regenerates HP based on a character's movement. Can be used on any meter, though.
/// </summary>
public class ExampleHPRegen : MonoBehaviour
{
    [Tooltip("The entity's HP we want to provide custom regeneration to.")]
    public RPGCharacter character;

    [Tooltip("The movement system that gives us movement data.")]
    public CharacterController charController;

    [Tooltip("A Curve representing movement speed (X) with regeneration rate (Y).")]
    public AnimationCurve moveRegenCurve;

    [Tooltip("Name of the Meter to regenerate.")]
    public string meterName;

    // Meter to regenerate.
    private Meter meter;

    // Start is called before the first frame update
    void Start()
    {
        // Automatically find required elements.
        if (character == null)
            character = GetComponent<RPGCharacter>();

        if (charController == null)
            charController = GetComponent<CharacterController>();

        meter = character.GetMeter(meterName);
    }

    // Update is called once per frame
    void Update()
    {
        meter.buildRate = moveRegenCurve.Evaluate(charController.velocity.magnitude);
    }
}
