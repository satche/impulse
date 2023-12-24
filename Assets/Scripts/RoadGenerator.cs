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
    private List<RoadPart> roadPartPendingList = new List<RoadPart>();

    void Start()
    {
        roadPartPendingList = CreatePendingList(roadPartBlueprintList);
        roadPartPendingList = ShuffleList(roadPartPendingList);
        GenerateRoad(); // TODO: generate road from list in param
    }

    /// <summary>
    /// Create a list of road parts to use to generate the road, according to the defined road part iteration
    /// </summary>
    /// <param name="list">The RoadPart blueprint list to use to create the pending list</params>
    /// <returns>The pending GameObject list</returns>
    private List<RoadPart> CreatePendingList(List<RoadPart> list)
    {
        List<RoadPart> pendingList = new List<RoadPart>();

        // Iterate in the blueprint list
        foreach (RoadPart roadPart in list)
        {
            // Add the road part to the pending list as many times as its iterations value
            for (int i = 0; i < roadPart.iterations; i++)
            {
                pendingList.Add(roadPart);
            }
        }

        return pendingList;
    }

    /// <summary>
    /// Shuffle a list of items
    /// </summary>
    /// <param name="list">The list to shuffle</params>
    /// <returns>The shuffled list</returns>
    private List<RoadPart> ShuffleList(List<RoadPart> list)
    {
        return list.OrderBy(x => Random.value).ToList();
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

                if (isColliding(nextRoadPart))
                {
                    fixRoad(previousRoadPart, nextRoadPart, i);
                }
            }
        }
    }

    private void fixRoad(GameObject previousRoadPart, GameObject nextRoadPart, int i)
    {
        // Keep track of the road parts that has not been uses
        List<RoadPart> availableRoadParts = this.roadPartBlueprintList;

        // Remove the road part that has been used from the available list
        availableRoadParts.Remove(this.roadPartPendingList[i]);

        for (int j = 0; j < availableRoadParts.Count; j++)
        {
            replaceRoadPartInList(nextRoadPart, availableRoadParts, i);
            nextRoadPart = spawnRoadPartFromList(roadPartPendingList, i);
            ConnectRoadParts(previousRoadPart, nextRoadPart);
            if (!isColliding(nextRoadPart)) { break; }
        }

        // If no more road part available and still colliding, step back and try again
        if (availableRoadParts.Count == 0 && isColliding(nextRoadPart))
        {
            // [WIP]
            // this.roadPartPendingList.RemoveAt(i);
            // DestroyImmediate(nextRoadPart);
            // i--;
        }
    }

    /// <summary>
    /// Replace the bad road part with another one in the pending list
    /// </summary>
    /// <param name="badRoadPart">The roadPart to replace</param>
    /// <param name="availableRoadPartsList">The list to go look for another potential roadPart</param>
    /// <param name="index">The index of the roadPart to replace</param>
    void replaceRoadPartInList(GameObject badRoadPart, List<RoadPart> availableRoadPartsList, int index)
    {
        // Check if the list is empty
        if (availableRoadPartsList.Count == 0)
        {
            Debug.Log("No available road parts to replace with.");
            return;
        }

        // Remove bad road part
        this.roadPartPendingList.RemoveAt(index);
        availableRoadPartsList.Remove(availableRoadPartsList.Find(x => $"{x.gameObject.name} {index}" == badRoadPart.name));
        DestroyImmediate(badRoadPart);

        // Replace with another one from the available list
        int randomIndex = Random.Range(0, availableRoadPartsList.Count);
        this.roadPartPendingList.Insert(index, availableRoadPartsList[randomIndex]);
        Debug.Log($"New potential road part: {availableRoadPartsList[randomIndex].gameObject.name} {index}");
    }

    /// <summary>
    /// Instanciatee and place the next road part
    /// </summary>
    /// <param name="list">List that contains the road part to instantiate</param>
    /// <param name="index">Index of the road part to instantiate</param>
    /// <returns>The spawned road part GameObject</returns>
    private GameObject spawnRoadPartFromList(List<RoadPart> list, int index)
    {
        // Instantiate the road part
        RoadPart roadPartToInstantiate = list[index];
        GameObject roadPartToSpawn = Instantiate(roadPartToInstantiate.gameObject, gameObject.transform.position, Quaternion.identity);

        // Put it at the road origin
        roadPartToSpawn.transform.SetParent(gameObject.transform);
        roadPartToSpawn.name = $"{roadPartToSpawn.name} {index}";

        return roadPartToSpawn;
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
        bool isColliding = false;
        Physics.SyncTransforms();

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