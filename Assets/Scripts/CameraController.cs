using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class CameraController : MonoBehaviour
{
    [Tooltip("The target to follow")]
    public GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        // Set camera as POV if VR is active
        if (XRSettings.isDeviceActive)
        {
            GetComponent<Camera>().transform.position.Set(0, 0.3f, 0);
        }
    }

    void Update()
    {
        // Follow the target
        if (target != null)
        {
            this.transform.position = target.transform.position;
        }

        // Control the camera with the VR headset
        if (XRSettings.isDeviceActive)
        {
            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevicesAtXRNode(XRNode.Head, devices);
            if (devices.Count > 0)
            {
                InputDevice device = devices[0];
                Quaternion rotation;
                if (device.TryGetFeatureValue(CommonUsages.deviceRotation, out rotation))
                {
                    GetComponent<Camera>().transform.rotation = rotation;
                }
            }
        }
    }
}
