using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using static RoadPart;

public class RoadGenerator : MonoBehaviour
{
    [Tooltip("The first road part, used as a starting point for the road")]
    public RoadPart roadPartStart;

    [Tooltip("List of the different GameObject to use to create the road")]
    public List<RoadPart> roadPartBlueprintList = new List<RoadPart>();

    [Tooltip("The last road part, used as a closing point for the road")]
    public RoadPart roadPartEnd;

    // Pending list of Road Part ready to be instanciate in the scene
    private List<RoadPart> roadPartPendingList = new List<RoadPart>();

    void Start()
    {
        roadPartPendingList = CreatePendingList(roadPartBlueprintList);
        roadPartPendingList = ShuffleList(roadPartPendingList);
        GenerateRoad();
    }

    /// <summary>
    /// Create a list of road parts to use to generate the road, according to the defined road part iteration
    /// </summary>
    /// <param name="list">The RoadPart blueprint list to use to create the pending list</params>
    /// <returns>The pending GameObject list</returns>
    private List<RoadPart> CreatePendingList(List<RoadPart> list)
    {
        List<RoadPart> pendingList = new List<RoadPart>();

        // Add the first and last road part to the pending list
        pendingList.Insert(0, roadPartStart);

        // Iterate in the blueprint list
        foreach (RoadPart roadPart in list)
        {
            // Add the road part to the pending list as many times as its iterations value
            for (int i = 0; i < roadPart.iterations; i++)
            {
                pendingList.Add(roadPart);
            }
        }

        // Add the last road part to the pending list
        pendingList.Add(roadPartEnd);

        return pendingList;
    }

    /// <summary>
    /// Shuffle a list of items
    /// </summary>
    /// <param name="list">The list to shuffle</params>
    /// <returns>The shuffled list</returns>
    private List<RoadPart> ShuffleList(List<RoadPart> list)
    {
        // Extract temporarly the first and last road part
        RoadPart firstRoadPart = list[0];
        RoadPart lastRoadPart = list[list.Count - 1];
        list.RemoveAt(0);
        list.RemoveAt(list.Count - 1);

        // Shuffle the list
        List<RoadPart> shuffledList = list.OrderBy(x => Random.value).ToList();

        // Put back the first and last road part
        shuffledList.Insert(0, firstRoadPart);
        shuffledList.Add(lastRoadPart);

        return shuffledList;
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
            if (roadPartInstancesCount == 0) { continue; }
            GameObject previousRoadPart = gameObject.transform.GetChild(roadPartInstancesCount - 1).gameObject;

            ConnectRoadParts(previousRoadPart, nextRoadPart);

            if (isColliding(nextRoadPart))
            {
                fixRoad(previousRoadPart, nextRoadPart, i, 0);
            }

        }
    }

    /// <summary>
    /// Fix the road by replacing the bad road part with another one from the pending list
    /// </summary>
    /// <param name="previousRoadPart">The previous road part in the scene</param>
    /// <param name="badRoadPart">The bad road part that collide with another one</param>
    /// <param name="i">Current step in generation</param>
    /// <param name="recursionCount">Number of time the function has been called recursively</param>
    private void fixRoad(GameObject previousRoadPart, GameObject badRoadPart, int i, int recursionCount = 0)
    {
        Debug.Log($"Fixing road at step {i}");
        // Keep track of the road parts that has not been uses
        List<RoadPart> availableRoadParts = new List<RoadPart>(roadPartBlueprintList);

        for (int j = availableRoadParts.Count; j > 0; j--)
        {
            replaceRoadPartInList(badRoadPart, availableRoadParts, i);
            badRoadPart = spawnRoadPartFromList(roadPartPendingList, i);
            ConnectRoadParts(previousRoadPart, badRoadPart);
            if (!isColliding(badRoadPart)) { break; }
        }

        // If no more road part available and still colliding, step back and try again
        if (availableRoadParts.Count == 0 && isColliding(badRoadPart))
        {
            Debug.Log("Can't fix road, take one step back");
            DestroyImmediate(badRoadPart);
            GameObject oldPreviousRoadPart = gameObject.transform.GetChild(gameObject.transform.childCount - 2).gameObject;
            GameObject oldNextRoadPart = gameObject.transform.GetChild(gameObject.transform.childCount - 1).gameObject;
            if (recursionCount > 5)
            {
                Debug.Log("Too many recursion, abort");
                // Empty the pending list to stop the generation
                this.roadPartPendingList.Clear();
                return;
            }
            fixRoad(oldPreviousRoadPart, oldNextRoadPart, i - 1, recursionCount + 1);
        }
    }

    /// <summary>
    /// Replace the bad road part with another one in the pending list
    /// </summary>
    /// <param name="badRoadPart">The roadPart to replace</param>
    /// <param name="availableRoadPartsList">The list to go look for another potential roadPart</param>
    /// <param name="index">The index of the roadPart to replace</param>
    private void replaceRoadPartInList(GameObject badRoadPart, List<RoadPart> availableRoadPartsList, int index)
    {
        // Remove bad road part
        this.roadPartPendingList.RemoveAt(index);
        availableRoadPartsList.Remove(availableRoadPartsList.Find(x => $"{x.gameObject.name} {index}" == badRoadPart.name));
        DestroyImmediate(badRoadPart);

        // Check if the list is empty
        if (availableRoadPartsList.Count == 0) { return; }

        // Replace with another one from the available list
        int randomIndex = Random.Range(0, availableRoadPartsList.Count);
        this.roadPartPendingList.Insert(index, availableRoadPartsList[randomIndex]);
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
        roadPartToSpawn.name = $"{roadPartToInstantiate.gameObject.name} {index}";

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
        Collider[] colliders = Physics.OverlapBox(roadPartCollider.bounds.center, roadPartCollider.bounds.extents, roadPart.transform.rotation);

        foreach (Collider collider in colliders)
        {
            // There is collision only if it's not own road part collider and the object is a road part
            if (collider != roadPartCollider && collider.gameObject.tag == "RoadPart")
            {
                isColliding = true;
                Debug.Log($"{roadPart.name} collides with {collider.gameObject.name}");
            }
        }

        return isColliding;
    }
}