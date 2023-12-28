using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TerrainAsset;
public class TerrainGenerator : MonoBehaviour
{

    [Tooltip("The assets that will be used to generate the terrain.")]
    public List<TerrainAsset> assets;

    void Start()
    {
        foreach (TerrainAsset asset in assets)
        {
            for (int i = 0; i < asset.iterations; i++)
            {
                int x = Random.Range(asset.minPositionVariation, asset.maxPositionVariation);
                int y = Random.Range(asset.minAltitudeVariation, asset.maxAltitudeVariation);
                int z = Random.Range(asset.minPositionVariation, asset.maxPositionVariation);
                int rotation = Random.Range(0, 360);
                float size = Random.Range(asset.minScaleVariation, asset.maxScaleVariation);

                Vector3 position = new Vector3(x, y, z);
                asset.gameObject.transform.rotation = Quaternion.Euler(0, rotation, 0);
                asset.gameObject.transform.localScale = new Vector3(size, size, size);

                // Ignore if it collides with another object
                bool isColliding = Physics.CheckBox(position, asset.gameObject.GetComponent<Collider>().bounds.size);
                if (isColliding) { continue; }

                Instantiate(asset.gameObject, position, Quaternion.identity);
            }
        }
    }
}
