/*
Versión corregida para controles configurables y freno de mano funcional.
Corregidos: uso de Rigidbody.velocity en lugar de linearVelocity y asignaciones correctas.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controlador : MonoBehaviour
{
    // --- Configuración jugador ---
    public bool isPlayer2 = false; // solo para asignar teclas por defecto en Start
    [Header("Key Mapping (editable)")]
    public KeyCode keyForward;
    public KeyCode keyReverse;
    public KeyCode keyLeft;
    public KeyCode keyRight;
    public KeyCode keyHandbrake;

    //CAR SETUP
    [Space(10)]
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
    float initialCarEngineSoundPitch = 1f;

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

    // Fricción guardada
    WheelFrictionCurve FLwheelFriction; float FLWextremumSlip;
    WheelFrictionCurve FRwheelFriction; float FRWextremumSlip;
    WheelFrictionCurve RLwheelFriction; float RLWextremumSlip;
    WheelFrictionCurve RRwheelFriction; float RRWextremumSlip;

    void Awake()
    {
        // Si no se asignaron teclas en el inspector, se asignan por defecto según isPlayer2
        if (keyForward == KeyCode.None &&
            keyReverse == KeyCode.None &&
            keyLeft == KeyCode.None &&
            keyRight == KeyCode.None &&
            keyHandbrake == KeyCode.None)
        {
            if (!isPlayer2)
            {
                keyForward = KeyCode.W;
                keyReverse = KeyCode.S;
                keyLeft = KeyCode.A;
                keyRight = KeyCode.D;
                keyHandbrake = KeyCode.Space;
            }
            else
            {
                keyForward = KeyCode.UpArrow;
                keyReverse = KeyCode.DownArrow;
                keyLeft = KeyCode.LeftArrow;
                keyRight = KeyCode.RightArrow;
                keyHandbrake = KeyCode.RightControl;
            }
        }
    }

    void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
        if (carRigidbody != null)
            carRigidbody.centerOfMass = bodyMassCenter;
        else
            Debug.LogWarning("[Controlador] Falta Rigidbody en " + gameObject.name);

        // Guardar fricciones (si están asignados los colliders)
        if (frontLeftCollider != null) { FLwheelFriction = frontLeftCollider.sidewaysFriction; FLWextremumSlip = FLwheelFriction.extremumSlip; }
        if (frontRightCollider != null) { FRwheelFriction = frontRightCollider.sidewaysFriction; FRWextremumSlip = FRwheelFriction.extremumSlip; }
        if (rearLeftCollider != null) { RLwheelFriction = rearLeftCollider.sidewaysFriction; RLWextremumSlip = RLwheelFriction.extremumSlip; }
        if (rearRightCollider != null) { RRwheelFriction = rearRightCollider.sidewaysFriction; RRWextremumSlip = RRwheelFriction.extremumSlip; }

        if (useUI)
            InvokeRepeating("CarSpeedUI", 0f, 0.1f);

        if (useSounds)
        {
            if (carEngineSound != null) initialCarEngineSoundPitch = carEngineSound.pitch;
            InvokeRepeating("CarSounds", 0f, 0.1f);
        }
    }

    void Update()
    {
        // seguridad: comprobar colliders y rigidbody
        if (frontLeftCollider == null || frontRightCollider == null || rearLeftCollider == null || rearRightCollider == null)
        {
            // no hacemos nada si falta asignación completa
            return;
        }

        // datos del coche
        carSpeed = (2 * Mathf.PI * frontLeftCollider.radius * frontLeftCollider.rpm * 60) / 1000;
        if (carRigidbody != null)
        {
            localVelocityX = transform.InverseTransformDirection(carRigidbody.linearVelocity).x;
            localVelocityZ = transform.InverseTransformDirection(carRigidbody.linearVelocity).z;
        }
        else
        {
            localVelocityX = 0f;
            localVelocityZ = 0f;
        }

        // Controles usando teclas configurables
        bool forward = Input.GetKey(keyForward);
        bool reverse = Input.GetKey(keyReverse);
        bool left = Input.GetKey(keyLeft);
        bool right = Input.GetKey(keyRight);
        bool handbrakeDown = Input.GetKey(keyHandbrake);
        bool handbrakeUp = Input.GetKeyUp(keyHandbrake);

        if (forward)
        {
            CancelInvoke("DecelerateCar");
            deceleratingCar = false;
            GoForward();
        }
        if (reverse)
        {
            CancelInvoke("DecelerateCar");
            deceleratingCar = false;
            GoReverse();
        }
        if (left) TurnLeft();
        if (right) TurnRight();

        if (handbrakeDown)
        {
            CancelInvoke("DecelerateCar");
            deceleratingCar = false;
            ApplyHandbrake(); // freno de mano funcional
        }
        if (handbrakeUp) RecoverTraction(); // al soltar

        if (!forward && !reverse) ThrottleOff();

        if (!forward && !reverse && !handbrakeDown && !deceleratingCar)
        {
            InvokeRepeating("DecelerateCar", 0f, 0.1f);
            deceleratingCar = true;
        }

        if (!left && !right && steeringAxis != 0f)
            ResetSteeringAngle();


        AnimateWheelMeshes();

        if (isTractionLocked && !Input.GetKey(keyHandbrake))
        {
            if (carRigidbody.linearVelocity.magnitude < 0.5f)
                RecoverTraction();
        }
    }

    // UI Speed
    public void CarSpeedUI()
    {
        if (useUI && carSpeedText != null)
            carSpeedText.text = Mathf.RoundToInt(Mathf.Abs(carSpeed)).ToString();
    }

    // Sound
    public void CarSounds()
    {
        if (!useSounds || carEngineSound == null) return;
        float enginePitch = initialCarEngineSoundPitch + (Mathf.Abs(carRigidbody != null ? carRigidbody.linearVelocity.magnitude : 0f) / 25f);
        carEngineSound.pitch = enginePitch;

        if ((isDrifting) || (isTractionLocked && Mathf.Abs(carSpeed) > 12f))
        {
            if (tireScreechSound != null && !tireScreechSound.isPlaying) tireScreechSound.Play();
        }
        else
        {
            if (tireScreechSound != null) tireScreechSound.Stop();
        }
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
        if (frontLeftCollider != null)
        {
            frontLeftCollider.GetWorldPose(out Vector3 FLpos, out Quaternion FLrot);
            if (frontLeftMesh != null) { frontLeftMesh.transform.position = FLpos; frontLeftMesh.transform.rotation = FLrot; }
        }
        if (frontRightCollider != null)
        {
            frontRightCollider.GetWorldPose(out Vector3 FRpos, out Quaternion FRrot);
            if (frontRightMesh != null) { frontRightMesh.transform.position = FRpos; frontRightMesh.transform.rotation = FRrot; }
        }
        if (rearLeftCollider != null)
        {
            rearLeftCollider.GetWorldPose(out Vector3 RLpos, out Quaternion RLrot);
            if (rearLeftMesh != null) { rearLeftMesh.transform.position = RLpos; rearLeftMesh.transform.rotation = RLrot; }
        }
        if (rearRightCollider != null)
        {
            rearRightCollider.GetWorldPose(out Vector3 RRpos, out Quaternion RRrot);
            if (rearRightMesh != null) { rearRightMesh.transform.position = RRpos; rearRightMesh.transform.rotation = RRrot; }
        }
    }

    //
    // ENGINE / BRAKE / DRIFT
    //
    public void GoForward()
    {
    frontLeftCollider.brakeTorque = 0;
    frontRightCollider.brakeTorque = 0;
    rearLeftCollider.brakeTorque = 0;
    rearRightCollider.brakeTorque = 0;

        if (Mathf.Abs(localVelocityX) > 2.5f) isDrifting = true; else isDrifting = false;

        throttleAxis += Time.deltaTime * 3f;
        if (throttleAxis > 1f) throttleAxis = 1f;

        if (localVelocityZ < -1f) { Brakes(); return; }

        if (Mathf.RoundToInt(carSpeed) < maxSpeed) ApplyMotorTorque(accelerationMultiplier * 50f * throttleAxis);
        else ApplyMotorTorque(0);
    }

    public void GoReverse()
    {
    frontLeftCollider.brakeTorque = 0;
    frontRightCollider.brakeTorque = 0;
    rearLeftCollider.brakeTorque = 0;
    rearRightCollider.brakeTorque = 0;

        if (Mathf.Abs(localVelocityX) > 2.5f) isDrifting = true; else isDrifting = false;

        throttleAxis -= Time.deltaTime * 3f;
        if (throttleAxis < -1f) throttleAxis = -1f;

        if (localVelocityZ > 1f) { Brakes(); return; }

        if (Mathf.Abs(Mathf.RoundToInt(carSpeed)) < maxReverseSpeed)
            ApplyMotorTorque(accelerationMultiplier * -50f * Mathf.Abs(throttleAxis));
        else
            ApplyMotorTorque(0);
    }

    void ApplyMotorTorque(float torque)
    {
        // Si tracción bloqueada (handbrake), no aplicar motor torque
        if (isTractionLocked) torque = 0f;

        if (frontLeftCollider != null) frontLeftCollider.motorTorque = torque;
        if (frontRightCollider != null) frontRightCollider.motorTorque = torque;
        if (rearLeftCollider != null) rearLeftCollider.motorTorque = torque;
        if (rearRightCollider != null) rearRightCollider.motorTorque = torque;
    }

    public void ThrottleOff()
    {
        ApplyMotorTorque(0);
    }

    public void DecelerateCar()
    {
        if (Mathf.Abs(localVelocityX) > 2.5f) isDrifting = true; else isDrifting = false;

        if (throttleAxis > 0f) throttleAxis -= Time.deltaTime * 10f;
        else if (throttleAxis < 0f) throttleAxis += Time.deltaTime * 10f;

        if (Mathf.Abs(throttleAxis) < 0.15f) throttleAxis = 0f;

        if (carRigidbody != null)
        {
            float factor = (1f / (1f + (0.025f * decelerationMultiplier)));
            carRigidbody.linearVelocity = carRigidbody.linearVelocity * factor;
        }

        ApplyMotorTorque(0);

        if (carRigidbody != null && carRigidbody.linearVelocity.magnitude < 0.25f)
        {

            CancelInvoke("DecelerateCar");
        }
    }

    public void Brakes()
    {
        if (frontLeftCollider != null) frontLeftCollider.brakeTorque = brakeForce;
        if (frontRightCollider != null) frontRightCollider.brakeTorque = brakeForce;
        if (rearLeftCollider != null) rearLeftCollider.brakeTorque = brakeForce;
        if (rearRightCollider != null) rearRightCollider.brakeTorque = brakeForce;
    }

    // Freno de mano auténtico: aplica brakeTorque a las ruedas traseras y reduce fricción lateral.
    public void ApplyHandbrake()
    {
        isTractionLocked = true;

        // Aumentar brake torque trasero
        float hbTorque = brakeForce * handbrakeDriftMultiplier;
        if (rearLeftCollider != null) rearLeftCollider.brakeTorque = hbTorque;
        if (rearRightCollider != null) rearRightCollider.brakeTorque = hbTorque;

        // Reducir ligeramente la fricción lateral de las ruedas traseras para facilitar el drift
        if (rearLeftCollider != null)
        {
            WheelFrictionCurve w = rearLeftCollider.sidewaysFriction;
            w.extremumSlip = RLWextremumSlip * handbrakeDriftMultiplier;
            rearLeftCollider.sidewaysFriction = w;
        }

        if (rearRightCollider != null)
        {
            WheelFrictionCurve w = rearRightCollider.sidewaysFriction;
            w.extremumSlip = RRWextremumSlip * handbrakeDriftMultiplier;
            rearRightCollider.sidewaysFriction = w;
        }

        // Evitar aplicar motor mientras el freno de mano esté activo
        ApplyMotorTorque(0);
    }

    public void RecoverTraction()
    {
        isTractionLocked = false;

        // Quitar brake torque
        if (rearLeftCollider != null) rearLeftCollider.brakeTorque = 0f;
        if (rearRightCollider != null) rearRightCollider.brakeTorque = 0f;

        // Restaurar fricción lateral original
        if (rearLeftCollider != null)
        {
            WheelFrictionCurve wRL = rearLeftCollider.sidewaysFriction;
            wRL.extremumSlip = RLWextremumSlip;
            rearLeftCollider.sidewaysFriction = wRL;
        }

        if (rearRightCollider != null)
        {
            WheelFrictionCurve wRR = rearRightCollider.sidewaysFriction;
            wRR.extremumSlip = RRWextremumSlip;
            rearRightCollider.sidewaysFriction = wRR;
        }
    }
}
