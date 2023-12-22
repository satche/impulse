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
        GenerateRoad();
    }

    /// <summary>
    /// Generates a road by randomly selecting road parts from a list
    /// Instantiate them in a sequence, connecting the end of each part to the start of the next part.
    /// </summary>
    private void GenerateRoad()
    {
        // Generate the road
        for (int i = 0; i < roadLength; i++)
        {
            // Take a random road part from the list
            int roadPartIndex = Random.Range(0, roadParts.Count);
            GameObject roadPart = roadParts[roadPartIndex];

            // Instantiate the road part, put it in current GameObject origin
            int roadPartInstanceCount = gameObject.transform.childCount;
            GameObject nextRoadPart = Instantiate(roadPart, gameObject.transform.position, Quaternion.identity);
            nextRoadPart.transform.SetParent(gameObject.transform);

            // If it's not the first road part, put it at the end of the previous one
            if (roadPartInstanceCount > 0)
            {
                GameObject previousRoadPart = gameObject.transform.GetChild(roadPartInstanceCount - 1).gameObject;
                GameObject previousRoadPartEnd = previousRoadPart.transform.Find("End").gameObject;
                GameObject nextRoadPartStart = nextRoadPart.transform.Find("Start").gameObject;

                // Align the start of the next road part to the end of the previous one
                nextRoadPart.transform.rotation = Quaternion.LookRotation(previousRoadPartEnd.transform.forward, previousRoadPartEnd.transform.up);
                Vector3 offset = nextRoadPartStart.transform.position - nextRoadPart.transform.position;
                nextRoadPart.transform.position = previousRoadPartEnd.transform.position - offset;
            }
        }
    }
}