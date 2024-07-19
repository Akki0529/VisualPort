using UnityEngine;

public class TrajectoryPredictor : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public int segmentCount = 50;
    public float segmentLength = 1f;

    // Manually set polynomial coefficients for a slight curve to the left
    private float a = -0.01f;
    private float b = 0f;
    private float c = 0.1f;
    private float d = 0f;

    void Start()
    {
        lineRenderer.positionCount = segmentCount;
        UpdateTrajectory();
    }

    public void UpdateTrajectory()
    {
        Vector3[] points = new Vector3[segmentCount];
        float z = 0f;

        for (int i = 0; i < segmentCount; i++)
        {
            float x = CalculatePolynomial(z);
            points[i] = new Vector3(x, 0, z);
            z += segmentLength;
        }

        lineRenderer.SetPositions(points);
    }

    float CalculatePolynomial(float z)
    {
        return a * Mathf.Pow(z, 2) + b * z + c + d;
    }

    public Vector3[] GetTrajectoryPoints()
    {
        Vector3[] points = new Vector3[segmentCount];
        lineRenderer.GetPositions(points);
        return points;
    }
}
