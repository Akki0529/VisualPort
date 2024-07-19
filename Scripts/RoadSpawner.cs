using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class RoadSpawner : MonoBehaviour
{
    public List<GameObject> roads;

    public List<GameObject> typeRoads;

    private float offset = 40f;

    // Start is called before the first frame update
    void Start()
    {
        if (roads != null && roads.Count > 0)
        {
            roads = roads.OrderBy(r => r.transform.position.z).ToList();
        }

    }

    public void MoveRoad()
    {
        GameObject moveRoad = roads[0];
        roads.Remove(moveRoad);
        float newZ = roads[roads.Count - 1].transform.position.z + offset;
        moveRoad.transform.position = new Vector3(0, 0, newZ);
        moveRoad.SetActive(false);
        roads.Add(moveRoad);

    }

    public void SpawnNewRoad(int x)
    {
        if (x == 1) //Straight road 1 lane
        {

        }
        else if (x == 2) //curve left
        {

        }
        else if (x == 3) //cruve right 
        {

        }
    }


}
