using UnityEngine;
using System;
using UnityEngine.UIElements;
using UnityEngine.Animations;
using System.Diagnostics;

public class CharacterMover : MonoBehaviour
{
    public float speed = 10;

    CharacterController cc;
    Vector2 moveInput = new Vector2();
    bool jumpInput = false;
    public Vector3 velocity = new Vector3();
    public float jumpHeight = 2;
    public bool isGrounded = false;
    Animator animator;
    public float mass = 200;
    Vector3 m_impulseForce = new Vector3();     // Used to enact external impulsive forces onto the movement; knockback.

    [SerializeField]
    [Tooltip("How fast should the character model turn?")]
    float m_turnSpeed = 10.0f;

    [SerializeField]
    [Tooltip("How steep of a slope the character can climb.")]
    float m_maxSlopeAngle = 45;

    [Tooltip("Whether this character is currently being controlled.")]
    public bool isControlled = false;

    RaycastHit m_slopeHit;  // The raycast result detecting whether the character sits on a valid slope.

    public Vector3 hitDirection;
    Vector3 ccHitPoint;

    float m_moveSpeed;  // The speed the character is currently moving.

    /// <summary>
    /// The Agent controlling this CharacterMover.
    /// </summary>
    public Agent agent;


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        hitDirection = hit.point - transform.position;
        ccHitPoint = hit.point;

        Rigidbody otherRB = hit.rigidbody ? hit.rigidbody : hit.collider.attachedRigidbody;

