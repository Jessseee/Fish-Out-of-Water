﻿using System;
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
    public static UnityAction onTriggerDoublePressed;
    public static UnityAction<OVRInput.Controller, GameObject> onControllerSource;
    public static UnityAction<Vector2> onTouchpadTouch;
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

    public GameObject tutorialText;
    private bool firstInput = false;
    private float triggerCoolDown;
    private float triggerCount;

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
        if (!firstInput)
        {
            if (OVRInput.GetDown(OVRInput.Button.Any))
            {
                tutorialText.SetActive(false);
                firstInput = true;
            }
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad))
            onTouchpadDown?.Invoke();

        if (OVRInput.GetUp(OVRInput.Button.PrimaryTouchpad))
            onTouchpadUp?.Invoke();

        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            if (triggerCount == 0)
                onTriggerDown?.Invoke();

            if (triggerCoolDown > 0 && triggerCount == 1)
                onTriggerDoublePressed?.Invoke();
            else
            {
                triggerCoolDown = 0.25f;
                triggerCount += 1;
            }
        }

        if (triggerCoolDown > 0)
            triggerCoolDown -= 1* Time.deltaTime;
        else
        {
            triggerCount = 0;
        }


        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger))
            onTriggerUp?.Invoke();

        Vector2 touchpadPosition = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
        if (touchpadPosition != Vector2.zero)
            onTouchpadTouch?.Invoke(touchpadPosition);
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
