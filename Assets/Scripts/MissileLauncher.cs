using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncher : MonoBehaviour
{

    public GameObject missilePrefab;


    [Tooltip("In seconds")]
    public float spawnInterval = 2f;
    [Tooltip("In meters")]
    public float axisDelta = 3f;

    private float timer;

    void Update()
    {
        // Spawn a missile every x seconds
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            LaunchMissile();
            timer = 0;
        }
    }

    void LaunchMissile()
    {

        // Define spawn position at missile launcher position
        Vector3 spawnPosition = transform.position;
        spawnPosition.x += Random.Range(-axisDelta, axisDelta);
        spawnPosition.y = 0; // Ground level spawn

        Instantiate(missilePrefab, spawnPosition, Quaternion.identity);
    }
}