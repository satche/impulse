using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class CameraController : MonoBehaviour
{
    [Tooltip("The target to follow")]
    public GameObject target;

    [Tooltip("The camera in first person, typically used to see the spaceship from the outside")]
    public GameObject firstPersonCamera;

    [Tooltip("The camera in third person, typically used for VR in the cockpit")]
    public GameObject thirdPersonCamera;

    private GameObject spaceship;

    private GameObject cockpit;

    void Awake()
    {
        spaceship = this.target.transform.Find("Spaceship").gameObject;
        cockpit = this.target.transform.Find("Cockpit").gameObject;
        ChosePOV();
    }

    private void ChosePOV()
    {
        if (XRSettings.isDeviceActive)
        {
            firstPersonCamera.SetActive(true);
            cockpit.SetActive(false);

            thirdPersonCamera.SetActive(false);
            spaceship.SetActive(false);
        }
        else
        {
            firstPersonCamera.SetActive(false);
            cockpit.SetActive(false);

            thirdPersonCamera.SetActive(true);
            spaceship.SetActive(true);
        }
    }
}