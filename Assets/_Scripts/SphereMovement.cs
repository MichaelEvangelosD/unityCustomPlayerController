using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereMovement : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)]
    float maxSpeed = 10f;

    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 10f;

    [SerializeField, Range(0f, 10f)]
    float jumpForce = 1f;

    [SerializeField, Range(0.1f, 10f)]
    float maxJumpHeight = 1f;

    [SerializeField, Range(0, 5)]
    int maxAirJumps = 0;

    [SerializeField]
    Rect allowedMoveArea = new Rect(-4.5f, -4.5f, 10f, 10);

    [SerializeField, Range(0f, 1f)]
    float bounciness = .5f;

    Rigidbody playerRB;

    Vector2 playerInput;
    Vector3 velocity, desiredVelocity;

    bool canJump;
    bool isGrounded;

    //The current air jumps performed
    int jumpPhase = 0;

    private void Start()
    {
        playerRB = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        UpdateState();
        
        float maxSpeedChange = maxAcceleration * Time.deltaTime;

        //Basically the same IFs from Phase 3 BUT COOLER 
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);

        //canJump changes inside Update()
        if (canJump)
        {
            canJump = false;

            //Execute the actual jump
            ExecuteJump();
        }

        //Apply the final velocity to the attached rigidBody
        playerRB.velocity = velocity;

        //reset the isGrounded
        isGrounded = false; 
    }

    void UpdateState()
    {
        //Reset the rigidBody velocity
        velocity = playerRB.velocity;

        //Reset the current jumpPhase
        if (isGrounded)
        {
            jumpPhase = 0;
        }
    }

    void ExecuteJump()
    {
        if(isGrounded || jumpPhase < maxAirJumps)
        {
            //Increment jumps executed
            jumpPhase += 1;

            //Apply the Vy = Sqrt(-2*g*h) mathematical type to restrain the jump Height, don't ask me ask the scientists....
            velocity.y = Mathf.Sqrt(-2 * Physics.gravity.y * maxJumpHeight);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        EvaluateCollision(collision);
    }

    /*private void OnCollisionExit()
    {
        isGrounded = false;
    }*/

    //We use ...Stay() so we don't miss a frame.
    private void OnCollisionStay(Collision collision)
    {
        EvaluateCollision(collision);
    }

    /// <summary>
    /// Call to check if an UPWARD (X axis) force is applied on the gameObject.
    /// <para>Not optimal, neither correct for a game, just for demostration purposes.</para>
    /// </summary>
    /// <param name="collision"></param>
    void EvaluateCollision(Collision collision)
    {
        Vector3 normal = Vector3.zero;

        for (int i = 0; i < collision.contactCount; i++)
        {
            normal = collision.GetContact(i).normal;

            isGrounded |= normal.y >= 0.9f;
        }
    }

    private void Update()
    {
        #region Phase 1
        /*playerInput.x = Input.GetAxisRaw("Horizontal");
        playerInput.y = Input.GetAxisRaw("Vertical");

        //playerInput.Normalize(); // Normalize the value between 0-1

        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        transform.localPosition = new Vector3(playerInput.x, 0.5f, playerInput.y);*/
        #endregion

        #region Phase 2
        /*playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");

        Vector3 acceleration = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
        velocity += acceleration * Time.deltaTime;

        Vector3 displacement = acceleration * Time.deltaTime;

        transform.localPosition += displacement;*/
        #endregion

        #region Phase 3
        /*playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");

        Vector3 desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
        float maxSpeedChange = maxAcceleration * Time.deltaTime;

        //For X axis
        //If the sphere speed is smaller than the desired speed
        if (velocity.x < desiredVelocity.x)
        {
            //Gradually increase the velocity x
            //velocity.x += maxSpeedChange;

            //Could be done with a Clamp() method too
            velocity.x = Mathf.Min(velocity.x + maxSpeedChange, desiredVelocity.x);
        }
        else if (velocity.x > desiredVelocity.x)
        {
            velocity.x = Mathf.Max(velocity.x - maxSpeedChange, desiredVelocity.x);
        }

        //For Z axis
        if (velocity.z < desiredVelocity.z)
        {
            //Gradually increase the velocity z
            //velocity.z += maxSpeedChange;

            //Could be done with a Clamp() method too
            velocity.z = Mathf.Min(velocity.z + maxSpeedChange, desiredVelocity.z);
        }
        else if (velocity.z > desiredVelocity.z)
        {
            velocity.z = Mathf.Max(velocity.z - maxSpeedChange, desiredVelocity.z);
        }

        Vector3 displacement = velocity * Time.deltaTime;

        transform.localPosition += displacement;*/
        #endregion

        #region Phase 4
/*        //Cache user input
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");

        //Set the max speed we want to achieve
        Vector3 desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
        float maxSpeedChange = maxAcceleration * Time.deltaTime;

        //Basically the same IFs from Phase 3 BUT COOLER 
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);

        //Smoothes out the velocity so it is not inconsistent
        Vector3 displacement = velocity * Time.deltaTime;

        //The new position to teleport the sphere
        Vector3 newPos = transform.localPosition + displacement;

        //X axis bounds check
        if (newPos.x < allowedMoveArea.xMin)
        {
            //Stop the sphere from moving further out of the specified bounds
            newPos.x = allowedMoveArea.xMin; 
            velocity.x = -velocity.x * bounciness; //Bounce the ball back
        }
        else if (newPos.x > allowedMoveArea.xMax)
        {
            newPos.x = allowedMoveArea.xMax;
            velocity.x = -velocity.x * bounciness;
        }

        //Z axis bounds check
        if (newPos.z < allowedMoveArea.yMin)
        {
            newPos.z = allowedMoveArea.yMin;
            velocity.z = -velocity.z * bounciness;
        }
        else if (newPos.z > allowedMoveArea.yMax)
        {
            newPos.z = allowedMoveArea.yMax;
            velocity.z = -velocity.z * bounciness;
        }

        //Finally set the new sphere position
        transform.localPosition = newPos;*/
        #endregion

        #region Phase_5
        /*//Cache user input
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");

        //Set the max speed we want to achieve
        desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;*/
        #endregion

        //ADDED JUMPING HERE
        #region Phase_6
        //Cache user input
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        
        //Set the max speed we want to achieve
        desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;

        //Set canJump to the user input given or just keep it to the previous value
        canJump |= Input.GetButtonDown("Jump");
        #endregion
    }

    #region Utilities
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.0f, 1f, 0.0f);
        DrawRect(allowedMoveArea);
    }

    void DrawRect(Rect rect)
    {
        Gizmos.DrawWireCube(new Vector3(rect.center.x, 0.01f, rect.center.y), new Vector3(rect.size.x, 0.01f, rect.size.y));
    }
    #endregion
}
