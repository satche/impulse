using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EndTrigger : MonoBehaviour
{
    private GameObject player;

    private void Start()
    {
        player = GameObject.Find("Player");
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, Mathf.PingPong(Time.time, 1), transform.position.z);

        Collider thisCollider = GetComponent<Collider>();
        Collider playerCollider = player.GetComponent<Collider>();

        if (playerCollider.bounds.Intersects(thisCollider.bounds))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("EndScene");
        }
    }
}
