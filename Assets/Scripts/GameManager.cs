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

    public GameSettings gameSettings;
    public GameObject player;


    void Start()
    {
        playerUdpClient = new UdpClientController(5000);
        gameSettings = Resources.Load<GameSettings>("GameSettings");
        player = GameObject.Find("Player");
    }

    void Update()
    {
        if (playerUdpClient.IsConnected())
        {
            PositionManager pm = player.GetComponent<PositionManager>();
            pm.updatePosition(playerUdpClient?.data);
        }
    }
}
