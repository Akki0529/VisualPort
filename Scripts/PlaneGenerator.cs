using System.Collections.Generic;
using UnityEngine;

public class PlaneGenerator : MonoBehaviour
{
    public GameObject planePrefab;
    public float segmentLength = 30f;
    public float laneWidth = 3.5f;
    public int initialSegmentCount = 10;

    private Queue<GameObject> planeSegments = new Queue<GameObject>();
    private Transform carTransform;
    private Vector3 lastPlaneEndPosition;
    private Vector3 lastPlaneForward;

    void Start()
    {
        carTransform = GetComponent<Transform>();
        GenerateInitialRoad();
    }

    void Update()
    {
        // Check if we need to generate a new segment
        if (Vector3.Distance(carTransform.position, lastPlaneEndPosition) < segmentLength)
        {
            GenerateNewSegment();
            RemoveOldSegment();
        }

       
    }

    private void GenerateInitialRoad()
    {
        lastPlaneForward = transform.forward;
        for (int i = 0; i < initialSegmentCount; i++)
        {
            Vector3 position = transform.position + lastPlaneForward * i * segmentLength;
            Quaternion rotation = Quaternion.LookRotation(lastPlaneForward);
            GameObject plane = Instantiate(planePrefab, position, rotation);
            plane.transform.localScale = new Vector3(laneWidth, 1, segmentLength);
            plane.transform.parent = transform;

            planeSegments.Enqueue(plane);

            if (i == initialSegmentCount - 1)
            {
                lastPlaneEndPosition = position;
            }
        }
    }

    private void GenerateNewSegment()
    {
        lastPlaneEndPosition += lastPlaneForward * segmentLength;
        Quaternion rotation = Quaternion.LookRotation(lastPlaneForward);
        GameObject plane = Instantiate(planePrefab, lastPlaneEndPosition, rotation);
        plane.transform.localScale = new Vector3(laneWidth, 1, segmentLength);
        plane.transform.parent = transform;

        planeSegments.Enqueue(plane);

        // Update the last plane's forward direction based on the car's movement
        lastPlaneForward = carTransform.forward;
    }

    private void RemoveOldSegment()
    {
        if (planeSegments.Count > initialSegmentCount)
        {
            GameObject oldPlane = planeSegments.Dequeue();
            Destroy(oldPlane);
        }
    }
}
