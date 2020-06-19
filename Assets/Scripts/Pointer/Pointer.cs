﻿using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(LineRenderer))]
public class Pointer : MonoBehaviour
{
    public static Pointer instance = null;

    #region Events
    public UnityAction<Vector3, GameObject> onPointerUpdate;
    #endregion

    #region Masks
    public LayerMask everythingMask = 0;
    public LayerMask interactableMask = 0;
    #endregion

    #region Line
    public float maxLineLength = 10.0f;
    public Color standardColor = Color.white;
    public Color pressedColor = Color.blue;
    [NonSerialized] public Vector3 lineEndPosition;
    private LineRenderer lineRenderer;
    #endregion

    #region Interactables
    public float moveGrabbedSpeed = 1.0f;
    private Interactable currentInteractable;
    private Rigidbody grabbedObject;
    #endregion

    #region Pointer Controller
    private Transform currentOrigin;
    private GameObject currentObject;
    #endregion


    private void Awake()
    {
        #region Singleton
        if (instance != null && instance != this)
        {
            Debug.LogWarning("There was another pointer present in the scene");
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        #endregion

        lineRenderer = GetComponent<LineRenderer>();

        VRInput.onControllerSource += UpdateOrigin;
        VRInput.onTouchpadDown += ProcessTouchPadDown;
        VRInput.onTouchpadUp += ProcessTouchPadUp;
        VRInput.onTriggerDown += ProcessTriggerDown;
        VRInput.onTriggerUp += ProcessTriggerUp;
        VRInput.onTouchpadTouch += ProcessTouchpadTouched;
    }

    private void Start()
    {
        SetLineColor(standardColor);
    }

    private void OnDestroy()
    {
        VRInput.onControllerSource -= UpdateOrigin;
        VRInput.onTouchpadDown -= ProcessTouchPadDown;
        VRInput.onTouchpadUp -= ProcessTouchPadUp;
        VRInput.onTriggerDown -= ProcessTriggerDown;
        VRInput.onTriggerUp -= ProcessTriggerUp;
        VRInput.onTouchpadTouch -= ProcessTouchpadTouched;
    }

    private void Update()
    {
        Vector3 hitPoint = UpdateLine();

        currentObject = UpdatePointerStatus();

        onPointerUpdate?.Invoke(hitPoint, currentObject);
    }

    private Vector3 UpdateLine()
    {
        RaycastHit hit = CreateRaycast(everythingMask);

        lineEndPosition = currentOrigin.position + (currentOrigin.forward * maxLineLength);
        if (hit.collider != null)
            lineEndPosition = hit.point;

        lineRenderer.SetPosition(0, currentOrigin.position);
        lineRenderer.SetPosition(1, currentOrigin.position + (currentOrigin.forward * maxLineLength/5));

        return lineEndPosition;
    }

    private void UpdateOrigin(OVRInput.Controller controller, GameObject controllerObject)
    {
        currentOrigin = controllerObject.transform;
    }

    private GameObject UpdatePointerStatus()
    {
        RaycastHit hit = CreateRaycast(interactableMask);

        if (hit.collider)
            return hit.collider.gameObject;

        return null;
    }

    private RaycastHit CreateRaycast(int layer)
    {
        RaycastHit hit;
        Ray ray = new Ray(currentOrigin.position, currentOrigin.forward);
        Physics.Raycast(ray, out hit, maxLineLength, layer);

        return hit;
    }

    private void SetLineColor(Color newColor)
    {
        lineRenderer.startColor = newColor;
        newColor.a = 0;
        lineRenderer.endColor = newColor;
    }

    private void ProcessTouchPadDown()
    {
        SetLineColor(pressedColor);

        if (grabbedObject != null)
        {
            Release();
            return;
        }

        if (!currentObject)
            return;

        currentInteractable = currentObject.GetComponent<Interactable>();

        if (currentInteractable.grabbable) {
            TryGrab(currentInteractable.GetComponent<Rigidbody>());
        }

        currentInteractable.TouchpadDown();
    }

    private void ProcessTouchPadUp()
    {
        SetLineColor(standardColor);

        if (currentObject)
            currentInteractable.TouchpadUp();
    }

    private void ProcessTriggerDown()
    {
        if (currentObject)
        {
            currentInteractable = currentObject.GetComponent<Interactable>();
            currentInteractable.TriggerDown();
        }
    }

    private void ProcessTriggerUp()
    {
        if (currentObject)
            currentInteractable.TriggerUp();
    }

    private void ProcessTouchpadTouched(Vector2 touchpadPosition)
    {
        if (grabbedObject != null)
            moveGrabbed(touchpadPosition);
    }

    private void moveGrabbed(Vector2 touchpadPosition)
    {
        float touchpadDistance = touchpadPosition.y / 10.0f;

        if ((touchpadPosition.y > 0.5 || touchpadPosition.y < -0.5))
        {
            Vector3 relativePosition = grabbedObject.transform.localPosition;
            if (relativePosition.z + touchpadDistance <= maxLineLength)
                relativePosition.z += touchpadDistance;

            grabbedObject.transform.localPosition = relativePosition;
        }
    }

    public void TryGrab(Rigidbody grabbable)
    {
        if (grabbedObject == null)
        {
            grabbedObject = grabbable;
            grabbable.isKinematic = true;
            grabbable.transform.SetParent(currentOrigin);
        }
    }

    public void Release()
    {
        grabbedObject.transform.SetParent(null);
        grabbedObject.isKinematic = false;
        grabbedObject = null;
    }
}
