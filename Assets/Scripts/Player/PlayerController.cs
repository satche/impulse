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
        // Create a new UDP client to receive the player position from the server
        playerUdpClient = new UdpClientController(5000);
    }

    // Update is called once per frame
    void Update()
    {
        AutomaticForwardMovement(this.speed);
        DefinePlayerControls(playerUdpClient.IsConnected());
        DefineXRControls();
    }

    /// <summary>
    /// Give a constant movement toward the facing direction
    /// </summary>
    /// <param name="speed">Movement speed</param>
    private void AutomaticForwardMovement(float speed)
    {
        // Give a constant movement toward the facing direction
        Vector3 automaticMovement = this.transform.forward;
        this.transform.position += automaticMovement * Time.deltaTime * speed;
    }

    /// <summary>
    /// Define the player controls according if there is a connection to the UDP server or not
    /// </summary>
    /// <param name="isConnectedToUdpServer">True if the player is connected to the UDP server</param>
    private void DefinePlayerControls(bool isConnectedToUdpServer)
    {
        if (isConnectedToUdpServer)
        {
            PositionManager pm = this.GetComponent<PositionManager>();
            pm.updatePosition(playerUdpClient?.data);
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

    /// <summary>
    /// Define the XR controls if the VR headset is active
    /// </summary>
    private void DefineXRControls()
    {
        if (XRSettings.isDeviceActive)
        {
            // Control player with right controller
            InputDevice rightHand = GetXRNode(XRNode.RightHand);
            Vector3 handPosition;
            Quaternion handRotation;

            if (rightHand.TryGetFeatureValue(CommonUsages.devicePosition, out handPosition) && rightHand.TryGetFeatureValue(CommonUsages.deviceRotation, out handRotation))
            {
                // Create a new Quaternion with the modified y component
                Quaternion newRotation = Quaternion.Euler(handRotation.eulerAngles.x, handRotation.eulerAngles.y * rotationSensibility, handRotation.eulerAngles.z);
                this.transform.rotation = newRotation;
            }

            // Control camera with headset
            InputDevice headset = GetXRNode(XRNode.Head);
            Quaternion headsetRotation;
            if (headset.TryGetFeatureValue(CommonUsages.deviceRotation, out headsetRotation))
            {
                GetComponent<Camera>().transform.rotation = headsetRotation;
            }
        }
    }

    /// <summary>
    /// Get the XR node according to the current platform
    /// </summary>
    private InputDevice GetXRNode(XRNode node)
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(node, devices);
        InputDevice device = devices[0];

        return device;
    }
}
