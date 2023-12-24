using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using static RoadPart;

public class RoadGenerator : MonoBehaviour
{

    [Tooltip("List of the different GameObject to use to create the road")]
    public List<RoadPart> roadPartBlueprintList = new List<RoadPart>();

    // Pending list of Road Part ready to be instanciate in the scene
    private List<GameObject> roadPartPendingList = new List<GameObject>();

    void Start()
    {
        roadPartPendingList = CreatePendingList(roadPartBlueprintList);
        ShuffleList(roadPartPendingList); // TODO: return a shuffled list
        GenerateRoad(); // TODO: generate road from list in param
    }

    /// <summary>
    /// Create a list of road parts to use to generate the road, according to the defined road part iteration
    /// </summary>
    /// <param name="list">The RoadPart blueprint list to use to create the pending list</params>
    /// <returns>The pending GameObject list</returns>

    private List<GameObject> CreatePendingList(List<RoadPart> list)
    {
        List<GameObject> pendingList = new List<GameObject>();

        // Iterate in the blueprint list
        foreach (RoadPart roadPart in list)
        {
            // Add the road part to the pending list as many times as its iterations value
            for (int i = 0; i < roadPart.iterations; i++)
            {
                pendingList.Add(roadPart.gameObject);
            }
        }

        return pendingList;
    }

    /// <summary>
    /// Shuffle a list of items
    /// </summary>
    /// <param name="list">The list to shuffle</params>
    private void ShuffleList(List<GameObject> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            GameObject temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    /// <summary>
    /// Generates a road from the roadParts list.<br/>
    /// Instantiate them in a sequence, connecting the end of the previous roadPart to the start of the next one.
    /// </summary>
    private void GenerateRoad()
    {
        // Generate the road
        for (int i = 0; i < roadPartPendingList.Count; i++)
        {
            Debug.Log($"Step {i}");
            int roadPartInstancesCount = gameObject.transform.childCount;
            GameObject nextRoadPart = spawnRoadPartFromList(roadPartPendingList, i);

            if (roadPartInstancesCount > 0)
            {
                GameObject previousRoadPart = gameObject.transform.GetChild(roadPartInstancesCount - 1).gameObject;
                ConnectRoadParts(previousRoadPart, nextRoadPart);

                // Keep track of the potential road parts as replacement for this step
                List<GameObject> availableRoadParts = roadPartBlueprintList.Select(roadPart => roadPart.gameObject).ToList();
                if (isColliding(nextRoadPart))
                {
                    GameObject badRoadPart = nextRoadPart;

                    // Replace the bad road part with another one
                    Debug.Log($"Remove {badRoadPart.name}");
                    roadPartPendingList.RemoveAt(i);
                    availableRoadParts.Remove(badRoadPart);
                    DestroyImmediate(badRoadPart);

                    int randomIndex = Random.Range(0, availableRoadParts.Count);
                    roadPartPendingList.Insert(i, availableRoadParts[randomIndex].gameObject);
                    Debug.Log($"New potential road part: {availableRoadParts[randomIndex].gameObject.name}");

                    nextRoadPart = spawnRoadPartFromList(roadPartPendingList, i);

                    ConnectRoadParts(previousRoadPart, nextRoadPart);
                    isColliding(nextRoadPart);
                }
            }
        }
    }

    /// <summary>
    /// Instanciatee and place the next road part
    /// </summary>
    /// <param name="list">List that contains the road part to instantiate</param>
    /// <param name="index">Index of the road part to instantiate</param>
    /// <returns>The spawned RoadPart GameObject</returns>
    private GameObject spawnRoadPartFromList(List<GameObject> list, int index)
    {
        // Instantiate the road part
        GameObject roadPartToInstantiate = list[index];
        GameObject roadPart = Instantiate(roadPartToInstantiate.gameObject, gameObject.transform.position, Quaternion.identity);

        // Put it at the road origin
        roadPart.transform.SetParent(gameObject.transform);
        roadPart.name = $"{roadPart.name} {index}";

        return roadPart;
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
    private bool isColliding(GameObject roadPart)
    {
        // Note: at this point, we need to sync the physic engine.
        // If autoSyncTransforms is not enable in project settings, use Physics.SyncTransforms();

        bool isColliding = false;

        // List all colliders that overlap this roadpart's collider
        BoxCollider roadPartCollider = roadPart.GetComponent<BoxCollider>();
        Collider[] colliders = Physics.OverlapBox(roadPartCollider.bounds.center, roadPartCollider.bounds.extents / 2, roadPart.transform.rotation);

        foreach (Collider collider in colliders)
        {
            // There is collision only if it's not own road part collider and the object is a road part
            if (collider != roadPartCollider && collider.gameObject.tag == "RoadPart")
            {
                Debug.Log($"{roadPart.name} collide with {collider.gameObject.name}");
                isColliding = true;
            }
        }

        return isColliding;
    }
}