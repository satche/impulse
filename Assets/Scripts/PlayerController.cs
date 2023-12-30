using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour
{

    [Tooltip("The base automatic speed of the player.")]
    [Range(0, 10f)]
    public float speed = 3f;

    [Tooltip("How sensitive the player rotation is.")]
    [Range(0, 10f)]
    public float rotationSensibility = 1f;

    [Tooltip("How sensitive the player movement is.")]
    [Range(0, 10f)]
    public float movementSensibility = 1f;

    private UdpClientController playerUdpClient;

    void Start()
    {
        playerUdpClient = new UdpClientController(5000);
    }

    // Update is called once per frame
    void Update()
    {
        // Give a constant movement toward the facing direction
        Vector3 automaticMovement = this.transform.forward;
        this.transform.position += automaticMovement * Time.deltaTime * speed;

        // Update the player position according to the data received from the server
        if (playerUdpClient.IsConnected())
        {
            PositionManager pm = this.GetComponent<PositionManager>();
            pm.updatePosition(playerUdpClient?.data);
        }
        else if (XRSettings.isDeviceActive)
        {
            // Control the player with the VR headset and controllers
            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller, devices);
            foreach (var device in devices)
            {
                Vector3 position;
                Quaternion rotation;
                if (device.TryGetFeatureValue(CommonUsages.devicePosition, out position) && device.TryGetFeatureValue(CommonUsages.deviceRotation, out rotation))
                {
                    // Create a new Quaternion with the modified y component
                    Quaternion newRotation = Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y * rotationSensibility, rotation.eulerAngles.z);
                    this.transform.rotation = newRotation;
                }
            }
        }
        else
        {
            // Rotate the player with the mouse (horizontal axis only)
            float mouseX = Input.GetAxis("Mouse X");
            Vector3 rotation = this.transform.rotation.eulerAngles;
            this.transform.rotation = Quaternion.Euler(rotation.x, rotation.y + mouseX * rotationSensibility, rotation.z);

            // Shift the player with the keyboard, according to the camera facing direction
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            Vector3 movement = this.transform.right * horizontal + this.transform.forward * vertical;
            this.transform.position += movement * Time.deltaTime * movementSensibility;
        }
    }
}
