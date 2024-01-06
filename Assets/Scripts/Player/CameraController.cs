using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class CameraController : MonoBehaviour
{
    [Tooltip("The target to follow")]
    public GameObject target;

    [Tooltip("Offset from the target")]
    public Vector3 offset;

    private GameObject spaceship;

    private GameObject cockpit;

    void Awake()
    {
        spaceship = this.target.transform.Find("Spaceship").gameObject;
        cockpit = this.target.transform.Find("Cockpit").gameObject;
    }

    void Start()
    {
        this.cockpit.SetActive(false);
        this.spaceship.SetActive(true);

        // Set the camera in POV if the VR headset is active
        if (XRSettings.isDeviceActive)
        {
            // offset = new Vector3(0, 0.16f, -0.17f);
            this.cockpit.SetActive(true);
            this.spaceship.SetActive(false);
        }
    }

    void LateUpdate()
    {
        // Follow the target with an offset
        if (target != null)
        {
            // Calculate the desired position
            Vector3 rotatedOffset = target.transform.rotation * offset;
            this.transform.position = target.transform.position + rotatedOffset;

            // Make the camera look in the same direction as the target
            this.transform.rotation = target.transform.rotation;
        }
    }
}