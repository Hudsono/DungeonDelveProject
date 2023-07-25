using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_PauseMenu : MonoBehaviour
{
    public UI_OptionBoxSlider mouseSensitivitySlider;
    public UI_OptionBoxSlider cameraFOVSlider;
    public Toggle invertYToggle;

    // Start is called before the first frame update
    void Start()
    {
        UpdateSettingsUI();
        GameManager.instance.SetPause(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnToggleValueChanged(Toggle _toggle)
    {
        GameManager.SetMouseInvert(_toggle.isOn);
    }

    public void UpdateSettingsUI()
    {
        mouseSensitivitySlider.SetSliderValue(GameManager.instance.settings.MouseSensitivity);
        cameraFOVSlider.SetSliderValue(GameManager.instance.settings.CameraFOV);
        invertYToggle.isOn = GameManager.instance.settings.InvertMouseY;
        Debug.Log("Read settings to menu");
    }
}
