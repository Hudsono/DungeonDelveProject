using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeScript : MonoBehaviour
{
    public int Uses; //how many dodges can be preformed
    public float cooldown; //time to gain new dodge
    public float distance; //distance of the dodge
    public float speed; //how fast is the dodge
    public float destinationMultiplier; //offset to stop clipping into wall upon arrival
    public float cameraHeight; //offset to stop clipping into floor upon arrival
    public Transform cam;
    public LayerMask layerMask;
    public CharacterController characterController;
    public GameObject render;

    int maxUses;
    float cooldownTimer;
    bool dodging = false;
    Vector3 destination;

    void Start()
    {
        maxUses = Uses;
        cooldownTimer = cooldown;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Dodge();
            Debug.Log("Execute Dodge");
        }

        if (Uses < maxUses)
        {
            if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;
            else
            {
                Uses += 1;
                cooldownTimer = cooldown;
            }
        }

        if (dodging)
        {
            var dist = Vector3.Distance(transform.position, destination);
            if (dist > 0.5f)
            {
                characterController.enabled = false;
                transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * speed); //move the player to the new point
                characterController.enabled = true;
                Debug.Log("Hello");
                render.SetActive(true);
            }
            else
            {
                dodging = false;
                render.SetActive(false);
            }
        }
    }

    void Dodge()
    {
        if (Uses > 0)
        {
            Uses -= 1;

            RaycastHit hit;
            if(Physics.Raycast(cam.position, cam.forward, out hit, distance, layerMask)) //raycast hits a wall or object
            {
                destination = hit.point * destinationMultiplier; //offset to stop clipping into wall upon arrival 
                Debug.Log("Wall");
                Debug.DrawLine(cam.position, hit.point * destinationMultiplier, Color.yellow, 2);
            }
            else //raycast hits nothing
            {
                destination = (cam.position + cam.forward.normalized * distance) * destinationMultiplier;
                Debug.Log("Open Space");
                Debug.DrawRay(cam.position, (cam.forward * distance) * destinationMultiplier, Color.green, 2);
            }

            destination.y += cameraHeight; //offset to stop clipping into floor upon arrival
            dodging = true;
        }
    }
}
