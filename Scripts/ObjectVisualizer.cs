/*using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

public class ObjectVisualizer : MonoBehaviour
{
    public GameObject carPrefab;
    public GameObject truckPrefab;
    public GameObject bikePrefab;
    public GameObject vanPrefab;
    public Transform carTransform;
    private List<FrameData> frames;
    private int currentFrameIndex = 0;
    private Dictionary<int, SpawnedTraffic> activeObjects = new Dictionary<int, SpawnedTraffic>(); // Track active objects by their ID
    private float startTime;

    void Start()
    {
        LoadJsonData();
        if (frames != null && frames.Count > 0)
        {
            startTime = Time.time;
            InvokeRepeating(nameof(UpdateScene), 0f, 0.00002f); // Call UpdateScene every 0.03 seconds
        }
        else
        {
            Debug.LogError("No frames to process. JSON data might be missing or incorrectly formatted.");
        }
    }

    void LoadJsonData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("ObjectData");

        if (jsonFile != null)
        {
            try
            {
                var root = JsonConvert.DeserializeObject<RootObject>(jsonFile.text);
                frames = root.objects;

                Debug.Log("Successfully loaded and parsed JSON data. Number of frames: " + frames.Count);
            }
            catch (JsonException e)
            {
                Debug.LogError("Failed to parse JSON data: " + e.Message);
            }
        }
        else
        {
            Debug.LogError("Failed to load JSON file from Resources");
        }
    }

    void UpdateScene()
    {
        if (frames == null || frames.Count == 0)
        {
            Debug.LogError("No frames available to process.");
            return;
        }

        if (currentFrameIndex >= frames.Count)
        {
            CancelInvoke(nameof(UpdateScene)); // Stop the loop if there are no more frames
            return;
        }

        float elapsedTime = Time.time - startTime;

        while (currentFrameIndex < frames.Count && frames[currentFrameIndex].Time <= elapsedTime)
        {
            ProcessFrame(frames[currentFrameIndex]);
            currentFrameIndex++;
        }
    }

    void ProcessFrame(FrameData frame)
    {
        HashSet<int> currentObjectIds = new HashSet<int>();

        foreach (ObjectData objData in frame.objectList)
        {
            int objectId = (int)objData.objectID;
            currentObjectIds.Add(objectId);

            if (activeObjects.ContainsKey(objectId))
            {
                UpdateTrafficPosition(activeObjects[objectId], objData);
            }
            else
            {
                Debug.Log($"Instantiating object ID: {objectId}");

                GameObject prefab = GetPrefab(objData.objectClass);
                if (prefab != null)
                {
                    Vector3 spawnPosition = carTransform.position + new Vector3(objData.lateralDistance, 0, objData.longitudinalDistance);
                    spawnPosition.y = carTransform.position.y; // Ensure it stays at the same height as the car

                    Quaternion spawnRotation = Quaternion.Euler(0, objData.heading * Mathf.Rad2Deg, 0);
                    GameObject trafficObject = Instantiate(prefab, spawnPosition, spawnRotation);

                    SpawnedTraffic newTraffic = new SpawnedTraffic(trafficObject, new TrafficData(GetObjectType(objData.objectClass), spawnPosition - carTransform.position, Vector3.zero, Vector3.forward, objData.relativeVelocityLong));
                    activeObjects[objectId] = newTraffic;

                    Debug.Log($"Object ID: {objData.objectID} instantiated at position: {trafficObject.transform.position}");
                }
            }
        }

        List<int> idsToRemove = new List<int>();
        foreach (int id in activeObjects.Keys)
        {
            if (!currentObjectIds.Contains(id))
            {
                Destroy(activeObjects[id].gameObject);
                idsToRemove.Add(id);
                Debug.Log("Removed object ID: " + id);
            }
        }

        foreach (int id in idsToRemove)
        {
            activeObjects.Remove(id);
        }
    }

    private void UpdateTrafficPosition(SpawnedTraffic traffic, ObjectData data)
    {
        Vector3 targetPosition = carTransform.position + new Vector3(data.lateralDistance, 0, data.longitudinalDistance);
        targetPosition.y = carTransform.position.y; // Keep the same Y position

        traffic.gameObject.transform.position = targetPosition;
        traffic.gameObject.transform.rotation = Quaternion.Euler(0, data.heading * Mathf.Rad2Deg, 0);

        Debug.Log($"Updated Object ID: {data.objectID} to position: {traffic.gameObject.transform.position}, rotation: {traffic.gameObject.transform.rotation}");
    }

    private GameObject GetPrefab(int objectClass)
    {
        switch (objectClass)
        {
            case 1: return carPrefab;
            case 2: return truckPrefab;
            // Add more cases if there are more types
            default: return carPrefab;
        }
    }

    private string GetObjectType(int objectClass)
    {
        switch (objectClass)
        {
            case 1: return "car";
            case 2: return "truck";
            // Add more types if necessary
            default: return "car";
        }
    }

    [System.Serializable]
    public class RootObject
    {
        public Metadata metadata;
        public List<FrameData> objects;
    }

    [System.Serializable]
    public class Metadata
    {
        public string topic;
        public string source_file;
        public List<string> columns;
    }

    [System.Serializable]
    public class FrameData
    {
        public float Time;
        public Header header;
        public int numberOfVideoObjects;
        public List<ObjectData> objectList;
    }

    [System.Serializable]
    public class Header
    {
        public int seq;
        public int stamp_secs;
        public int stamp_nsecs;
        public string frame_id;
    }

    [System.Serializable]
    public class ObjectData
    {
        public float objectID;
        public int objectClass;
        public int objectLaneAssignment;
        public float width;
        public float length;
        public float height;
        public float absoluteVelocityLong;
        public float absoluteVelocityLat;
        public float relativeVelocityLong;
        public float relativeVelocityLat;
        public float longitudinalDistance;
        public float lateralDistance;
        public float heading;
        public bool brakeLight;
        public bool leftTurnIndication;
        public bool rightTurnIndication;
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
}

*/


