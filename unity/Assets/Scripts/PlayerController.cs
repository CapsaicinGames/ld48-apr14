﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
    public AnimationCurve m_accelCurve;
    public float speedScalar;
    public Vector2 rotationScalar;
    public AnimationCurve rotationCoefficentBySpeed;
    public Vector2 minMaxPitch;
    public float pitchFadeOffWindow;
    public float rightingForce;

    public float airDrag;
    public float waterDrag;

    public float aboveWaterTorque;
    public float maxAirAngSpeed;

    public Transform m_rendererTransform;
    public float m_maxTilt;
    [Range(0f, 1f)]
    public float m_tiltSmoothing;

    public AnimationCurve autoLevelForceFromSpeed;
    public AnimationCurve autoLevelForceFromAngleError;
    public AnimationCurve autoLevelForceFromAngle;
    public AnimationCurve autoLevelForceFromDepth;

    public AudioClip[] screamSounds;

    private SharkInput m_input;
    private Vector2 m_angularVelocity;

    private float m_previousTilt;

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
            rigidbody.drag = airDrag;
            break;
        case ControlState.Water:
            rigidbody.useGravity = false;
            rigidbody.drag = waterDrag;
            break;
        }
        m_controlState = newControlState;
    }

    void UpdateRotation(Vector2 desiredVelocity) 
    {
        m_angularVelocity = desiredVelocity;

        var desiredPitchDelta = m_angularVelocity.x * Time.fixedDeltaTime;
        transform.Rotate(Vector3.right * desiredPitchDelta);
        
        var desiredYawDelta = m_angularVelocity.y * Time.fixedDeltaTime;
        transform.Rotate(Vector3.up * desiredYawDelta, Space.World);    

        var normalizedTilt = 
            Mathf.InverseLerp(0f, maxAirAngSpeed, Mathf.Abs(m_angularVelocity.y));
        var tiltMag = normalizedTilt * m_maxTilt;
        var targetTilt = tiltMag * (m_angularVelocity.y > 0f ? -1f : 1f);
        var tilt = Mathf.Lerp(m_previousTilt, targetTilt, m_tiltSmoothing);
        m_rendererTransform.localEulerAngles = Vector3.forward * tilt;
        m_previousTilt = tilt;
    }

    void OnTriggerEnter(Collider other)
    {
        //
        if (other.gameObject.tag == "Swimmer")
        {
            audio.pitch = Random.Range(0.8f, 1.2f);
            audio.PlayOneShot(screamSounds[Random.Range(0, screamSounds.Length-1)], 0.75f);
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
        // head is heavy, so tilt downwards.

        var rawPointingDownness = Vector3.Dot(-Vector3.up, transform.forward);
        var pointingDownness = Mathf.InverseLerp(1f, -1f, rawPointingDownness);

        var horizontalness = 1f - Mathf.Abs(rawPointingDownness);

        float mul = pointingDownness * horizontalness;

        // if upside down, flip torque.
        bool isRightWayUp = Vector3.Dot(Vector3.up, transform.up) > 0f;
        float torqueToDown = isRightWayUp ? aboveWaterTorque : -aboveWaterTorque;

        var angVelDelta = mul * new Vector2(torqueToDown, 0f);

        var desiredAngVel = 
            Vector2.ClampMagnitude(m_angularVelocity + angVelDelta, maxAirAngSpeed);

        return desiredAngVel;
    }

    Vector2 DoWaterControl()
    {
        float speedInput = m_input.GetSpeed();

        float currentSpeed = rigidbody.velocity.magnitude;
        float modifierSpeed = m_accelCurve.Evaluate(currentSpeed);
        var desiredForwardForce = speedInput * speedScalar * modifierSpeed;
        rigidbody.AddRelativeForce(Vector3.forward * desiredForwardForce);

        float zRotation = transform.eulerAngles.z;
        if (Mathf.Abs(zRotation) > 5f) 
        {
            // rotate back right way up
            // don't allow rotation control yet
            transform.Rotate(Vector3.forward, rightingForce);
            return Vector2.zero;
        }
        else
        {
            float speedTurnCoefficent = rotationCoefficentBySpeed.Evaluate(currentSpeed);

            float pitchSteering = m_input.GetVertical();

            float levelingVel = LevelingVel(currentSpeed, pitchSteering);

            float pitchFadeOff = PitchFadeOffProportion(pitchSteering);
            var desiredPitchVel = 
                pitchSteering * rotationScalar.y * pitchFadeOff * speedTurnCoefficent 
                + levelingVel;

            float yawSteering = m_input.GetHorizontal();
            var desiredYawVel = yawSteering * rotationScalar.x * speedTurnCoefficent;

            var desiredVelocity = new Vector2(desiredPitchVel, desiredYawVel);

            var angVelocity = 
                Vector2.Lerp(m_angularVelocity, desiredVelocity, rigidbody.angularDrag);

            return angVelocity;
        }
    }

    float LevelingVel(float currentSpeed, float pitchRequest)
    {
        var speedLevelingScalar = autoLevelForceFromSpeed.Evaluate(currentSpeed);
        
        float unlevelness = DegreesFromHorizontal();
        var angleLevelingScalar = 
            autoLevelForceFromAngleError.Evaluate(Mathf.Abs(unlevelness));

        float highAngleScalar = autoLevelForceFromAngle.Evaluate(Mathf.Abs(unlevelness));

        // if heading towards surface, level us off faster
        float depthScalar = unlevelness < 0f && pitchRequest == 0f
            ? autoLevelForceFromDepth.Evaluate(transform.position.y)
            : 1f;
        
        float levelingVel = 
            angleLevelingScalar * speedLevelingScalar * highAngleScalar * depthScalar
            * (unlevelness < 0f ? -1f : 1f);

        return levelingVel;
    }

    // normalize so up is pos degrees, down is negative.
    // by default, rotation starts from horizon, goes +ve down, and wraps to 360
    float DegreesFromHorizontal()
    {
        float currentPitch = transform.eulerAngles.x;
        bool isPitchingUp = currentPitch > 180f;
        currentPitch = -(isPitchingUp ? currentPitch - 360f : currentPitch);
        return currentPitch;
    }

    float PitchFadeOffProportion(float pitchRequest)
    {
        float currentPitch = DegreesFromHorizontal();
        bool isPitchingUp = currentPitch > 0f;

        bool isRequestingPitchToCenter = (isPitchingUp && pitchRequest > 0f) 
            || (isPitchingUp == false && pitchRequest < 0f);
        if (isRequestingPitchToCenter) 
        {
            // you can always return to center
            return 1f;
        }

        float pitchIntoFadeWindow = isPitchingUp
            ? currentPitch - (minMaxPitch.y - pitchFadeOffWindow)
            : (minMaxPitch.x + pitchFadeOffWindow) - currentPitch;
        float propIntoFadeOffWindow = 
            Mathf.InverseLerp(pitchFadeOffWindow, 0, pitchIntoFadeWindow);
        
        return propIntoFadeOffWindow;
    }
}
