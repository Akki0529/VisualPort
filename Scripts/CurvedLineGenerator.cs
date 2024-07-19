using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CurvedLineGenerator : MonoBehaviour
{
    public int segmentCount = 100;  // Number of segments to create the curve
    public float length = 40f;     // Total length of the curve
    public float a = 0.001f;        // Coefficient for x^3
    public float b = 0.002f;        // Coefficient for x^2
    public float c = 0.01f;         // Coefficient for x
    public float d = 0f;            // Constant term
    public Material lineMaterial;   // Material for the LineRenderer
    public Color lineColor = Color.white; // Color of the line
    public float lineWidth = 0.1f;  // Width of the line
    public GameObject planePrefab;  // Plane prefab to use for the road

    private LineRenderer lineRenderer;
    private LineRenderer leftLineRenderer;
    private LineRenderer rightLineRenderer;

    private float halfWidth = 1.75f;
    public List<Transform> waypoints = new List<Transform>();

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        InitializeLineRenderer(lineRenderer);
        GenerateCurvedLine(lineRenderer, 0);

        // Create left and right line renderers
        CreateParallelLineRenderers();

        // Generate road using the plane prefab
        GenerateRoad();

        // Generate waypoints
        GenerateWaypoints();
    }

    void InitializeLineRenderer(LineRenderer lr)
    {
        if (lineMaterial != null)
        {
            lr.material = lineMaterial;
        }
        else
        {
            lr.material = new Material(Shader.Find("Sprites/Default")); // Default material
        }

        lr.startColor = lineColor;
        lr.endColor = lineColor;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
    }

    void CreateParallelLineRenderers()
    {
        // Create left LineRenderer
        GameObject leftLineObject = new GameObject("LeftLine");
        leftLineRenderer = leftLineObject.AddComponent<LineRenderer>();
        InitializeLineRenderer(leftLineRenderer);
        leftLineObject.transform.parent = transform;

        // Create right LineRenderer
        GameObject rightLineObject = new GameObject("RightLine");
        rightLineRenderer = rightLineObject.AddComponent<LineRenderer>();
        InitializeLineRenderer(rightLineRenderer);
        rightLineObject.transform.parent = transform;

        // Generate the curved lines for left and right LineRenderers
        GenerateCurvedLine(leftLineRenderer, -halfWidth);
        GenerateCurvedLine(rightLineRenderer, halfWidth);
    }

    public void GenerateCurvedLine(LineRenderer lr, float xOffset)
    {
        lr.positionCount = segmentCount + 1;  // Set the number of points in the LineRenderer
        Vector3[] points = new Vector3[segmentCount + 1];

        for (int i = 0; i <= segmentCount; i++)
        {
            float t = i / (float)segmentCount;
            float z = t * length;
            float x = EvaluatePolynomial(z);
            Vector3 offset = CalculateOffset(x, z, xOffset);
            points[i] = new Vector3(x, 0, z) + offset;
        }

        lr.SetPositions(points);
    }

    Vector3 CalculateOffset(float x, float z, float xOffset)
    {
        float delta = 0.1f;
        float dx = EvaluatePolynomial(z + delta) - EvaluatePolynomial(z - delta);
        float dz = 2 * delta;

        Vector3 perpendicular = new Vector3(-dz, 0, dx).normalized;
        return perpendicular * xOffset;
    }

    float EvaluatePolynomial(float z)
    {
        // Evaluate the polynomial a*z^3 + b*z^2 + c*z + d for the x-coordinate
        return a * Mathf.Pow(z, 3) + b * Mathf.Pow(z, 2) + c * z + d;
    }

    void GenerateRoad()
    {
        if (planePrefab == null)
        {
            Debug.LogError("Plane prefab is not assigned!");
            return;
        }

        for (int i = 0; i < segmentCount; i++)
        {
            Vector3 startPos = lineRenderer.GetPosition(i);
            Vector3 endPos = lineRenderer.GetPosition(i + 1);

            Vector3 direction = (endPos - startPos).normalized;
            float segmentLength = Vector3.Distance(startPos, endPos);

            Vector3 position = (startPos + endPos) / 2;
            Quaternion rotation = Quaternion.LookRotation(direction);

            GameObject plane = Instantiate(planePrefab, position, rotation);
            plane.transform.localScale = new Vector3(3.5f, 0.01f, segmentLength);
            plane.transform.parent = transform;
        }
    }

    void GenerateWaypoints()
    {
        for (int i = 0; i <= segmentCount; i++)
        {
            GameObject waypoint = new GameObject("Waypoint" + i);
            waypoint.transform.position = lineRenderer.GetPosition(i);
            waypoints.Add(waypoint.transform);
        }
    }
}
