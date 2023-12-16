using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using PathCreation;

/// <summary>
/// The GameManager class is the entry point of the game.
/// </summary>
public class GameManager : MonoBehaviour
{
    private UdpClientController playerUdpClient;
    public GameObject player;
    public PathCreator pathCreator;

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
        pathCreator = GameObject.Find("Road").GetComponent<PathCreator>();
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

            // Doc: https://docs.google.com/document/d/1-FInNfD2GC-fVXO6KyeTSp9OSKst5AzLxDaBRb69b-Y/edit

            // Place the player parallel to the Road game object according to the path
            Vector3 playerPosition = player.transform.position;
            Vector3 pathPosition = pathCreator.path.GetClosestPointOnPath(playerPosition);
            Vector3 pathDirection = pathCreator.path.GetDirectionAtDistance(pathCreator.path.GetClosestDistanceAlongPath(playerPosition));
            Vector3 pathRotation = pathCreator.path.GetRotationAtDistance(pathCreator.path.GetClosestDistanceAlongPath(playerPosition)).eulerAngles;

            // Give a constant movement toward the facing direction
            Vector3 automaticMovement = player.transform.forward;
            player.transform.position += automaticMovement * Time.deltaTime * speed;

            // Rotate the player with the mouse (horizontal axis only)
            float mouseX = Input.GetAxis("Mouse X");
            Vector3 rotation = player.transform.rotation.eulerAngles;
            player.transform.rotation = Quaternion.Euler(pathRotation.x, rotation.y + mouseX * rotationSensibility, rotation.z);

            // Shift the player with the keyboard, according to the camera facing direction
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            Vector3 movement = player.transform.right * horizontal + player.transform.forward * vertical;
            player.transform.position += movement * Time.deltaTime * movementSensibility;

            // Align the player's altitude with the road
            player.transform.position = new Vector3(player.transform.position.x, pathPosition.y + 0.1f, player.transform.position.z);

        }
    }
}
