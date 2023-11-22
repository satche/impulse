using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileMovement : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        // Move the projectile in straight line
        transform.Translate(Vector3.back * speed * Time.deltaTime);

        // When this projectile is out of screen, destroy it
        if (transform.position.z < -10)
        {
            Destroy(gameObject);
        }
    }
}