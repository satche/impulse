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
        ShuffleRoadPartList();
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
    }

    /// <summary>
    /// Shuffle the road part list<br/>
    /// Make sure to avoid collisions between road parts
    /// </summary>
    private void ShuffleRoadPartList()
    {
        // Shuffle the list
        for (int i = 0; i < roadPartsToInstanciate.Count; i++)
        {
            GameObject temp = roadPartsToInstanciate[i];
            int randomIndex = Random.Range(i, roadPartsToInstanciate.Count);
            roadPartsToInstanciate[i] = roadPartsToInstanciate[randomIndex];
            roadPartsToInstanciate[randomIndex] = temp;
        }
    }

    /// <summary>
    /// Generates a road from the roadParts list.<br/>
    /// Instantiate them in a sequence, connecting the end of the previous roadPart to the start of the next one.
    /// </summary>
    private void GenerateRoad()
    {
        // Generate the road
        for (int i = 0; i < roadPartsToInstanciate.Count; i++)
        {
            Debug.Log($"Step {i}");
            int roadPartInstancesCount = gameObject.transform.childCount;

            // Take the road part from the list
            GameObject roadPartToInstantiate = roadPartsToInstanciate[i];

            // Instantiate the road part, put it at the road origin
            GameObject nextRoadPart = Instantiate(roadPartToInstantiate.gameObject, gameObject.transform.position, Quaternion.identity);
            nextRoadPart.transform.SetParent(gameObject.transform);
            nextRoadPart.name = $"{nextRoadPart.name} {i}";

            if (roadPartInstancesCount > 0)
            {
                GameObject previousRoadPart = gameObject.transform.GetChild(roadPartInstancesCount - 1).gameObject;
                ConnectRoadParts(previousRoadPart, nextRoadPart);
                CheckCollision(nextRoadPart);
            }
        }
    }

    /// <summary>
    /// Connect the end of the previous road part to the start of the next one
    /// </summary>
    /// <param name="previousRoadPart">The previous road part to connect with</param>
    /// <param name="nextRoadPart">The next road part to connect to the previous one</param>
    private void ConnectRoadParts(GameObject previousRoadPart, GameObject nextRoadPart)
    {
        GameObject previousRoadPartEnd = previousRoadPart.transform.Find("End").gameObject;
        GameObject nextRoadPartStart = nextRoadPart.transform.Find("Start").gameObject;

        // Align the start of the next road part to the end of the previous one (rotation and position)
        nextRoadPart.transform.rotation = Quaternion.LookRotation(previousRoadPartEnd.transform.forward, previousRoadPartEnd.transform.up);
        Vector3 offset = nextRoadPartStart.transform.position - nextRoadPart.transform.position;
        nextRoadPart.transform.position = previousRoadPartEnd.transform.position - offset;
    }

    /// <summary>
    /// Check if the road part collide with any other road part
    /// </summary>
    /// <param name="roadPart">The road part to check</param>
    /// <returns>True if the road part collide with any other road part</returns>
    private bool CheckCollision(GameObject roadPart)
    {
        // List all colliders that overlap this roadpart's collider
        Physics.SyncTransforms();
        BoxCollider roadPartCollider = roadPart.GetComponent<BoxCollider>();
        Collider[] colliders = Physics.OverlapBox(roadPartCollider.bounds.center, roadPartCollider.bounds.extents / 2, roadPart.transform.rotation);

        foreach (Collider collider in colliders)
        {
            if (collider != roadPartCollider && collider.gameObject.tag == "RoadPart")
            {
                Debug.Log($"{roadPart.name} collide with {collider.gameObject.name}");

            }
        }

        return false;
    }
}