using Car.Modules;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform[] rayPoints;
    [SerializeField] private Transform[] steeringWheels;
    [SerializeField] private LayerMask driveableLayer;

    [Header("Suspension Settings")]
    [SerializeField] private float springStiffness = 40000f;
    [SerializeField] private float damperStiffness = 4000f;
    [SerializeField] private float restLength = 1.2f;
    [SerializeField] private float springTravel = 0.4f;
    [SerializeField] private float wheelRadius = 0.35f;

    [Header("Steering Settings")]
    [SerializeField] private float maxSteerAngle = 35f;
    [SerializeField] private float steerSpeed = 6f;
    [SerializeField] private float turnTorque = 12f;
    [Tooltip("Fraction of sideways velocity cancelled every FixedUpdate (0-1). Close to 1 = no slide.")]
    [SerializeField] private float gripMultiplier = 0.95f;
    [Tooltip("Grip used instead of gripMultiplier while the handbrake is held. Lower = more slide.")]
    [SerializeField] private float handbrakeGripMultiplier = 0.15f;
    [SerializeField] private float minSteeringSpeed = 0.7f;
    [SerializeField] private float steeringDamping = 4f;

    [Header("Motor Settings")]
    [SerializeField] private float maxMotorForce = 2000f;
    [SerializeField] private float brakeForce = 1500f;
    [SerializeField] private float reverseMultiplier = 0.6f;
    [SerializeField] private float accelerationMultiplier = 1f;
    [SerializeField] private float decelerationMultiplier = 0.8f;
    [SerializeField] private float handbrakeForce = 1200f;
    [SerializeField] private float rollingResistance = 0.08f;
    [SerializeField] private Transform[] rearWheelTransforms;

    private CarSuspensionModule suspensionModule;
    private CarSteeringModule steeringModule;
    private CarMotorModule motorModule;

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        suspensionModule = new CarSuspensionModule(
            rb,
            rayPoints,
            driveableLayer,
            springStiffness,
            damperStiffness,
            restLength,
            springTravel,
            wheelRadius);

        steeringModule = new CarSteeringModule(
            rb,
            transform,
            steeringWheels,
            maxSteerAngle,
            steerSpeed,
            turnTorque,
            gripMultiplier,
            handbrakeGripMultiplier,
            minSteeringSpeed,
            steeringDamping);

        motorModule = new CarMotorModule(
            rb,
            transform,
            maxMotorForce,
            brakeForce,
            reverseMultiplier,
            accelerationMultiplier,
            decelerationMultiplier,
            handbrakeForce,
            rollingResistance,
            rearWheelTransforms);
    }

    private void FixedUpdate()
    {
        ProcessInput();
        suspensionModule.Tick();
    }

    private void ProcessInput()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        float brakeInput = Input.GetAxis("Jump");
        bool handbrakeInput = Input.GetKey(KeyCode.Space);

        steeringModule.SetSteerInput(horizontalInput, handbrakeInput);
        motorModule.SetDriveInput(verticalInput);
        motorModule.ApplyBraking(brakeInput);
        motorModule.ApplyHandbrake(handbrakeInput);
    }

    private void OnDrawGizmosSelected()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        if (rb == null)
        {
            return;
        }

        Gizmos.color = Color.cyan;
        Vector3 forwardPoint = transform.position + transform.forward * 1.5f;
        Gizmos.DrawWireSphere(forwardPoint, 0.12f);
        Gizmos.DrawLine(transform.position, forwardPoint);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position - transform.forward * 0.8f, 0.1f);
    }
}