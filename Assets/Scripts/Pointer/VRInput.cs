using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VRInput : MonoBehaviour
{
    #region Events
    public static UnityAction onTouchpadUp;
    public static UnityAction onTouchpadDown;
    public static UnityAction onTriggerUp;
    public static UnityAction onTriggerDown;
    public static UnityAction<OVRInput.Controller, GameObject> onControllerSource;
    #endregion

    #region Anchors
    private GameObject leftAnchor;
    private GameObject rightAnchor;
    #endregion

    #region Input
    private Dictionary<OVRInput.Controller, GameObject> controllerSets;
    private OVRInput.Controller inputSource = OVRInput.Controller.None;
    private OVRInput.Controller controller = OVRInput.Controller.None;
    private bool inputActive = true;
    #endregion

    private void Awake()
    {
        leftAnchor = GameObject.FindGameObjectWithTag("LeftAnchor");
        rightAnchor = GameObject.FindGameObjectWithTag("RightAnchor");

        OVRManager.HMDMounted += PlayerFound;
        OVRManager.HMDUnmounted += PlayerLost;

        controllerSets = CreateControllerSets();
        CheckForController();
    }

    private void OnDestroy()
    {
        OVRManager.HMDMounted -= PlayerFound;
        OVRManager.HMDUnmounted -= PlayerLost;
    }

    private void Update()
    {
        if (!inputActive)
            return;

        CheckForController();
        CheckInputSource();
        Input();
    }

    private void CheckForController()
    {
        OVRInput.Controller controllerCheck = controller;

        if (OVRInput.IsControllerConnected(OVRInput.Controller.RTrackedRemote))
            controllerCheck = OVRInput.Controller.RTrackedRemote;

        if (OVRInput.IsControllerConnected(OVRInput.Controller.LTrackedRemote))
            controllerCheck = OVRInput.Controller.LTrackedRemote;

        controller = UpdateSource(controllerCheck, controller);
    }

    private void CheckInputSource()
    {
        inputSource = UpdateSource(OVRInput.GetActiveController(), inputSource);
    }

    private void Input()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad))
        {
            onTouchpadDown?.Invoke();
        }

        if (OVRInput.GetUp(OVRInput.Button.PrimaryTouchpad))
        {
            onTouchpadUp?.Invoke();
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            onTriggerDown?.Invoke();
        }

        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger))
        {
            onTriggerUp?.Invoke();
        }

        if (OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad) != Vector2.zero)
        {

        }
    }

    private OVRInput.Controller UpdateSource(OVRInput.Controller check, OVRInput.Controller previous)
    {
        if (check == previous)
            return previous;

        GameObject controllerObject;
        controllerSets.TryGetValue(check, out controllerObject);

        onControllerSource?.Invoke(check, controllerObject);

        return check;
    }

    private void PlayerFound()
    {
        inputActive = true;
    }

    private void PlayerLost()
    {
        inputActive = false;
    }

    private Dictionary<OVRInput.Controller, GameObject> CreateControllerSets()
    {
        return new Dictionary<OVRInput.Controller, GameObject>()
        {
            { OVRInput.Controller.LTrackedRemote, leftAnchor },
            { OVRInput.Controller.RTrackedRemote, rightAnchor }
        };
    }
}
