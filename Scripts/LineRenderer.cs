using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineGenerator : MonoBehaviour
{
    public int segmentCount = 100;
    public float segmentLength = 30f;
    public float laneWidth = 3.5f;
    public float a = 0.001f;  // Coefficient for x^3
    public float b = 0.002f;  // Coefficient for x^2
    public float c = 0.01f;   // Coefficient for x
    public float d = 0f;      // Constant term

    public LineRenderer centerLineRenderer;
    public LineRenderer leftLineRenderer;
    public LineRenderer rightLineRenderer;

    private float halfLaneWidth;

    void Start()
    {
        halfLaneWidth = laneWidth / 2f;
        InitializeLineRenderer(centerLineRenderer);
        InitializeLineRenderer(leftLineRenderer);
        InitializeLineRenderer(rightLineRenderer);

        GenerateLines();
    }

    void InitializeLineRenderer(LineRenderer lr)
    {
        lr.positionCount = segmentCount + 1;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
    }

    void GenerateLines()
    {
        Vector3[] centerPoints = new Vector3[segmentCount + 1];
        Vector3[] leftPoints = new Vector3[segmentCount + 1];
        Vector3[] rightPoints = new Vector3[segmentCount + 1];

        for (int i = 0; i <= segmentCount; i++)
        {
            float t = i / (float)segmentCount;
            float z = t * segmentLength;
            float x = EvaluatePolynomial(z);

            centerPoints[i] = new Vector3(x, 0, z);
            Vector3 offset = CalculateOffset(x, z, halfLaneWidth);
            leftPoints[i] = new Vector3(x, 0, z) - offset;
            rightPoints[i] = new Vector3(x, 0, z) + offset;
        }

        centerLineRenderer.SetPositions(centerPoints);
        leftLineRenderer.SetPositions(leftPoints);
        rightLineRenderer.SetPositions(rightPoints);
    }

    float EvaluatePolynomial(float z)
    {
        return a * Mathf.Pow(z, 3) + b * Mathf.Pow(z, 2) + c * z + d;
    }

    Vector3 CalculateOffset(float x, float z, float xOffset)
    {
        float delta = 0.1f;
        float dx = EvaluatePolynomial(z + delta) - EvaluatePolynomial(z - delta);
        float dz = 2 * delta;

        Vector3 perpendicular = new Vector3(-dz, 0, dx).normalized;
        return perpendicular * xOffset;
    }
}
