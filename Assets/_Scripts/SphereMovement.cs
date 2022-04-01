using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereMovement : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)]
    float maxSpeed = 10f;

    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 10f;

    [SerializeField]
    Rect allowedMoveArea = new Rect(-4.5f, -4.5f, 10f, 10);

    [SerializeField, Range(0f, 1f)]
    float bounciness = .5f;

    Vector2 playerInput;
    Vector3 velocity;

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
        //Cache user input
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");

        //Set the max speed we want to achieve
        Vector3 desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
        float maxSpeedChange = maxAcceleration * Time.deltaTime;

        //Basically the same IFs from Phase 3 BUT COOLER 
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);

        //Smoothes out the velocity so it is not snappy
        Vector3 displacement = velocity * Time.deltaTime;

        //The new position to teleport the sphere
        Vector3 newPos = transform.localPosition + displacement;

        //X axis bounds check
        if (newPos.x < allowedMoveArea.xMin)
        {
            newPos.x = allowedMoveArea.xMin; //Stop the sphere from moving
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
        transform.localPosition = newPos;
        #endregion
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.0f, 1f, 0.0f);
        DrawRect(allowedMoveArea);
    }

    void DrawRect(Rect rect)
    {
        Gizmos.DrawWireCube(new Vector3(rect.center.x, 0.01f, rect.center.y), new Vector3(rect.size.x, 0.01f, rect.size.y));
    }
}
