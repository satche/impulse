using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncher : MonoBehaviour
{

    public GameObject missilePrefab;
    public float spawnInterval = 2f;
    public float spawnDistance = 10f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            LaunchMissile();
            timer = 0;
        }
    }

    void LaunchMissile()
    {
        Vector3 spawnPosition = Random.onUnitSphere * spawnDistance;
        spawnPosition.y = 0; // Assuming you want to spawn at ground level

        Instantiate(missilePrefab, spawnPosition, Quaternion.identity);
    }
}