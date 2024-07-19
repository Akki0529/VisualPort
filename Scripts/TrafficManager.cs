using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrafficData
{
    public string objectType;
    public Vector3 positionOffset;
    public Vector3 rotationOffset;
    public Vector3 direction;
    public float speed;

    public TrafficData(string objectType, Vector3 positionOffset, Vector3 rotationOffset, Vector3 direction, float speed)
    {
        this.objectType = objectType;
        this.positionOffset = positionOffset;
        this.rotationOffset = rotationOffset;
        this.direction = direction;
        this.speed = speed;
    }
}

public class TrafficManager : MonoBehaviour
{
    public GameObject mainCar;
    public GameObject carPrefab;
    public GameObject truckPrefab;
    public GameObject bikePrefab;
    public GameObject vanPrefab;

    private List<SpawnedTraffic> spawnedTraffic = new List<SpawnedTraffic>();

    // Start is called before the first frame update
    void Start()
    {
        List<TrafficData> sampleData = GenerateSampleData();
        SpawnTraffic(sampleData);
        StartCoroutine(UpdateTrafficPositionsCoroutine());
    }

    private List<TrafficData> GenerateSampleData()
    {
        List<TrafficData> sampleData = new List<TrafficData>();

        // Example data: A car overtaking the main car and then stops
        //sampleData.Add(new TrafficData("car", new Vector3(5, 0, 10), new Vector3(0, 0, 0), Vector3.forward, 5f));
        /*sampleData.Add(new TrafficData("truck", new Vector3(-3, 0, 15), new Vector3(0, 0, 0), Vector3.forward, 3f));
        sampleData.Add(new TrafficData("bike", new Vector3(2, 0, 5), new Vector3(0, 0, 0), Vector3.forward, 4f));
        sampleData.Add(new TrafficData("van", new Vector3(8, 0, 20), new Vector3(0, 0, 0), Vector3.forward, 2f));
*/
        return sampleData;
    }

    private void SpawnTraffic(List<TrafficData> trafficDataList)
    {
        foreach (TrafficData data in trafficDataList)
        {
            GameObject prefab = GetPrefab(data.objectType);
            if (prefab != null)
            {
                Vector3 spawnPosition = mainCar.transform.position + data.positionOffset;
                Quaternion spawnRotation = Quaternion.Euler(data.rotationOffset);
                GameObject trafficObject = Instantiate(prefab, spawnPosition, spawnRotation);
                spawnedTraffic.Add(new SpawnedTraffic(trafficObject, data));
            }
        }
    }

    private GameObject GetPrefab(string objectType)
    {
        switch (objectType)
        {
            case "car": return carPrefab;
            case "truck": return truckPrefab;
            case "van": return vanPrefab;
            case "bike": return bikePrefab;
            default: return null;
        }
    }

    private IEnumerator UpdateTrafficPositionsCoroutine()
    {
        while (true)
        {
            foreach (SpawnedTraffic traffic in spawnedTraffic)
            {
                Vector3 targetPosition = mainCar.transform.position + traffic.data.positionOffset;
                if (traffic.data.objectType == "car")

                {
                    // Move the car forward until it overtakes the main car, then stop
                    float distance = Vector3.Distance(traffic.gameObject.transform.position, targetPosition);
                    if (distance > 1f) // Adjust the distance threshold as needed
                    {
                        traffic.gameObject.transform.position = Vector3.MoveTowards(traffic.gameObject.transform.position, targetPosition, traffic.data.speed * Time.deltaTime);
                    }
                    else
                    {
                        // Car has overtaken the main car and stops
                        traffic.data.speed = 0;
                    }
                }
                else
                {
                    traffic.gameObject.transform.position = targetPosition;
                }
            }
            yield return new WaitForSeconds(0.01f); // Update every 0.01 seconds
        }
    }

    // Optional: If you want to manually stop the coroutine
    private void OnDestroy()
    {
        StopCoroutine(UpdateTrafficPositionsCoroutine());
    }
}

public class SpawnedTraffic
{
    public GameObject gameObject;
    public TrafficData data;

    public SpawnedTraffic(GameObject gameObject, TrafficData data)
    {
        this.gameObject = gameObject;
        this.data = data;
    }
}
