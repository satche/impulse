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

    [Tooltip("Last GameObject to use to end the road")]
    public GameObject roadPartEnd;

    // List of the road parts instances used to display the road
    private List<GameObject> roadPartsInstances = new List<GameObject>();

    void Start()
    {
        // Generate the road
        for (int i = 0; i < roadLength; i++)
        {
            Vector3 roadPartPosition;

            // Take a random road part from the list
            int random = Random.Range(0, roadParts.Count);
            random = 0; // test with only straight road
            GameObject roadPart = roadParts[random];

            // Is there a road part instance already?
            if (gameObject.transform.childCount > 0)
            {
                // If yes, display the next road part after the last one
                Transform lastRoadPart = gameObject.transform.GetChild(gameObject.transform.childCount - 1);
                roadPartPosition = lastRoadPart.position + new Vector3(0, 0, 8);
            }
            else
            {
                // If not, display the first road part at the origin
                roadPartPosition = gameObject.transform.position;
            }

            // Instantiate the road part as a child of the road
            GameObject roadPartInstance = Instantiate(roadPart, roadPartPosition, Quaternion.identity);
            roadPartInstance.transform.SetParent(gameObject.transform);
        }

    }

}
