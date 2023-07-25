using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum ViewStates
{
    THIRD,    // Sets the camera behind the designated object.
    FIRST,    // Sets the camera's perspective to the designated object.
    ORBIT     // Orbits the camera around a designated object; "spectates" it.
}

public class CameraController : MonoBehaviour
{
    public Transform thirdPersonTarget;        // The object we want to look at while in 3rd person.
    public Transform firstPersonTarget;        // The object we want to hinge from while in 1st person.
    public float xSpeed = 1500;      // The speed of camera's horizontal movement via input.
    public float ySpeed = 1500;      // The speed of camera's vertical movement via input.
    public float distance = 5;      // The maximum distance to keep the camera between the target.
    float currentDistance;          // The distance the camera is currently at.
    public float relaxSpeed = 10;   // The speed at which the camera relaxes back to the target distance.
    public float distanceBack;      // The distance offset set via scrolling; zoom.
    public float zoomSpeed = 5;     // The speed the player can zoom their camera.
    public float heightOffset = 0;  // The target height offset of the camera.
    public ViewStates viewState = ViewStates.THIRD;  // The camera view state. Defaults to third person.
    public LayerMask ignoreMask;
    bool usedZoomModifier = false;  // Used to record if we've used TAB for the camera Zoom, otherwise we've just tapped it for a perspective swap.
    public bool yInvert = false;    // Invert Y-axis mouse movement, for plane nerds.

    public Vector3 firstPersonOffset = Vector3.zero;    // The offset position to place the first person camera.

    // Start is called before the first frame update
    void Start()
    {
        currentDistance = distance;
        distanceBack = distance;

        // Hide & lock the cursor to the game.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Right drag rotates the camera.
        //if (Input.GetMouseButton(1))
        {
            Vector3 angles = transform.eulerAngles;
            float dx = !GameManager.instance.IsCursorLocked() ? 0 : GameManager.instance.Input_GetAxis("Mouse Y") * (yInvert ? 1 : -1);
            float dy = !GameManager.instance.IsCursorLocked() ? 0 : -GameManager.instance.Input_GetAxis("Mouse X");

            // Look up and down by rotating around X-axis.
            //angles.x = Mathf.Clamp(angles.x + dx * speed * Time.deltaTime, 0, 70);
            angles.x += dx * xSpeed;

            // Converts to -180 to +180 degrees so we can process it properly.
            float tempXAng = angles.x - (angles.x > 180 ? 360 : 0);

            // Clamp the angle.
            if (tempXAng > 80)
                angles.x = 80;
            else if (tempXAng < -80)
                angles.x = 280;

            // Spin the camera round.
            angles.y += -dy * ySpeed;
            transform.eulerAngles = angles;
        }

        // Releasing Tab swaps the view mode quickly.
        if (GameManager.instance.Input_GetButtonUp("Camera") && !usedZoomModifier)
        {
            // Toggle between first and third person.

            switch (viewState)
            {
                case ViewStates.THIRD:
                    //viewState = ViewStates.FIRST;
                    distanceBack = 0;
                    break;

                case ViewStates.FIRST:
                    //viewState = ViewStates.THIRD;
                    distanceBack = 5;
                    break;

                case ViewStates.ORBIT:
                    viewState = ViewStates.THIRD;  // TODO: Placeholder.
                    break;
            }
        }
        else if (GameManager.instance.Input_GetButtonUp("Camera") && usedZoomModifier)
            usedZoomModifier = false;

        // Zoom in & out with the mouse wheel.
        // But, only if we're holding Tab!
        // Tab is the magical "camera"-related key....
        if (GameManager.instance.Input_GetButton("Camera"))
        {
            distanceBack = Mathf.Clamp(distanceBack - GameManager.instance.Input_GetAxis("Mouse ScrollWheel") * zoomSpeed, 0, distance);

            // If we touched the mouse wheel while holding Tab, record it.
            if (GameManager.instance.Input_GetAxis("Mouse ScrollWheel") != 0)
            {
                usedZoomModifier = true;
            }
        }

        // Wall collisions for varied camera distance.
        RaycastHit hit;
        if (Physics.Raycast(GetTargetPosition(), -transform.forward, out hit, distanceBack, ~ignoreMask))
        {
            // Snap the camera right in to where the collision happened.
            currentDistance = hit.distance;
        }
        else
        {
            // Relax the camera back to the desired distance.
            currentDistance = Mathf.MoveTowards(currentDistance, distanceBack, Mathf.Abs(currentDistance - distanceBack) * relaxSpeed * Time.deltaTime);
        }

        // Automatically swap camera views depending on the camera distance ala Bethesda games.
        if (currentDistance < 1)
            viewState = ViewStates.FIRST;
        else if (currentDistance > 1)
            viewState = ViewStates.THIRD;


        // Look at the target point.
        transform.position = GetTargetPosition();

        if (viewState != ViewStates.FIRST)
            transform.position -= currentDistance * transform.forward;
    }

    Vector3 GetTargetPosition()
    {
        switch (viewState)
        {
            case ViewStates.THIRD:
                return thirdPersonTarget.position + heightOffset * Vector3.up;

            case ViewStates.FIRST:
                return firstPersonTarget.position + firstPersonOffset.y * Vector3.up;

            case ViewStates.ORBIT:
                return thirdPersonTarget.position + heightOffset * Vector3.up;  // TODO: Placeholder.

            default:
                return thirdPersonTarget.position + heightOffset * Vector3.up;  // Defaults to 3P.
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        Gizmos.DrawSphere(GetTargetPosition(), 0.1f);
    }
}