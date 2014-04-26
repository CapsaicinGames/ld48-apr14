using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
    public float steeringSensitivity;
    public float speedScalar;
    public float minSteeringScalar;
    public float maxSteeringScalar;
    public float speedSteeringFactor;

	// Update is called once per frame
	void FixedUpdate () 
    {
        float forward = Input.GetAxis("Vertical");
        float steering = Input.GetAxis("Horizontal");

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

        rigidbody.AddRelativeTorque(0f, steering * steeringSensitivity * currentSpeedMultiplier, 0);
	}
}
