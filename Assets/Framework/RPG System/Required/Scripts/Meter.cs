using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class Meter
{
    public string meterName;           // Name of this meter.
    public string meterDescription;    // Description of this meter.
    public Color meterColour = Color.red;            // Colour of the Meter, to be used elsewhere.
    public float value = 1;                // What value this meter is currently at.
    public float max = 1;                  // The maximum value this meter can be at.
    public float initial = 1;               // The value for the meter to begin with.
    public bool initialPercentage = false;  // Treat the Initial value as a percentage of the Max value.
    public float buildTo = 1;               // The value to build up to.
    public bool buildToPercentage = false;  // Treat the BuildTo value as a percentage of the Max value.

    [Tooltip("The rate this meter builds (value/second).")]
    public float buildRate;

    [Tooltip("How long after being used should the meter start building (in seconds).")]
    public float buildTimeout;
    private float buildTimeoutCurrent;   // The current timeout value.

    [Tooltip("How long after being used can this meter be used again.")]
    public float useTimeout;
    private float useTimeoutCurrent;    // The current use timeout value.

    [Tooltip("Inverting timeout value functionality.")]
    public bool invert;

    [Tooltip("Called when the meter has been emptied.")]
    public UnityEvent onEmptied;

    [Tooltip("Called when the meter has been filled.")]
    public UnityEvent onFilled;

    [Tooltip("Called when the meter has been used.")]
    public UnityEvent onUsed;

    [Tooltip("Called when the meter has been externally added to rather than used.")]
    public UnityEvent onAdd;

    /// <summary>
    /// Add to the Meter's current value.
    /// </summary>
    /// <param name="value"></param>
    public void AddValue(float value, bool ignoreCooldown = false)
    {
        SetValue(this.value + value, ignoreCooldown);
    }

    /// <summary>
    /// Set the meter's value.
    /// </summary>
    /// <param name="value"></param>
    public void SetValue(float value, bool ignoreCooldown = false)
    {
        // If we're decrementing (or incrementing if inverted) the meter, check use and build timeout values.
        if ((value < this.value && !invert) || (value > this.value && invert))
        {
            // If use timeout is active and damage type does not ignore the cooldown, ignore this attempted value change.
            if (useTimeoutCurrent > 0 && !ignoreCooldown)
                return;

            // Reset the use and build timeouts.
            useTimeoutCurrent = useTimeout;
            buildTimeoutCurrent = buildTimeout;

            if (value < this.value && !invert)
                onUsed?.Invoke();   // We successfully used the meter, invoke the Event.
            else
                onAdd?.Invoke();    // We successfully added to the meter, 
        }

        // Clamp the new value between the minimum and maximum.
        this.value = Mathf.Clamp(value, 0, max);

        // If we've fillted the meter, call the onFilled delegate.
        // Else if we've emptied the meter, call the onEmptied delegate.
        if (IsFull())
            onFilled?.Invoke(); // The ? checks if the delegate was assigned before attempting to invoke it.
        else if (IsEmpty())
            onEmptied?.Invoke();
    }

    /// <summary>
    /// Updates timeout and build values.
    /// </summary>
    public void Update()
    {
        // Count down the build timeout if it's been set.
        if (buildTimeoutCurrent > 0)
            buildTimeoutCurrent -= Time.deltaTime;

        // Count down the use timeout if it's been set.
        if (useTimeoutCurrent > 0)
            useTimeoutCurrent -= Time.deltaTime;

        // Increment (or decrement if inverted) the meter over time if we're building and not full.
        if (!invert)
        {
            if (buildTimeoutCurrent <= 0 && value < PercCheck(buildTo, buildToPercentage))
                AddValue(Mathf.Clamp(buildRate * Time.deltaTime, 0, PercCheck(buildTo, buildToPercentage)));
        }
        else
        {
            if (buildTimeoutCurrent <= 0 && value > PercCheck(buildTo, buildToPercentage))
                AddValue(Mathf.Clamp(buildRate * Time.deltaTime, PercCheck(buildTo, buildToPercentage), max));
        }

    }

    /// <summary>
    /// Is the meter's value at max?
    /// </summary>
    /// <returns></returns>
    public bool IsFull()
    {
        return value >= max;
    }

    /// <summary>
    /// Is the meter's value at 0?
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty()
    {
        return value <= 0;
    }

    /// <summary>
    /// Reset the meter's values.
    /// </summary>
    public void ResetMeter()
    {
        // Apply the Initial value as a percentage of Max if enabled.
        value = PercCheck(initial, initialPercentage);
        buildTimeoutCurrent = 0;
        useTimeoutCurrent = 0;
    }

    /// <summary>
    /// Returns whether this meter's usage timeout is active.
    /// </summary>
    /// <returns></returns>
    public bool UseTimeoutActive()
    {
        return useTimeoutCurrent > 0;
    }

    /// <summary>
    /// Check if this value is optionally treated as a percentage or not based on Max.
    /// </summary>
    /// <param name="inValue">The value to check.</param>
    /// <param name="isPercentage">Is this value a percentage?</param>
    /// <returns>Either the direct value or the percentage of Max.</returns>
    private float PercCheck(float inValue, bool isPercentage)
    {
        return isPercentage ? (inValue / 100f) * max : inValue;
    }
}