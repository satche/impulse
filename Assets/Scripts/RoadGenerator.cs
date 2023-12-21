using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoadGenerator : MonoBehaviour
{
    [Tooltip("How many road parts to create")]
    public int roadLength;

    [Tooltip("List of the different GameObject to use to create the road")]
    public List<GameObject> roadParts;

    private List<GameObject> roadPartsInstances = new List<GameObject>();

    void Start()
    {
        // Generate the road
        for (int i = 0; i < roadLength; i++)
        {
            // Take a random road part from the list
            int random = Random.Range(0, roadParts.Count);
            GameObject roadPart = roadParts[random];

            // Instantiate the road part, put it in current GameObject origin
            int roadPartInstanceCount = gameObject.transform.childCount;
            GameObject roadPartInstance = Instantiate(roadPart, gameObject.transform.position, Quaternion.identity);
            roadPartInstance.transform.SetParent(gameObject.transform);

            // If it's not the first road part, put it at the end of the previous one
            if (roadPartInstanceCount > 0)
            {
                GameObject previousRoadPart = gameObject.transform.GetChild(roadPartInstanceCount - 1).gameObject;
                GameObject previousRoadPartEnd = previousRoadPart.transform.Find("End").gameObject;
                GameObject roadPartInstanceStart = roadPartInstance.transform.Find("Start").gameObject;
                
                roadPartInstance.transform.rotation = previousRoadPartEnd.transform.rotation;
                Vector3 offset = roadPartInstanceStart.transform.position - roadPartInstance.transform.position;
                roadPartInstance.transform.position = previousRoadPartEnd.transform.position - offset;
            }
        }
    }
}