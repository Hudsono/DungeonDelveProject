using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

using System.Runtime.InteropServices;   // For DLL importing; for mouse positioning.
using UnityEngine.Animations;   // For Constraints.

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Make this class a singleton; only one can exist.
    /// </summary>
    public static GameManager instance;

    /// <summary>
    /// Whether the game is paused.
    /// </summary>
    public bool isPaused = false;

    //public bool invertMouse = false;
    //public float mouseSensitivity = 1500;

    [Tooltip("The Pause menu UI")]
    public GameObject pauseUI;

    [Tooltip("The Settings prefab object containing the Settings script.")]
    public GameObject settingsPrefab;

    [Tooltip("The Settings prefab script, saved to disk.")]
    public Settings settings;

    [Tooltip("The Default Settings prefab object containing the default Settings script.")]
    public Settings defaultSettings;

    // Records whether settings were changed while paused.
    bool SettingsChanged = false;

    [Tooltip("Where the player will spawn.")]
    public SpawnPoint spawnPoint;

    [Tooltip("List of controllable characters in the scene. Automatically filled.")]
    public List<CharacterMover> characters;

    [Tooltip("The index of the currently-controlled character in the lsit of controllable characers.")]
    public int characterIndex = 0;

    [Tooltip("The Character the player is currently controlling.")]
    public CharacterMover controlledCharacter;

    [Tooltip("The object containing a Constraint used to track the player with the main Camera.")]
    public PositionConstraint cameraConstraint;

    [Tooltip("Saved cursor position the last time it was unlocked. Makes going in-and-out of menus less of a pain; not always resetting in the centre.")]
    public Vector2 lastCursorPosition;

    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int X, int Y);

    [Tooltip("The GameObject in which Billboard UI objects should aim at.")]
    public GameObject cameraUIAim;

    public bool isFreeAiming = false;

    // Start is called before the first frame update
    void Start()
    {
        // Record this instance as a singleton.
        if (instance == null)
            instance = this;
        else
            Debug.LogError("Duplicate GameManagers should not exist!");

        // Hide the pause menu.
        //pauseUI.SetActive(false);
        //SetPause(false);

        // Get the Settings script off the prefab.
        settings = settingsPrefab.GetComponent<Settings>();
        ReadSettings();
    }

    // Update is called once per frame
    void Update()
    {
        // Swap characters.
        if (Input_GetButtonDown("Swap Character"))
        {
            SwapCharacter(characterIndex + (int)Input_GetAxis("Swap Character"));
        }

        // Open/Close the Spawn Menu.
        if (Input_GetButton("Free Aim") && !isPaused)
        {
            SetCursorLocked(false);
        }
        else if (!Input_GetButton("Free Aim") && !isPaused)
        {
            SetCursorLocked(true);
        }

        isFreeAiming = Input_GetButton("Free Aim");

        // Move camera aim while Free Aiming.
        //if (isFreeAiming)
        //{
        //    cameraAim.transform.localPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraAim.transform.localPosition.z);
        //}
        //else
        //{
        //    cameraAim.transform.localPosition = new Vector3(0, 0, cameraAim.transform.localPosition.z);
        //}


        // Pause/Unpause the game.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetPause(!IsPaused());
        }
    }

    public void SetPause(bool _pause)
    {
        isPaused = _pause;

        pauseUI.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;

        // Lock/Unlock the cursor.
        SetCursorLocked(!isPaused);

        // Write settings when un-pausing
        if (!isPaused)
        {
            WriteSettings();
        }
    }

    public bool IsPaused() { return isPaused; }

    public void QuitGame()
    {
        Debug.Log("Quitting the game!");
        Application.Quit();
    }

    public static void SetMouseSensitivity(float _sensitivity)
    {
        //CameraController camControl = Camera.main.gameObject.GetComponent<CameraController>();
        //camControl.xSpeed = _sensitivity;
        //camControl.ySpeed = _sensitivity;
        //instance.SettingsChanged = true;
    }

    public static void SetMouseInvert(bool _invert)
    {
        //Camera.main.gameObject.GetComponent<CameraController>().yInvert = _invert;
        //instance.SettingsChanged = true;
    }

    public static void SetCameraFOV(float _fov)
    {
        //Camera.main.fieldOfView = _fov;
        //instance.SettingsChanged = true;
    }

    public float Input_GetAxis(string _axisName) { return isPaused ? 0 : Input.GetAxis(_axisName); }
    public bool Input_GetButton(string _buttonName) { return isPaused ? false : Input.GetButton(_buttonName); }
    public bool Input_GetButtonDown(string _buttonName) { return isPaused ? false : Input.GetButtonDown(_buttonName); }
    public bool Input_GetButtonUp(string _buttonName) { return isPaused ? false : Input.GetButtonUp(_buttonName); }

    public void LoadScene(string _sceneName)
    {
        SetPause(false);    // Un-Pause the game first.
        SceneManager.LoadScene(_sceneName, LoadSceneMode.Single);
    }

    /// <summary>
    /// Save changes to the game settings in the assigned prefab.
    /// </summary>
    void WriteSettings()
    {
        // Only write if anything was changed, and that the settings prefab exists.
        if (settings != null && SettingsChanged)
        {
            SettingsChanged = false;

            settings.InvertMouseY = Camera.main.gameObject.GetComponent<CameraController>().yInvert;
            settings.CameraFOV = Camera.main.fieldOfView;
            settings.MouseSensitivity = Camera.main.gameObject.GetComponent<CameraController>().xSpeed;

            Debug.Log("Wrote settings to prefab!");
        }
    }

    void ReadSettings()
    {
        // Only read if the prefab component exists.
        if (settings != null)
        {
            SetCameraFOV(settings.CameraFOV);
            SetMouseInvert(settings.InvertMouseY);
            SetMouseSensitivity(settings.MouseSensitivity);
            SettingsChanged = false;    // We just read the settings into the game; didn't change them, don't need to write!

            Debug.Log("Read settings from prefab!");
        }
    }

    public void ResetSettings()
    {
        if (defaultSettings != null)
        {
            SetCameraFOV(defaultSettings.CameraFOV);
            SetMouseInvert(defaultSettings.InvertMouseY);
            SetMouseSensitivity(defaultSettings.MouseSensitivity);

            UI_PauseMenu pauseMenu = pauseUI.GetComponent<UI_PauseMenu>();
            pauseMenu.UpdateSettingsUI();
        }
    }

    /// <summary>
    /// Respawns the currently-controlled character to the referenced spawn point.
    /// </summary>
    public void RespawnCharacter(CharacterMover cm)
    {
        // Check if a Spawn Point was supplied--teleport them there if so, otherwise teleport them at the world origin.
        cm.setPosition(spawnPoint != null ? spawnPoint.transform.position : new Vector3(0, 0, 0));
    }

    /// <summary>
    /// Swaps the currently-controlled character to another.
    /// </summary>
    public void SwapCharacter(int _characterIndex)
    {
        CharacterMover thisCm = characters[characterIndex];
        thisCm.isControlled = false;
        Item[] items = thisCm.GetComponentsInChildren<Item>();
        for (int i = 0; i < items.Length; i++)
            items[i].controlled = false;


        characterIndex = _characterIndex;
        if (characterIndex > characters.Count - 1)
            characterIndex = 0;
        if (characterIndex < 0)
            characterIndex = characters.Count - 1;

        //CameraController camControl = Camera.main.GetComponent<CameraController>();

        // Update the main Camera's constraint to the newly-controlled character.
        ConstraintSource cs = new ConstraintSource();
        cs.sourceTransform = characters[characterIndex].transform;
        cs.weight = 1;
        cameraConstraint.SetSource(0, cs);

        thisCm = characters[characterIndex];
        thisCm.isControlled = true;
        items = thisCm.GetComponentsInChildren<Item>();
        for (int i = 0; i < items.Length; i++)
            items[i].controlled = true;
    }

    public void AddCharacter(CharacterMover _character)
    {
        // Record the given character.
        characters.Add(_character);

        // If this is the first controllable characer added, swap to it.
        if (characters.Count == 1)
            SwapCharacter(0);
    }

    /// <summary>
    /// Instantiates the given object where the player is looking.
    /// </summary>
    /// <param name="_object">The object to spawn.</param>
    public void SpawnObject(GameObject _object, Vector3 _rotationOffset)
    {
        RaycastHit hit;

        // If the player was aiming at something collideable, spawn the object.
        if (Physics.Raycast(Camera.main.transform.position + (Camera.main.transform.forward * 2), Camera.main.transform.forward, out hit))
        {
            Renderer objRenderer = _object.GetComponent<Renderer>();
            Vector3 positionOffset = new Vector3();

            if (objRenderer != null)
                positionOffset.y = objRenderer.bounds.extents.y + 1;

            Vector3 objRotation = Camera.main.transform.rotation.eulerAngles;
            objRotation.x = _rotationOffset.x;
            objRotation.y += _rotationOffset.y;
            objRotation.z += _rotationOffset.z;
            //objRotation.Normalize();
            GameObject newObject = Instantiate(_object, hit.point + positionOffset, Quaternion.Euler(hit.normal));

            newObject.transform.rotation = Quaternion.Euler(objRotation);
        }
    }


    /// <summary>
    /// Set the cursor to be invisible & locked, or visible and unlocked.
    /// </summary>
    /// <param name="_locked">Whether the cursor should be locked.</param>
    public void SetCursorLocked(bool _locked)
    {
        // Ignore if the cursor's already in the requested state.
        if (IsCursorLocked() == _locked)
            return;

        //int fakeWidth = Screen.width;
        //int realWidht = (int)EditorWindow.focusedWindow.position.width;
        //int fakeHeight = Screen.height;
        //int realHeight = (int)EditorWindow.focusedWindow.position.height;
        //
        //float heightDeltaRatio = fakeHeight / realHeight;
        //
        //// If we're locking the cursor, save its position before locking it.
        //if (_locked)
        //{
        //    lastCursorPosition = new Vector2((int)Input.mousePosition.x, realHeight - ((int)Input.mousePosition.y * heightDeltaRatio));
        //    Debug.Log("SAVED: " + lastCursorPosition);
        //}

        Cursor.lockState = _locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !_locked;

        // If we're unlocking the cursor, restore its position after it being unlocked.
        //if (!_locked)
        //{
        //    SetCursorPos((int)lastCursorPosition.x, (int)lastCursorPosition.y);
        //    Debug.Log("LOADED: " + lastCursorPosition);
        //}
    }

    /// <summary>
    /// Is the cursor currently locked?
    /// </summary>
    public bool IsCursorLocked()
    {
        return Cursor.lockState == CursorLockMode.Locked ? true : false;
    }
}
