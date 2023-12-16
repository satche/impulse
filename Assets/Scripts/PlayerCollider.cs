using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{

    // Store current material color
    private Color originalColor;

    void Start()
    {
        // Store the original color
        originalColor = GetComponent<Renderer>().material.color;
    }

    void OnTriggerEnter(Collider other)
    {
        // Change color of the Player
        GetComponent<Renderer>().material.color = Color.green;

        // Remove the projectile from the scene
        Destroy(other.gameObject);

        // Start a coroutine to wait for x second before changing the color back to original
        StartCoroutine(ChangeColorBack());
    }

    IEnumerator ChangeColorBack()
    {
        // Wait
        yield return new WaitForSeconds(0.25f);

        // Change the color back to white
        GetComponent<Renderer>().material.color = originalColor;
    }
}
