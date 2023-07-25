using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum UIFloatMethods
{
    MouseSensitivity,
    CameraFov
}

public class UI_OptionBoxSlider : MonoBehaviour
{
    // The associated UI text displaying the value.Automatically searches for a child GameObject by the name "Value".
    public TextMeshProUGUI m_uiValueText;

    // The Slider UI object. Automatically finds the first Slider component.
    public Slider m_slider;

    public delegate void SetGameFloat(float _sensitivity);

    // What public GameManager method are we looking for to change its float value?
    // With I could serialize this in the inspector :(
    public SetGameFloat m_floatValueMethod;

    [SerializeField]
    UIFloatMethods m_methodType;

    [SerializeField]
    [Tooltip("How much to divide the UI text value, to simplify it for the user. Default = 1")]
    float m_simpleDivisor = 1;



    // Start is called before the first frame update
    void Start()
    {
        // Automatically grab the first Slider object--there should only be one.
        m_slider = GetComponentInChildren<Slider>();

        // Search for the UI text value object.
        // There are multiple, so search for the one attached to a GameObject named "Value".
        Transform nameSearchedObj = transform.Find("Value");

        // If found by the name, get its TextMeshPro component.
        if (nameSearchedObj != null)
            m_uiValueText = nameSearchedObj.GetComponent<TextMeshProUGUI>();

        // Assign the delegate based on what method type was supplied.
        switch (m_methodType)
        {
            case UIFloatMethods.MouseSensitivity:
                m_floatValueMethod = GameManager.SetMouseSensitivity;
                break;
        
            case UIFloatMethods.CameraFov:
                m_floatValueMethod = GameManager.SetCameraFOV;
                break;
        
            default:
                break;
        }

        m_slider.onValueChanged.AddListener(delegate { OnSliderValueChanged(); });
    }

    public void OnSliderValueChanged()
    {
        // Call the assigned method with the slider's value, if assigned.
        if (m_floatValueMethod != null)
            m_floatValueMethod(m_slider.value);

        // Set the UI value text to the given value, if assigned.
        if (m_uiValueText != null)
                m_uiValueText.text = ((int)(m_slider.value / m_simpleDivisor)).ToString();   // Cut off some zeroes to make it more readable to the user.
    }

    public void SetSliderValue(float _value)
    {
        if (m_slider != null)
            m_slider.value = _value;

        OnSliderValueChanged();
    }
}
