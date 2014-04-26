using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
    public float speedScalar;
    public Vector2 rotationScalar;

    private SharkInput m_input;
    private Vector2 m_angularVelocity;

    enum ControlState
    {
        Air,
        Water,
    }
    private ControlState m_controlState;

    void Start()
    {
        m_input = GetComponent<SharkInput>();
        SetControlState(ControlState.Water);
    }

	// Update is called once per frame
	void FixedUpdate () 
    {
        var desiredControlState = 
            transform.position.y > 0f ? ControlState.Air : ControlState.Water;
        SetControlState(desiredControlState);
        
        var desiredAngularVel = 
            m_controlState == ControlState.Water ? DoWaterControl() : DoAirControl();
        UpdateRotation(desiredAngularVel);
	}

    void SetControlState(ControlState newControlState) 
    {
        if (newControlState == m_controlState)
        {
            return;
        }

        switch(newControlState) 
        {
        case ControlState.Air:
            rigidbody.useGravity = true;
            break;
        case ControlState.Water:
            rigidbody.useGravity = false;
            break;
        }
        m_controlState = newControlState;
    }

    void UpdateRotation(Vector2 desiredVelocity) 
    {
        m_angularVelocity = 
            Vector2.Lerp(m_angularVelocity, desiredVelocity, rigidbody.angularDrag);

        var desiredPitchDelta = m_angularVelocity.x * Time.fixedDeltaTime;
        transform.Rotate(Vector3.right * desiredPitchDelta);
        
        var desiredYawDelta = m_angularVelocity.y * Time.fixedDeltaTime;
        transform.Rotate(Vector3.up * desiredYawDelta, Space.World);        
    }

    void OnTriggerEnter(Collider other)
    {
        //
        if (other.gameObject.tag == "Swimmer")
        {
            //Destroy(other.gameObject);
            other.transform.parent = transform;
            other.gameObject.BroadcastMessage("onGrabbed", SendMessageOptions.DontRequireReceiver);
            foreach (var col in other.gameObject.GetComponentsInChildren<Collider>())
            {
                col.enabled = false;
            }
            foreach (var rb in other.gameObject.GetComponentsInChildren<Rigidbody>())
            {
                Destroy(rb);
            }
        }
    }

    Vector2 DoAirControl()
    {
        return Vector2.zero;
    }

    Vector2 DoWaterControl()
    {
        float speedInput = m_input.GetSpeed();

        var desiredForwardForce = speedInput * speedScalar;
        rigidbody.AddRelativeForce(Vector3.forward * desiredForwardForce);

        float pitchSteering = m_input.GetVertical();
        var desiredPitchVel = pitchSteering * rotationScalar.y;

        float yawSteering = m_input.GetHorizontal();
        var desiredYawVel = yawSteering * rotationScalar.x;

        var desiredVelocity = new Vector2(desiredPitchVel, desiredYawVel);

        return desiredVelocity;
    }
}
p