using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Tooltip("World UI. Applied to a Slider to track an RPG Entity's Meter.")]
public class WUI_Meter : MonoBehaviour
{
    [Tooltip("The Slider UI component to update.")]
    public Slider slider;

    [Tooltip("The RPG Entity with the Meter attached.")]
    public RPGEntity meteredEntity;

    [Tooltip("The name of the Meter to track.")]
    public string meterName;

    private Meter meter;

    //[Tooltip("The Meter to track.")]
    //public Meter meter;

    // Start is called before the first frame update
    void Start()
    {
        // Automatically find the Slider if not provided.
        if (slider == null)
            slider = GetComponent<Slider>();

        // Retrieve the Meter object.
        meter = meteredEntity.GetMeter(meterName);

        // Apply the Meter's properties to this Slider.
        slider.maxValue = meter.max;

        // Automatically set the Slider's fill rectangle Image colour to that of the Meter's.
        slider.fillRect.transform.GetComponent<Image>().color = meter.meterColour;
    }

    // Update is called once per frame
    void Update()
    {
        if (meter == null)
        {
            Debug.LogWarning("NO METER!");
            return;
        }
        // Update the slider's value to match.
        slider.value = meter.value;
    }
}
