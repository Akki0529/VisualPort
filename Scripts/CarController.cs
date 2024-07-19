using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private float horizontalInput;
    private float verticalInput;
    private bool isBreaking;
    private float currentBreakForce;
    private float currentSteerAngle;

    [SerializeField] private float motorForce = 1500f;
    [SerializeField] private float breakForce = 3000f;
    [SerializeField] private float maxSteeringAngle = 30f;

    [SerializeField] private WheelCollider FrontLeftWheelCollider;
    [SerializeField] private WheelCollider FrontRightWheelCollider;
    [SerializeField] private WheelCollider RearLeftWheelCollider;
    [SerializeField] private WheelCollider RearRightWheelCollider;

    [SerializeField] private Transform FrontLeftWheelTransform;
    [SerializeField] private Transform FrontRightWheelTransform;
    [SerializeField] private Transform RearLeftWheelTransform;
    [SerializeField] private Transform RearRightWheelTransform;

    public SpawnManager spawnManager;

    public float movementSpeed = 25f;

    [SerializeField] private CurvedLineGenerator lineGenerator;

    private List<Transform> waypoints;
    private int currentWaypointIndex = 0;

    // New flag for autopilot mode
    public bool autopilot = true;

    private void Start()
    {
        if (lineGenerator != null)
        {
            waypoints = lineGenerator.waypoints;
        }
    }

    private void Update()
    {
        if (autopilot)
        {
            HandlePathFollowing();
        }
        else
        {
            GetInput();
        }
    }

    private void FixedUpdate()
    {
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
        isBreaking = Input.GetKey(KeyCode.Space);
        if (verticalInput > 0)
        {
            isBreaking = false;
        }
    }

    private void HandleMotor()
    {
        RearLeftWheelCollider.motorTorque = verticalInput * motorForce;
        RearRightWheelCollider.motorTorque = verticalInput * motorForce;

        currentBreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();
    }

    private void ApplyBreaking()
    {
        FrontLeftWheelCollider.brakeTorque = currentBreakForce;
        FrontRightWheelCollider.brakeTorque = currentBreakForce;
        RearLeftWheelCollider.brakeTorque = currentBreakForce;
        RearRightWheelCollider.brakeTorque = currentBreakForce;
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteeringAngle * horizontalInput;
        FrontLeftWheelCollider.steerAngle = currentSteerAngle;
        FrontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(FrontLeftWheelCollider, FrontLeftWheelTransform);
        UpdateSingleWheel(FrontRightWheelCollider, FrontRightWheelTransform);
        UpdateSingleWheel(RearLeftWheelCollider, RearLeftWheelTransform);
        UpdateSingleWheel(RearRightWheelCollider, RearRightWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        spawnManager.SpawnTriggered();
    }

    private void HandlePathFollowing()
    {
        if (currentWaypointIndex >= waypoints.Count)
            return;

        Vector3 targetPosition = waypoints[currentWaypointIndex].position;
        Vector3 directionToTarget = targetPosition - transform.position;
        float distanceToTarget = directionToTarget.magnitude;

        // Check if the car is close to the current waypoint
        if (distanceToTarget < 1f)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Count)
            {
                currentWaypointIndex = 0; // Loop back to the first waypoint or stop if desired
            }
        }

        // Calculate steering angle to the target waypoint
        Vector3 targetDirection = directionToTarget.normalized;
        float angleToTarget = Vector3.SignedAngle(transform.forward, targetDirection, Vector3.up);
        horizontalInput = Mathf.Clamp(angleToTarget / maxSteeringAngle, -1f, 1f);

        // Adjust vertical input based on desired speed
        verticalInput = movementSpeed / motorForce;
    }
}
