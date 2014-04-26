using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
    public float steeringSensitivity;
    public float speedScalar;
    public float minSteeringScalar;
    public float maxSteeringScalar;
    public float speedSteeringFactor;
    public float reversePenaltyFactor;

    private Vector3 previousWorldTransform;

	// Update is called once per frame
	void FixedUpdate () 
    {
        float forward = -Input.GetAxis("ForwardAxis");
        float steeringLR = Input.GetAxis("Horizontal");
        float steeringUD = Input.GetAxis("Vertical");

        Debug.Log("Forward: " + forward);

        if (forward < 0f)
        {
            forward *= reversePenaltyFactor;
        }

        rigidbody.AddRelativeForce(0f, 0f, forward * speedScalar);
        
        float currentSpeedMultiplier = (float)rigidbody.velocity.magnitude;
        if (currentSpeedMultiplier < minSteeringScalar)
        {
            currentSpeedMultiplier = minSteeringScalar;
        }
        else if (currentSpeedMultiplier > maxSteeringScalar)
        {
            currentSpeedMultiplier = maxSteeringScalar;
        }

        currentSpeedMultiplier *= speedSteeringFactor;

        float pitch = steeringUD * steeringSensitivity * currentSpeedMultiplier;
        float yaw = steeringLR * steeringSensitivity * currentSpeedMultiplier;
        bool isTurning = Mathf.Approximately(pitch, 0f) == false || Mathf.Approximately(yaw, 0f) == false;
        if (isTurning)
        {
            Vector3 requiredInput = new Vector3(steeringUD * steeringSensitivity * currentSpeedMultiplier, 0, 0);

            Vector3 worldReqInput = transform.TransformDirection(requiredInput) + new Vector3(0, steeringLR * steeringSensitivity * currentSpeedMultiplier, 0);
            rigidbody.angularVelocity = worldReqInput;
            previousWorldTransform = worldReqInput;
        }
        else
        {
            previousWorldTransform = previousWorldTransform * 0.9f;
            rigidbody.angularVelocity = previousWorldTransform;
        }
        

	}
}
