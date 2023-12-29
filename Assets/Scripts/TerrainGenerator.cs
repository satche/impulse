using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TerrainAsset;

public class TerrainGenerator : MonoBehaviour
{

    [Tooltip("The game object containing the road")]
    public GameObject road;

    [Tooltip("The game object using as the groud")]
    public GameObject ground;

    [Tooltip("The assets that will be used to generate the terrain.")]
    public List<TerrainAsset> assets;

    private Vector3 roadCenter;

    void Start()
    {
        defineRoadCenter();
        spawnGround();
        spawnAssets();
    }

    private void defineRoadCenter()
    {
        Bounds bounds = new Bounds(this.road.transform.position, Vector3.zero);
        foreach (Renderer renderer in this.road.GetComponentsInChildren<Renderer>())
        {
            bounds.Encapsulate(renderer.bounds);
        }
        this.roadCenter = bounds.center;
    }

    /// <summary>
    /// Set the ground position
    /// </summary>
    private void spawnGround()
    {
        // Take the gameobject in "Road" gameobject with the lowest altitude
        float groundLevel = float.MaxValue;
        foreach (Transform child in this.road.transform)
        {
            if (child.position.y < groundLevel)
            {
                groundLevel = child.position.y;
            }
        }

        // Set the ground position
        Vector3 groundPosition = this.roadCenter;
        groundPosition.y = groundLevel - 3;
        this.ground.transform.position = groundPosition;
        Instantiate(this.ground, groundPosition, Quaternion.identity);
    }

    /// <summary>
    /// Spawn the assets
    /// </summary>
    private void spawnAssets()
    {
        // Spawn flying assets
        foreach (TerrainAsset asset in this.assets)
        {
            for (int i = 0; i < asset.iterations; i++)
            {
                int x = Random.Range(asset.minPositionVariation, asset.maxPositionVariation);
                int y = Random.Range(asset.minAltitudeVariation, asset.maxAltitudeVariation);
                int z = Random.Range(asset.minPositionVariation, asset.maxPositionVariation);
                int rotation = Random.Range(0, 360);
                float size = Random.Range(asset.minScaleVariation, asset.maxScaleVariation);

                Vector3 offset = new Vector3(x, y, z);
                Vector3 position = this.roadCenter + offset;
                Quaternion rotationQuaternion = Quaternion.Euler(0, rotation, 0);
                Vector3 scale = new Vector3(size, size, size);

                GameObject gameObject = Instantiate(asset.gameObject, position, rotationQuaternion);
                gameObject.transform.localScale = scale;
                gameObject.transform.parent = this.transform;

                // List all colliders that overlap this roadpart's collider
                Physics.SyncTransforms();
                BoxCollider assetCollider = gameObject.GetComponent<BoxCollider>();
                Collider[] colliders = Physics.OverlapBox(assetCollider.bounds.center, assetCollider.bounds.extents);

                foreach (Collider collider in colliders)
                {
                    // There is collision only if it's not own road part collider and the object is a road part
                    if (collider != assetCollider)
                    {
                        DestroyImmediate(gameObject);
                    }
                }

            }
        }
    }
}