/*
Mensaje del creador original, respetado.

*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrometeoCarController : MonoBehaviour
{
    // --- NUEVA VARIABLE PARA 2 JUGADORES ---
    public bool isPlayer2 = false; // FALSE = WASD | TRUE = Flechas.

    //CAR SETUP
    [Space(20)]
    [Range(20, 190)]
    public int maxSpeed = 90;
    [Range(10, 120)]
    public int maxReverseSpeed = 45;
    [Range(1, 10)]
    public int accelerationMultiplier = 2;

    [Range(10, 45)]
    public int maxSteeringAngle = 27;
    [Range(0.1f, 1f)]
    public float steeringSpeed = 0.5f;

    [Range(100, 600)]
    public int brakeForce = 350;
    [Range(1, 10)]
    public int decelerationMultiplier = 2;
    [Range(1, 10)]
    public int handbrakeDriftMultiplier = 5;

    public Vector3 bodyMassCenter;

    // WHEELS
    public GameObject frontLeftMesh;
    public WheelCollider frontLeftCollider;
    public GameObject frontRightMesh;
    public WheelCollider frontRightCollider;
    public GameObject rearLeftMesh;
    public WheelCollider rearLeftCollider;
    public GameObject rearRightMesh;
    public WheelCollider rearRightCollider;

    // EFFECTS
    public bool useEffects = false;
    public ParticleSystem RLWParticleSystem;
    public ParticleSystem RRWParticleSystem;
    public TrailRenderer RLWTireSkid;
    public TrailRenderer RRWTireSkid;

    // UI
    public bool useUI = false;
    public Text carSpeedText;

    // SOUNDS
    public bool useSounds = false;
    public AudioSource carEngineSound;
    public AudioSource tireScreechSound;
    float initialCarEngineSoundPitch;

    // TOUCH (sin cambios)
    public bool useTouchControls = false;
    public GameObject throttleButton;
    PrometeoTouchInput throttlePTI;
    public GameObject reverseButton;
    PrometeoTouchInput reversePTI;
    public GameObject turnRightButton;
    PrometeoTouchInput turnRightPTI;
    public GameObject turnLeftButton;
    PrometeoTouchInput turnLeftPTI;
    public GameObject handbrakeButton;
    PrometeoTouchInput handbrakePTI;

    // CAR DATA
    [HideInInspector] public float carSpeed;
    [HideInInspector] public bool isDrifting;
    [HideInInspector] public bool isTractionLocked;

    // PRIVATE
    Rigidbody carRigidbody;
    float steeringAxis;
    float throttleAxis;
    float driftingAxis;
    float localVelocityZ;
    float localVelocityX;
    bool deceleratingCar;
    bool touchControlsSetup = false;

    WheelFrictionCurve FLwheelFriction; float FLWextremumSlip;
    WheelFrictionCurve FRwheelFriction; float FRWextremumSlip;
    WheelFrictionCurve RLwheelFriction; float RLWextremumSlip;
    WheelFrictionCurve RRwheelFriction; float RRWextremumSlip;

    void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
        carRigidbody.centerOfMass = bodyMassCenter;

        // Guardar fricciones
        FLwheelFriction = frontLeftCollider.sidewaysFriction;
        FLWextremumSlip = FLwheelFriction.extremumSlip;

        FRwheelFriction = frontRightCollider.sidewaysFriction;
        FRWextremumSlip = FRwheelFriction.extremumSlip;

        RLwheelFriction = rearLeftCollider.sidewaysFriction;
        RLWextremumSlip = RLwheelFriction.extremumSlip;

        RRwheelFriction = rearRightCollider.sidewaysFriction;
        RRWextremumSlip = RRwheelFriction.extremumSlip;

        if (useUI)
            InvokeRepeating("CarSpeedUI", 0f, 0.1f);

        if (useSounds)
            InvokeRepeating("CarSounds", 0f, 0.1f);
    }

    void Update()
    {
        // datos del coche
        carSpeed = (2 * Mathf.PI * frontLeftCollider.radius * frontLeftCollider.rpm * 60) / 1000;
        localVelocityX = transform.InverseTransformDirection(carRigidbody.linearVelocity).x;
        localVelocityZ = transform.InverseTransformDirection(carRigidbody.linearVelocity).z;

        //
        // --- SISTEMA NUEVO DE CONTROLES PARA 2 JUGADORES ---
        //

        if (!isPlayer2)   // PLAYER 1 → WASD
        {
            if (Input.GetKey(KeyCode.W))
            {
                CancelInvoke("DecelerateCar");
                deceleratingCar = false;
                GoForward();
            }
            if (Input.GetKey(KeyCode.S))
            {
                CancelInvoke("DecelerateCar");
                deceleratingCar = false;
                GoReverse();
            }
            if (Input.GetKey(KeyCode.A)) TurnLeft();
            if (Input.GetKey(KeyCode.D)) TurnRight();

            if (Input.GetKey(KeyCode.Space))
            {
                CancelInvoke("DecelerateCar");
                deceleratingCar = false;
                Handbrake();
            }
            if (Input.GetKeyUp(KeyCode.Space)) RecoverTraction();

            if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S)) ThrottleOff();

            if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S)
                && !Input.GetKey(KeyCode.Space) && !deceleratingCar)
            {
                InvokeRepeating("DecelerateCar", 0f, 0.1f);
                deceleratingCar = true;
            }

            if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && steeringAxis != 0f)
                ResetSteeringAngle();
        }
        else // PLAYER 2 → FLECHAS
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                CancelInvoke("DecelerateCar");
                deceleratingCar = false;
                GoForward();
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                CancelInvoke("DecelerateCar");
                deceleratingCar = false;
                GoReverse();
            }
            if (Input.GetKey(KeyCode.LeftArrow)) TurnLeft();
            if (Input.GetKey(KeyCode.RightArrow)) TurnRight();

            if (Input.GetKey(KeyCode.RightControl))
            {
                CancelInvoke("DecelerateCar");
                deceleratingCar = false;
                Handbrake();
            }
            if (Input.GetKeyUp(KeyCode.RightControl)) RecoverTraction();

            if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow)) ThrottleOff();

            if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow)
                && !Input.GetKey(KeyCode.RightControl) && !deceleratingCar)
            {
                InvokeRepeating("DecelerateCar", 0f, 0.1f);
                deceleratingCar = true;
            }

            if (!Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow) && steeringAxis != 0f)
                ResetSteeringAngle();
        }

        AnimateWheelMeshes();
    }

    // UI Speed
    public void CarSpeedUI()
    {
        if (useUI)
            carSpeedText.text = Mathf.RoundToInt(Mathf.Abs(carSpeed)).ToString();
    }

    // Sound
    public void CarSounds()
    {
        if (!useSounds) return;
        float enginePitch = initialCarEngineSoundPitch + (Mathf.Abs(carRigidbody.linearVelocity.magnitude) / 25f);
        carEngineSound.pitch = enginePitch;

        if ((isDrifting) || (isTractionLocked && Mathf.Abs(carSpeed) > 12f))
            if (!tireScreechSound.isPlaying) tireScreechSound.Play();
            else { }
        else
            tireScreechSound.Stop();
    }

    //
    // STEERING
    //
    public void TurnLeft()
    {
        steeringAxis -= Time.deltaTime * 10f * steeringSpeed;
        if (steeringAxis < -1f) steeringAxis = -1f;

        float angle = steeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, angle, steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, angle, steeringSpeed);
    }

    public void TurnRight()
    {
        steeringAxis += Time.deltaTime * 10f * steeringSpeed;
        if (steeringAxis > 1f) steeringAxis = 1f;

        float angle = steeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, angle, steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, angle, steeringSpeed);
    }

    public void ResetSteeringAngle()
    {
        if (steeringAxis < 0f) steeringAxis += Time.deltaTime * 10f * steeringSpeed;
        else if (steeringAxis > 0f) steeringAxis -= Time.deltaTime * 10f * steeringSpeed;

        if (Mathf.Abs(steeringAxis) < 0.01f) steeringAxis = 0f;

        float angle = steeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, angle, steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, angle, steeringSpeed);
    }

    // WHEEL VISUAL UPDATES
    void AnimateWheelMeshes()
    {
        frontLeftCollider.GetWorldPose(out Vector3 FLpos, out Quaternion FLrot);
        frontLeftMesh.transform.position = FLpos;
        frontLeftMesh.transform.rotation = FLrot;

        frontRightCollider.GetWorldPose(out Vector3 FRpos, out Quaternion FRrot);
        frontRightMesh.transform.position = FRpos;
        frontRightMesh.transform.rotation = FRrot;

        rearLeftCollider.GetWorldPose(out Vector3 RLpos, out Quaternion RLrot);
        rearLeftMesh.transform.position = RLpos;
        rearLeftMesh.transform.rotation = RLrot;

        rearRightCollider.GetWorldPose(out Vector3 RRpos, out Quaternion RRrot);
        rearRightMesh.transform.position = RRpos;
        rearRightMesh.transform.rotation = RRrot;
    }

    //
    // ENGINE / BRAKE / DRIFT
    //
    public void GoForward()
    {
        if (Mathf.Abs(localVelocityX) > 2.5f)
            isDrifting = true;
        else
            isDrifting = false;

        throttleAxis += Time.deltaTime * 3f;
        if (throttleAxis > 1f) throttleAxis = 1f;

        if (localVelocityZ < -1f)
        {
            Brakes();
            return;
        }

        if (Mathf.RoundToInt(carSpeed) < maxSpeed)
        {
            ApplyMotorTorque(accelerationMultiplier * 50f * throttleAxis);
        }
        else
        {
            ApplyMotorTorque(0);
        }
    }

    public void GoReverse()
    {
        if (Mathf.Abs(localVelocityX) > 2.5f)
            isDrifting = true;
        else
            isDrifting = false;

        throttleAxis -= Time.deltaTime * 3f;
        if (throttleAxis < -1f) throttleAxis = -1f;

        if (localVelocityZ > 1f)
        {
            Brakes();
            return;
        }

        if (Mathf.Abs(Mathf.RoundToInt(carSpeed)) < maxReverseSpeed)
        {
            ApplyMotorTorque(accelerationMultiplier * -50f * Mathf.Abs(throttleAxis));
        }
        else
        {
            ApplyMotorTorque(0);
        }
    }

    void ApplyMotorTorque(float torque)
    {
        frontLeftCollider.motorTorque = torque;
        frontRightCollider.motorTorque = torque;
        rearLeftCollider.motorTorque = torque;
        rearRightCollider.motorTorque = torque;
    }

    public void ThrottleOff()
    {
        ApplyMotorTorque(0);
    }

    public void DecelerateCar()
    {
        if (Mathf.Abs(localVelocityX) > 2.5f)
            isDrifting = true;
        else
            isDrifting = false;

        if (throttleAxis > 0f)
            throttleAxis -= Time.deltaTime * 10f;
        else if (throttleAxis < 0f)
            throttleAxis += Time.deltaTime * 10f;

        if (Mathf.Abs(throttleAxis) < 0.15f)
            throttleAxis = 0f;

        carRigidbody.linearVelocity *= (1f / (1f + (0.025f * decelerationMultiplier)));
        ApplyMotorTorque(0);

        if (carRigidbody.linearVelocity.magnitude < 0.25f)
        {
            carRigidbody.linearVelocity = Vector3.zero;
            CancelInvoke("DecelerateCar");
        }
    }

    public void Brakes()
    {
        frontLeftCollider.brakeTorque = brakeForce;
        frontRightCollider.brakeTorque = brakeForce;
        rearLeftCollider.brakeTorque = brakeForce;
        rearRightCollider.brakeTorque = brakeForce;
    }

    public void Handbrake()
    {
        isTractionLocked = true;
        driftingAxis += Time.deltaTime;
        if (driftingAxis > 1f) driftingAxis = 1f;
    }

    public void RecoverTraction()
    {
        isTractionLocked = false;
        driftingAxis -= Time.deltaTime / 1.5f;
        if (driftingAxis < 0f) driftingAxis = 0f;
    }
}