        if (otherRB != null)
        {
            // Move objects when pushing into them.
            otherRB.AddForceAtPosition(velocity * mass, hit.point);
        }

    }

    void Update()
    {
        // Only permit input if we're controlled.
        if (agent != null)
        {
            //moveInput.x = GameManager.instance.Input_GetAxis("Horizontal");
            moveInput.x = agent.Input_GetAxis("Horizontal");
            moveInput.y = agent.Input_GetAxis("Vertical");

            m_moveSpeed = moveInput.magnitude;
            if (m_moveSpeed > 1)
            {
                moveInput.Normalize();  // Restrict to a circle rather than a square; prevents faster diagonal movement.
                m_moveSpeed = 1;
            }

            animator.SetFloat("Movement", m_moveSpeed);

            jumpInput = agent.Input_GetButton("Jump");

            if (agent.Input_GetButtonDown("Kill"))
                ResetPosition();
        }
        else
        {
            // Nullify input values when not controlled.
            moveInput = Vector2.zero;
            animator.SetFloat("Movement", 0);
            jumpInput = false;
            m_moveSpeed = 0;
        }

        //animator.SetBool("Jump", !isGrounded);
        //animator.SetBool("Crouch", m_isCrouching);
    }

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        // Add this character to the GameManager's list of them upon creation.
        GameManager.instance.AddCharacter(this);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 delta;

        // Player movement using WASD or arrow keys.
        // Find the horizontal unit vector facing forward from the camera.
        Vector3 camForward = isControlled ? Camera.main.transform.forward : Vector3.forward;
        camForward.y = 0;
        camForward.Normalize();

        //transform.forward = ((transform.forward - -camForward) * Time.fixedDeltaTime);
        //transform.forward = camForward; 

        // Use our camera's right vector, which is always horizontal.
        Vector3 camRight = isControlled ? Camera.main.transform.right : Vector3.right;

        delta = (moveInput.x * camRight + moveInput.y * camForward) * speed * Time.fixedDeltaTime;

        // Convert jump velocity into jump height:
        float jumpVelocity = Mathf.Sqrt(-2.0f * Physics.gravity.y * jumpHeight);

        // Check for jumping:
        if (jumpInput && isGrounded)
            velocity.y = jumpVelocity;

        bool onSlope = isOnSlope(); // Calculate once as we'll use it later in the udpate loop.

        // If we're standing on a slope, angle the character's movements to the slope accordingly.
        if (onSlope)
        {
            delta = Vector3.ProjectOnPlane(delta, m_slopeHit.normal);   // Project the movement delta onto the slope's angle via its normal.
        }

        // Check if we've hit ground from falling. If so, remove our velocity.
        if (isGrounded && velocity.y < 0)
            velocity.y = 0;

        // Apply gravity after zeroing velocity so we register as grounded still.
        velocity += Physics.gravity * Time.fixedDeltaTime;

        // Prevent character from standing on very edges of objects.
        if (!isGrounded)
            hitDirection = Vector3.zero;

        // Slide objects off surfaces they're hanging on to.
        if (!Physics.Raycast(transform.position, Vector3.down, cc.height * 0.5f + 0.3f, LayerMask.GetMask("Default")))
        {
            Vector3 horizontalHitDirection = hitDirection;
            horizontalHitDirection.y = 0;
            float displacement = horizontalHitDirection.magnitude;
            if (displacement > 0)
                velocity -= 0.2f * horizontalHitDirection / displacement;
        }
        else
        {
            velocity.x = 0;
            velocity.z = 0;
        }



        // And apply this to our positional update this time.
        delta += velocity * Time.fixedDeltaTime;

        // Apply impulsive forces.
        delta += m_impulseForce * Time.fixedDeltaTime;

        UnityEngine.Debug.DrawRay(transform.position, m_impulseForce, Color.red, 1);

        // Update impulsive forces.
        m_impulseForce *= 0.8f;

        // Limit unnecessarily-small calculations.
        if (m_impulseForce.magnitude < 0.1f)
            m_impulseForce = Vector3.zero;

        //UnityEngine.Debug.Log(m_impulseForce.magnitude);


        // Face the character in the direction we're moving.
        Vector3 aimDir;
        if (agent.isFacing)
        {
            aimDir = agent.facingPosition - transform.position;
        }   
        else
            aimDir = new Vector3(delta.x, 0, delta.z).normalized;

        if (agent.isFacing)
        {
            transform.forward = Vector3.RotateTowards(transform.forward, aimDir, m_turnSpeed * Time.fixedDeltaTime, 0.0f);
        }
        else
        {
            // Multiply by the aim direction by the current movement speed. This means the character turns slower when they move slower.
            // Good for interpolating from running -> stopping, so the character doesn't snap because of a stray movement value when standing still.
            transform.forward = Vector3.RotateTowards(transform.forward, aimDir, m_moveSpeed * m_turnSpeed * Time.fixedDeltaTime, 0.0f);
        }

        
        cc.Move(delta);
        isGrounded = cc.isGrounded || onSlope;
    }

    /// <summary>
    /// Set the position of the character controller.
    /// </summary>
    /// <param name="_position"></param>
    public void setPosition(Vector3 _position)
    {
        // The CharacterController overrides the position transform otherwise.
        cc.enabled = false;
        transform.position = _position;
        cc.enabled = true;
    }

    /// <summary>
    /// Detect whether the character is on a valid slope surface, determined by the maxSlopeAngle.
    /// This solution was adapted by Dave / GameDevelopment 2022, available at: https://www.youtube.com/watch?v=xCxSjgYTw9c (viewed 28/02/2023).
    /// </summary>
    /// <returns>Whether the character sits on a climb-able slope.</returns>
    bool isOnSlope()
    {
        // Fire a raycast to get the normal of the ground below + see if there's even a valid ground close enough.
        if (Physics.Raycast(transform.position, Vector3.down, out m_slopeHit, cc.height * 0.5f + 0.3f, LayerMask.GetMask("Default")))
        {
            // Get the angle of the ground we're standing on.
            float slopeAngle = Vector3.Angle(Vector3.up, m_slopeHit.normal);
            return (slopeAngle < m_maxSlopeAngle && slopeAngle != 0);   // Return true if the angle is less steep than the specified max angle + is not on flat ground.
        }

        return false;
    }

    /// <summary>
    /// Reset this character's position back to the game's spawn point.
    /// </summary>
    public void ResetPosition()
    {
        GameManager.instance.RespawnCharacter(this);
    }

    /// <summary>
    /// Enact an external impulsive force on the character's movement.
    /// </summary>
    /// <param name="force"></param>
    public void ImpulseForce(Vector3 force)
    {
        m_impulseForce = force;

        //UnityEngine.Debug.LogError("o");
    }
}