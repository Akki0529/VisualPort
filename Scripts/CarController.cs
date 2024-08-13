using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private Transform FrontLeftWheelTransform;
    [SerializeField] private Transform FrontRightWheelTransform;
    [SerializeField] private Transform RearLeftWheelTransform;
    [SerializeField] private Transform RearRightWheelTransform;

    private void Start()
    {
        // Ensure the car and wheels are positioned correctly at the start
        UpdateWheels();
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(FrontLeftWheelTransform);
        UpdateSingleWheel(FrontRightWheelTransform);
        UpdateSingleWheel(RearLeftWheelTransform);
        UpdateSingleWheel(RearRightWheelTransform);
    }

    private void UpdateSingleWheel(Transform wheelTransform)
    {
        // Here, we'll just align the wheel with the car's current position and rotation.
        Vector3 pos = wheelTransform.position;
        Quaternion rot = wheelTransform.rotation;
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }
}