//WORJKSSSSSSS
/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;


public class ObjectVisualizer : MonoBehaviour
{


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

    [System.Serializable]
    public class SpawnedTraffic
    {
        public GameObject trafficGameObject; // Renamed from gameObject to avoid ambiguity
        public TrafficData trafficData; // Renamed from data to avoid ambiguity

        public SpawnedTraffic(GameObject trafficGameObject, TrafficData trafficData)
        {
            this.trafficGameObject = trafficGameObject;
            this.trafficData = trafficData;
        }
    }
    public GameObject carTransform;
    public GameObject carPrefab;
    public GameObject truckPrefab;
    public GameObject bikePrefab;
    public GameObject vanPrefab;

    private List<SpawnedTraffic> spawnedTraffic = new List<SpawnedTraffic>();
    private List<FrameData> frames;
    private int currentFrameIndex = 0;

    void Start()
    {
        LoadJsonData();
        if (frames != null && frames.Count > 0)
        {
            InvokeRepeating(nameof(UpdateScene), 0f, 0.03f); // Update every 0.03 seconds
        }
    }

    void LoadJsonData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("ObjectData");

        if (jsonFile != null)
        {
            var root = JsonConvert.DeserializeObject<RootObject>(jsonFile.text);
            frames = root.objects;

            Debug.Log("Successfully loaded and parsed JSON data. Number of frames: " + frames.Count);
        }
        else
        {
            Debug.LogError("Failed to load JSON file from Resources");
        }
    }

    void UpdateScene()
    {
        if (currentFrameIndex >= frames.Count)
        {
            CancelInvoke(nameof(UpdateScene)); // Stop if there are no more frames
            return;
        }

        FrameData currentFrame = frames[currentFrameIndex];
        RefreshTrafficData(currentFrame);

        currentFrameIndex++;
    }

    private void RefreshTrafficData(FrameData frame)
    {
        // Clear previous objects
        foreach (SpawnedTraffic traffic in spawnedTraffic)
        {
            Destroy(traffic.trafficGameObject);
        }
        spawnedTraffic.Clear();

        // Add new objects from the current frame
        foreach (var objData in frame.objectList)
        {
            string objectType = DetermineObjectType(objData.objectClass);
            Vector3 positionOffset = new Vector3(objData.lateralDistance, carTransform.transform.position.y, objData.longitudinalDistance);
            Vector3 rotationOffset = new Vector3(0, objData.heading, 0);
            Vector3 direction = new Vector3(objData.relativeVelocityLat, 0, objData.relativeVelocityLong);
            float speed = objData.absoluteVelocityLong;

            TrafficData trafficData = new TrafficData(objectType, positionOffset, rotationOffset, direction, speed);
            SpawnTrafficObject(trafficData);
        }
    }

    private string DetermineObjectType(int objectClass)
    {
        switch (objectClass)
        {
            case 1: return "car";
            case 2: return "truck";
            case 3: return "bike";
            case 4: return "van";
            default: return "car"; // Default to car if unknown
        }
    }

    private void SpawnTrafficObject(TrafficData data)
    {
        GameObject prefab = GetPrefab(data.objectType);
        if (prefab != null)
        {
            Vector3 spawnPosition = carTransform.transform.position + data.positionOffset;
            Quaternion spawnRotation = Quaternion.Euler(data.rotationOffset);
            GameObject trafficObject = Instantiate(prefab, spawnPosition, spawnRotation);
            spawnedTraffic.Add(new SpawnedTraffic(trafficObject, data));
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
}

[System.Serializable]
public class RootObject
{
    public Metadata metadata;
    public List<FrameData> objects;
}

[System.Serializable]
public class Metadata
{
    public string topic;
    public string source_file;
    public List<string> columns;
}

[System.Serializable]
public class FrameData
{
    public float Time;
    public Header header;
    public int numberOfVideoObjects;
    public List<ObjectData> objectList;
}

[System.Serializable]
public class Header
{
    public int seq;
    public int stamp_secs;
    public int stamp_nsecs;
    public string frame_id;
}

[System.Serializable]
public class ObjectData
{
    public float objectID;
    public int objectClass;
    public int objectLaneAssignment;
    public float width;
    public float length;
    public float height;
    public float absoluteVelocityLong;
    public float absoluteVelocityLat;
    public float relativeVelocityLong;
    public float relativeVelocityLat;
    public float longitudinalDistance;
    public float lateralDistance;
    public float heading;
    public bool brakeLight;
    public bool leftTurnIndication;
    public bool rightTurnIndication;
}
*/



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ObjectVisualizer : MonoBehaviour
{
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

    [System.Serializable]
    public class SpawnedTraffic
    {
        public GameObject trafficGameObject;
        public TrafficData trafficData;

        public SpawnedTraffic(GameObject trafficGameObject, TrafficData trafficData)
        {
            this.trafficGameObject = trafficGameObject;
            this.trafficData = trafficData;
        }
    }

    public GameObject carTransform;
    public GameObject carPrefab;
    public GameObject truckPrefab;
    public GameObject bikePrefab;
    public GameObject vanPrefab;

    private List<SpawnedTraffic> spawnedTraffic = new List<SpawnedTraffic>();
    private List<FrameData> frames;
    private int currentFrameIndex = 0;

    void Start()
    {
        LoadJsonData();
        if (frames != null && frames.Count > 0)
        {
            InvokeRepeating(nameof(UpdateScene), 0f, 0.03f); // Update every 0.03 seconds
        }
    }

    void LoadJsonData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("ObjectData");

        if (jsonFile != null)
        {
            var root = JsonConvert.DeserializeObject<RootObject>(jsonFile.text);
            frames = root.objects;

            Debug.Log("Successfully loaded and parsed JSON data. Number of frames: " + frames.Count);
        }
        else
        {
            Debug.LogError("Failed to load JSON file from Resources");
        }
    }

    void UpdateScene()
    {
        if (currentFrameIndex >= frames.Count)
        {
            CancelInvoke(nameof(UpdateScene)); // Stop if there are no more frames
            return;
        }

        FrameData currentFrame = frames[currentFrameIndex];
        RefreshTrafficData(currentFrame);

        currentFrameIndex++;
    }

    private void RefreshTrafficData(FrameData frame)
    {
        // Clear previous objects
        foreach (SpawnedTraffic traffic in spawnedTraffic)
        {
            Destroy(traffic.trafficGameObject);
        }
        spawnedTraffic.Clear();

        // Add new objects from the current frame
        foreach (var objData in frame.objectList)
        {
            string objectType = DetermineObjectType(objData.objectClass);
            Vector3 positionOffset = new Vector3(objData.lateralDistance, carTransform.transform.position.y, objData.longitudinalDistance);
            Vector3 rotationOffset = new Vector3(0, objData.heading, 0);
            Vector3 direction = new Vector3(objData.relativeVelocityLat, 0, objData.relativeVelocityLong);
            float speed = objData.absoluteVelocityLong;

            TrafficData trafficData = new TrafficData(objectType, positionOffset, rotationOffset, direction, speed);
            SpawnTrafficObject(trafficData, objData.objectID, objData.lateralDistance, objData.longitudinalDistance);
        }
    }

    private string DetermineObjectType(int objectClass)
    {
        switch (objectClass)
        {
            case 1: return "car";
            case 2: return "truck";
            case 3: return "bike";
            case 4: return "van";
            default: return "car"; // Default to car if unknown
        }
    }

    private void SpawnTrafficObject(TrafficData data, float objectID, float lateralDistance, float longitudinalDistance)
    {
        GameObject prefab = GetPrefab(data.objectType);
        if (prefab != null)
        {
            Vector3 spawnPosition = carTransform.transform.position + data.positionOffset;
            Quaternion spawnRotation = Quaternion.Euler(data.rotationOffset);
            GameObject trafficObject = Instantiate(prefab, spawnPosition, spawnRotation);
            spawnedTraffic.Add(new SpawnedTraffic(trafficObject, data));

            Debug.Log($"Spawned Object ID: {objectID}, Lateral Distance: {lateralDistance}, Longitudinal Distance: {longitudinalDistance}");
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
}

[System.Serializable]
public class RootObject
{
    public Metadata metadata;
    public List<FrameData> objects;
}

[System.Serializable]
public class Metadata
{
    public string topic;
    public string source_file;
    public List<string> columns;
}

[System.Serializable]
public class FrameData
{
    public float Time;
    public Header header;
    public int numberOfVideoObjects;
    public List<ObjectData> objectList;
}

[System.Serializable]
public class Header
{
    public int seq;
    public int stamp_secs;
    public int stamp_nsecs;
    public string frame_id;
}

[System.Serializable]
public class ObjectData
{
    public float objectID;
    public int objectClass;
    public int objectLaneAssignment;
    public float width;
    public float length;
    public float height;
    public float absoluteVelocityLong;
    public float absoluteVelocityLat;
    public float relativeVelocityLong;
    public float relativeVelocityLat;
    public float longitudinalDistance;
    public float lateralDistance;
    public float heading;
    public bool brakeLight;
    public bool leftTurnIndication;
    public bool rightTurnIndication;
}
