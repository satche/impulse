using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// The GameManager class is the entry point of the game.
/// </summary>
public class GameManager : MonoBehaviour
{
    private UdpClientController playerUdpClient;
    public GameObject player;

    [Tooltip("The base automatic speed of the player.")]
    public float speed = 1f;

    [Tooltip("How sensitive the player rotation is.")]
    public float rotationSensibility = 1f;

    [Tooltip("How sensitive the player movement is.")]
    public float movementSensibility = 1f;


    void Start()
    {
        playerUdpClient = new UdpClientController(5000);
        player = GameObject.Find("Player");
    }

    void Update()
    {
        // Update the player position according to the data received from the server
        if (playerUdpClient.IsConnected())
        {
            PositionManager pm = player.GetComponent<PositionManager>();
            pm.updatePosition(playerUdpClient?.data);
        }
        else
        {
            // Give a constant movement toward the facing direction
            // TODO: this should be in the PositionManager class
            Vector3 automaticMovement = player.transform.forward;
            player.transform.position += automaticMovement * Time.deltaTime * speed;

            // Rotate the player with the mouse (horizontal axis only)
            float mouseX = Input.GetAxis("Mouse X");
            Vector3 rotation = player.transform.rotation.eulerAngles;
            player.transform.rotation = Quaternion.Euler(rotation.x, rotation.y + mouseX * rotationSensibility, rotation.z);

            // Shift the player with the keyboard, according to the camera facing direction
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            Vector3 movement = player.transform.right * horizontal + player.transform.forward * vertical;
            player.transform.position += movement * Time.deltaTime * movementSensibility;
        }
    }

    /// <summary>
    /// Generate the terrain.
    /// </summary>
    private void TerrainGenerator()
    {

    }
}