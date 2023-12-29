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

    // Blacklist of road parts that are not allowed to be used at a given step
    private Dictionary<int, List<RoadPart>> roadPartBlacklist = new Dictionary<int, List<RoadPart>>();

    // Keep track of the generation step
    private int step;


    void Start()
    {
        this.roadPartStart.iterations = 1;
        this.roadPartEnd.iterations = 1;

        this.roadPartPendingList = CreatePendingList(this.roadPartBlueprintList);
        this.roadPartPendingList = ShuffleList(this.roadPartPendingList);

        // Add the first and last road part to the pending list
        this.roadPartPendingList.Insert(0, roadPartStart);
        this.roadPartPendingList.Add(roadPartEnd);

        // Clean eventuals remaining children in the road parent
        if (gameObject.transform.childCount > 0)
        {
            foreach (Transform child in gameObject.transform)
            {
                DestroyImmediate(child.gameObject);
            }
        }

        GenerateRoad(roadPartPendingList);
    }

    /// <summary>
    /// Create a list of road parts to use to generate the road, according to the defined road part iteration
    /// </summary>
    /// <param name="roadPartBlueprintList">The RoadPart blueprint list to use to create the pending list</params>
    /// <returns>The pending GameObject list</returns>
    private List<RoadPart> CreatePendingList(List<RoadPart> roadPartBlueprintList)
    {
        List<RoadPart> pendingList = new List<RoadPart>();

        // Iterate in the blueprint list
        foreach (RoadPart roadPart in roadPartBlueprintList)
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
    /// <param name="list">The list of road parts to shuffle</params>
    /// <returns>The shuffled list</returns>
    private List<RoadPart> ShuffleList(List<RoadPart> list)
    {
        List<RoadPart> shuffledList = list.OrderBy(x => Random.value).ToList();
        return shuffledList;
    }

    /// <summary>
    /// Generates a road from the roadParts list.<br/>
    /// Instantiate them in a sequence, connecting the end of the previous roadPart to the start of the next one.
    /// </summary>
    /// <param name="roadPartPendingList">The list of road parts to use to generate the road</params>
    private void GenerateRoad(List<RoadPart> roadPartPendingList)
    {
        int maxSteps = roadPartPendingList.Count;

        // Generate the road
        for (this.step = 0; this.step < maxSteps; this.step++)
        {
            int spawnedRoadPartCount = this.gameObject.transform.childCount;

            Debug.Log($"Step {this.step}");

            GameObject nextRoadPart = SpawnRoadPart(roadPartPendingList[this.step]);
            if (spawnedRoadPartCount == 0) { continue; }

            GameObject previousRoadPart = this.gameObject.transform.GetChild(spawnedRoadPartCount - 1).gameObject;
            ConnectRoadParts(previousRoadPart, nextRoadPart);

            if (IsColliding(nextRoadPart))
            {
                FixRoad(previousRoadPart, nextRoadPart);
            }

        }
    }

    /// <summary>
    /// Instanciate and place the next road part
    /// </summary>
    /// <param name="roadPart">The road part to instantiate</param>
    /// <returns>The spawned road part GameObject</returns>
    private GameObject SpawnRoadPart(RoadPart roadPart)
    {
        GameObject spawnedRoadPart = Instantiate(roadPart.gameObject, gameObject.transform.position, Quaternion.identity);

        // Put it at the road origin
        spawnedRoadPart.transform.SetParent(gameObject.transform);
        spawnedRoadPart.name = $"{roadPart.gameObject.name} {this.step}";

        return spawnedRoadPart;
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
    private bool IsColliding(GameObject roadPart)
    {
        bool isColliding = false;
        Physics.SyncTransforms();

        // List all colliders that overlap this roadpart's collider
        BoxCollider roadPartCollider = roadPart.GetComponent<BoxCollider>();
        Collider[] colliders = Physics.OverlapBox(roadPartCollider.bounds.center, roadPartCollider.bounds.extents);

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

    /// <summary>
    /// Fix the road by replacing the bad road part with another one from the pending list.<br/>
    /// </summary>
    /// <remarks>
    /// When we take a step back to fix the road, a road part is destroyed but never replaced.<br/>
    /// This result of having shorter road than expected at the end of the generation.
    /// </remarks>
    /// <param name="previousRoadPart">The previous road part in the scene</param>
    /// <param name="badRoadPart">The bad road part that collide with another one</param>
    private void FixRoad(GameObject previousRoadPart, GameObject badRoadPart)
    {
        Debug.Log($"Fixing road at step {step}");

        // Initialize the blacklist for this step if it doesn't exist
        if (!this.roadPartBlacklist.ContainsKey(step))
        {
            this.roadPartBlacklist[this.step] = new List<RoadPart>();
        }

        // Add the bad road part to the blacklist for this step
        this.roadPartBlacklist[this.step].Add(this.roadPartBlueprintList.Find(x => $"{x.gameObject.name} {this.step}" == badRoadPart.name));

        // Create list of available road parts (that are not in the blacklist) for this step
        List<RoadPart> availableRoadParts = this.roadPartBlueprintList.Except(roadPartBlacklist[step]).ToList();

        // Try all the available road parts until one doesn't collide
        for (int i = availableRoadParts.Count; i > 0; i--)
        {
            ReplaceRoadPartInList(badRoadPart, availableRoadParts);
            GameObject newRoadPart = SpawnRoadPart(this.roadPartPendingList[this.step]);
            ConnectRoadParts(previousRoadPart, newRoadPart);

            if (!IsColliding(newRoadPart)) { break; }
            badRoadPart = newRoadPart;

            // If every road part bluepint are still colliding, take one step back and try again
            if (i == 1 && IsColliding(badRoadPart))
            {
                Debug.Log("Can't fix road, take one step back");

                DestroyImmediate(badRoadPart);
                GameObject oldPreviousRoadPart = gameObject.transform.GetChild(gameObject.transform.childCount - 2).gameObject;
                GameObject oldNextRoadPart = gameObject.transform.GetChild(gameObject.transform.childCount - 1).gameObject;

                FixRoad(oldPreviousRoadPart, oldNextRoadPart);

                // Clean the black list for this step
                this.roadPartBlacklist[this.step].Clear();
            }
        }
    }

    /// <summary>
    /// Replace the bad road part with another one in the pending list
    /// </summary>
    /// <param name="badRoadPart">The roadPart to replace</param>
    /// <param name="availableRoadPartsList">The list to go look for another potential roadPart</param>
    private void ReplaceRoadPartInList(GameObject badRoadPart, List<RoadPart> availableRoadPartsList)
    {
        // Remove bad road part form everywhere
        this.roadPartPendingList.RemoveAt(this.step);
        availableRoadPartsList.Remove(availableRoadPartsList.Find(x => $"{x.gameObject.name} {this.step}" == badRoadPart.name));
        DestroyImmediate(badRoadPart);

        // Check if the list is empty
        if (availableRoadPartsList.Count == 0) { return; }

        // Replace with another one from the available list
        int randomIndex = Random.Range(0, availableRoadPartsList.Count);
        this.roadPartPendingList.Insert(this.step, availableRoadPartsList[randomIndex]);
    }
}