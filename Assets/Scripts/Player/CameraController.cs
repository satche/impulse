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

    void Start()
    {
        // Set the camera in POV if the VR headset is active
        if (XRSettings.isDeviceActive)
        {
            offset = new Vector3(0, 0.3f, 0);
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