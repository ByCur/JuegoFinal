using UnityEngine;

public class AIControllerArcade : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] waypoints;
    public int currentWaypoint = 0;

    [Header("Driving Parameters")]
    public float maxAISpeed = 55f;                // Velocidad máxima típica
    public float turnAggression = 4f;             // Qué tan fuerte gira
    public float driftAngleThreshold = 12f;       // Ángulo para activar drift
    public float waypointReachDistance = 8f;      // Cuándo pasar al siguiente waypoint

    [Header("Arcade Boost")]
    public bool enableNitro = true;
    public float nitroSpeed = 75f;
    public float nitroChance = 0.2f;              // Probabilidad de activar turbo en rectas

    private Rigidbody rb;
    private Controlador car;

    private float stuckTimer = 0f;
    private Vector3 lastPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        car = GetComponent<Controlador>();

        // Desactivar controles del jugador
        car.enabled = false;

        lastPosition = transform.position;
    }

    void FixedUpdate()
    {
        if (waypoints.Length == 0) return;

        DriveTowardsWaypoint();
        ApplyArcadeSteering();
        HandleDrift();
        HandleNitro();
        AntiStuckSystem();
    }

    // -----------------------------------
    // 1. AVANZAR HACIA EL WAYPOINT
    // -----------------------------------
    void DriveTowardsWaypoint()
    {
        Vector3 dir = (waypoints[currentWaypoint].position - transform.position);
        float distance = dir.magnitude;

        Vector3 localDir = transform.InverseTransformDirection(dir.normalized);
        float angle = Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg;

        // Cambiar waypoint
        if (distance < waypointReachDistance)
        {
            currentWaypoint++;
            if (currentWaypoint >= waypoints.Length)
                currentWaypoint = 0;
        }

        // ACELERAR SIEMPRE
        float speed = rb.linearVelocity.magnitude;

        if (speed < maxAISpeed)
            ApplyMotor(250f);
        else
            ApplyMotor(0f);
    }

    // -----------------------------------
    // 2. GIRO ARCADE (DIRECTO Y RÁPIDO)
    // -----------------------------------
    void ApplyArcadeSteering()
    {
        Vector3 dir = (waypoints[currentWaypoint].position - transform.position);
        Vector3 localDir = transform.InverseTransformDirection(dir.normalized);

        float angle = Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg;

        float steer = Mathf.Clamp(angle / 45f, -1f, 1f);

        car.frontLeftCollider.steerAngle = steer * car.maxSteeringAngle * turnAggression;
        car.frontRightCollider.steerAngle = steer * car.maxSteeringAngle * turnAggression;
    }

    // -----------------------------------
    // 3. DRIFT AUTOMÁTICO ARCADE
    // -----------------------------------
    void HandleDrift()
    {
        Vector3 dir = (waypoints[currentWaypoint].position - transform.position);
        Vector3 localDir = transform.InverseTransformDirection(dir.normalized);

        float angle = Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg;

        if (Mathf.Abs(angle) > driftAngleThreshold)
        {
            // Simular freno de mano suave
            car.rearLeftCollider.brakeTorque = 150f;
            car.rearRightCollider.brakeTorque = 150f;

            // Reducir fricción lateral (drift)
            var leftFric = car.rearLeftCollider.sidewaysFriction;
            leftFric.stiffness = 0.4f;
            car.rearLeftCollider.sidewaysFriction = leftFric;

            var rightFric = car.rearRightCollider.sidewaysFriction;
            rightFric.stiffness = 0.4f;
            car.rearRightCollider.sidewaysFriction = rightFric;
        }
        else
        {
            // Recuperar fricción normal
            car.rearLeftCollider.brakeTorque = 0;
            car.rearRightCollider.brakeTorque = 0;

            var leftFric = car.rearLeftCollider.sidewaysFriction;
            leftFric.stiffness = 1f;
            car.rearLeftCollider.sidewaysFriction = leftFric;

            var rightFric = car.rearRightCollider.sidewaysFriction;
            rightFric.stiffness = 1f;
            car.rearRightCollider.sidewaysFriction = rightFric;
        }
    }

    // -----------------------------------
    // 4. NITRO EN RECTAS (ARCADE)
    // -----------------------------------
    void HandleNitro()
    {
        if (!enableNitro) return;

        Vector3 dir = (waypoints[currentWaypoint].position - transform.position);
        Vector3 localDir = transform.InverseTransformDirection(dir.normalized);

        // En rectas (muy poca dirección)
        if (Mathf.Abs(localDir.x) < 0.2f && Random.value < nitroChance * Time.fixedDeltaTime)
        {
            // Boost temporal
            rb.AddForce(transform.forward * 80f, ForceMode.Acceleration);

            if (rb.linearVelocity.magnitude > nitroSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * nitroSpeed;
        }
    }

    // -----------------------------------
    // 5. SISTEMA ANTI-ATASCOS
    // -----------------------------------
    void AntiStuckSystem()
    {
        if (Vector3.Distance(transform.position, lastPosition) < 0.5f)
            stuckTimer += Time.deltaTime;
        else
            stuckTimer = 0f;

        lastPosition = transform.position;

        // Si está atascado más de 2 segundos → retroceder
        if (stuckTimer > 2f)
        {
            rb.AddForce(-transform.forward * 12f, ForceMode.Acceleration);
            stuckTimer = 0f;
        }
    }

    // -----------------------------------
    // MOTOR
    // -----------------------------------
    void ApplyMotor(float torque)
    {
        car.frontLeftCollider.motorTorque = torque;
        car.frontRightCollider.motorTorque = torque;
        car.rearLeftCollider.motorTorque = torque;
        car.rearRightCollider.motorTorque = torque;
    }
}
