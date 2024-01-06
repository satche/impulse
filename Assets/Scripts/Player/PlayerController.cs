using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour
{

    [Tooltip("The base automatic speed of the player.")]
    [Range(1f, 3f)]
    public float speed = 1f;

    [Tooltip("How sensitive the player rotation is.")]
    [Range(0, 3f)]
    public float rotationSensibility = 1f;

    [Tooltip("How sensitive the player movement is.")]
    [Range(0, 3f)]
    public float movementSensibility = 1f;

    [Tooltip("The menus controller.")]
    public MenusController menusController;

    [Tooltip("The game manager.")]
    public GameManager gameManager;

    private UdpClientController playerUdpClient;

    void Awake()
    {
        playerUdpClient = new UdpClientController(5000);
    }

    void Start()
    {
        ApplyPlayerPrefs();
    }

    void Update()
    {
        GameStateControl();
        WatchPlayerControls();
        WatchXRControls();
        if (gameManager.isPaused) { return; }

        AutomaticForwardMovement(speed);
    }

    private void OnDestroy()
    {
        this.playerUdpClient.Close();
    }

    private void GameStateControl()
    {

        bool quitGame = Input.GetKeyDown(KeyCode.Escape);
        bool restartGame = Input.GetKeyDown(KeyCode.R);
        bool togglePauseGame = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.P);

        if (XRSettings.isDeviceActive)
        {
            InputDevice rightHand = GetXRNode(XRNode.RightHand);

            bool primaryButtonPressed = false;
            rightHand.TryGetFeatureValue(CommonUsages.primaryButton, out primaryButtonPressed);

            bool secondaryButtonPressed = false;
            rightHand.TryGetFeatureValue(CommonUsages.secondaryButton, out secondaryButtonPressed);

            bool gripButtonPressed = false;
            rightHand.TryGetFeatureValue(CommonUsages.gripButton, out gripButtonPressed);

            quitGame = gripButtonPressed;
            restartGame = secondaryButtonPressed;
            togglePauseGame = primaryButtonPressed;
        }

        // Quit game
        if (quitGame)
        {
            menusController.LoadScene("StartScene");
        }

        // Restart game
        if (restartGame) { menusController.LoadScene("MainScene"); }

        // Toggle pause Game
        if (togglePauseGame)
        {
            bool gameState = gameManager.isPaused;
            gameManager.PauseGame(!gameState);
        }
    }

    /// <summary>
    /// Apply all the player preferences
    /// </summary>
    private void ApplyPlayerPrefs()
    {
        this.speed = PlayerPrefs.GetFloat("Speed", this.speed);
        this.rotationSensibility = PlayerPrefs.GetFloat("Rotation Sensibility", this.rotationSensibility);
        this.movementSensibility = PlayerPrefs.GetFloat("Movement Sensibility", this.movementSensibility);
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
    private void WatchPlayerControls()
    {
        if (playerUdpClient.IsConnected())
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
    private void WatchXRControls()
    {
        if (XRSettings.isDeviceActive)
        {

            // Control player with right controller
            InputDevice rightHand = GetXRNode(XRNode.RightHand);
            Vector2 primary2DAxisInput;

            Camera camera = this.transform.Find("FirstPersonCamera").GetComponent<Camera>();

            if (rightHand.TryGetFeatureValue(CommonUsages.primary2DAxis, out primary2DAxisInput))
            {
                // Use the x value of the joystick or touchpad to control the rotation
                float rotationY = primary2DAxisInput.x * 5 * rotationSensibility * Time.deltaTime;

                // Only y axis is used for the rotation
                Quaternion newRotation = Quaternion.Euler(0, rotationY, 0);
                this.transform.rotation *= newRotation;
            }

            // Control camera with headset
            InputDevice headset = GetXRNode(XRNode.Head);
            Quaternion headsetRotation;
            if (headset.TryGetFeatureValue(CommonUsages.deviceRotation, out headsetRotation))
            {
                camera.transform.localRotation = headsetRotation;
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
