using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{

    public GameSettings gameSettings;
    private UdpClientController playerUdpClient;

    public GameObject player;
    private PositionManager positionManager;


    void Start()
    {
        gameSettings = Resources.Load<GameSettings>("GameSettings");
        playerUdpClient = new UdpClientController(5000);
        positionManager = new PositionManager(
            axisMin: -100,
            axisMax: 100,
            angleMin: -90,
            angleMax: 90
        );
        player = GameObject.Find("Player");
    }

    void Update()
    {
        if (playerUdpClient.IsConnected()) { updatePosition(); }
    }

    private void updatePosition()
    {
        // Send new position to player
        string data = playerUdpClient?.data;
        positionManager.StoreSpatialValues(data);

        // Update sensibility according to the game settings
        if (gameSettings?.movementSensibility != positionManager.sensibility)
        {
            positionManager.UpdateMovementSensibility(gameSettings.movementSensibility);
        }

        // Normalize the values according to the sensibility
        float x = positionManager.NormalizeValue(positionManager.coordinates[0], positionManager.sensibility);
        float y = positionManager.NormalizeValue(positionManager.coordinates[1], positionManager.sensibility);
        float z = positionManager.NormalizeValue(positionManager.coordinates[2], positionManager.sensibility);
        float angleX = positionManager.NormalizeValue(positionManager.angles[0], positionManager.sensibility);
        float angleY = positionManager.NormalizeValue(positionManager.angles[1], positionManager.sensibility);
        float angleZ = positionManager.NormalizeValue(positionManager.angles[2], positionManager.sensibility);

        // Update player position and angle
        Vector3 newPosition = new Vector3(x, y, z);
        // Vector3 newAngle = new Vector3(angleX, angleY, angleZ);
        player.transform.position = newPosition;
        // player.transform.eulerAngles = newAngle;
    }
}
