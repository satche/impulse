using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Change color of the Player
        GetComponent<Renderer>().material.color = Color.red;

        // Remove the projectile from the scene
        Destroy(other.gameObject);

        // Start a coroutine to wait for x second before changing the color back to white
        StartCoroutine(ChangeColorBack());
    }

    IEnumerator ChangeColorBack()
    {
        // Wait
        yield return new WaitForSeconds(0.25f);

        // Change the color back to white
        GetComponent<Renderer>().material.color = Color.white;
    }
}
