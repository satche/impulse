using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static RoadPart;

public class RoadGenerator : MonoBehaviour
{

    [Tooltip("List of the different GameObject to use to create the road")]
    public List<RoadPart> roadParts = new List<RoadPart>();
    private List<GameObject> roadPartsToInstanciate = new List<GameObject>();

    void Start()
    {
        CreateRoadPartList();
        GenerateRoad();
    }

    /// <summary>
    /// Create a list of road parts to use to generate the road
    /// </summary>
    private void CreateRoadPartList()
    {
        // Iterate in the roadParts list
        foreach (RoadPart roadPart in roadParts)
        {
            // Add the road part to the list as many times as the iterations value
            for (int i = 0; i < roadPart.iterations; i++)
            {
                roadPartsToInstanciate.Add(roadPart.gameObject);
            }
        }

        // Shuffle the list
        for (int i = roadPartsToInstanciate.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            GameObject temp = roadPartsToInstanciate[i];
            roadPartsToInstanciate[i] = roadPartsToInstanciate[randomIndex];
            roadPartsToInstanciate[randomIndex] = temp;
        }
    }

    /// <summary>
    /// Generates a road by using the list selecting road parts from a list
    /// Instantiate them in a sequence, connecting the end of each part to the start of the next part.
    /// </summary>
    private void GenerateRoad()
    {
        // Generate the road
        for (int i = 0; i < roadPartsToInstanciate.Count; i++)
        {
            int roadPartInstancesCount = gameObject.transform.childCount;

            // Take the road part from the list
            GameObject roadPart = roadPartsToInstanciate[i];

            // Instantiate the road part, put it at the road origin
            GameObject nextRoadPart = Instantiate(roadPart.gameObject, gameObject.transform.position, Quaternion.identity);
            nextRoadPart.transform.SetParent(gameObject.transform);

            // Put this road part at the end of the previous one
            if (roadPartInstancesCount > 0)
            {
                GameObject previousRoadPart = gameObject.transform.GetChild(roadPartInstancesCount - 1).gameObject;
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