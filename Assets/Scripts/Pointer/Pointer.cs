using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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
    private Vector3 reticulePosition;
    private LineRenderer lineRenderer;
    private Vector3 hitPoint;
    #endregion

    #region Interactables
    public float moveGrabbedSpeed = 1.0f;
    private Interactable currentInteractable;
    private Rigidbody grabbedObject;
    #endregion

    #region VR Set
    private Transform headset;
    private Transform currentOrigin;
    private GameObject currentObject;
    #endregion

    #region Teleportation
    private Vector3 teleportPosition;
    private float teleportTime;
    private bool attemptTeleport;
    public float teleportSpeed = 1;
    public float teleportCurveHeight = 3.0f;
    public int teleportCurveDetail = 12;
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

        #region Events
        VRInput.onControllerSource += UpdateOrigin;
        VRInput.onTouchpadDown += ProcessTouchPadDown;
        VRInput.onTouchpadUp += ProcessTouchPadUp;
        VRInput.onTriggerDown += ProcessTriggerDown;
        VRInput.onTriggerUp += ProcessTriggerUp;
        VRInput.onTriggerDoublePressed += ProcessTriggerDoublePressed;
        VRInput.onTouchpadTouch += ProcessTouchpadTouched;
        #endregion

        headset = transform.parent;
        teleportPosition = headset.position;
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        SetLineColor(standardColor);
        lineRenderer.positionCount = teleportCurveDetail;
    }

    private void OnDestroy()
    {
        #region Events
        VRInput.onControllerSource -= UpdateOrigin;
        VRInput.onTouchpadDown -= ProcessTouchPadDown;
        VRInput.onTouchpadUp -= ProcessTouchPadUp;
        VRInput.onTriggerDown -= ProcessTriggerDown;
        VRInput.onTriggerUp -= ProcessTriggerUp;
        VRInput.onTriggerDoublePressed += ProcessTriggerDoublePressed;
        VRInput.onTouchpadTouch -= ProcessTouchpadTouched;
        #endregion
    }

    private void Update()
    {
        if (headset.position != teleportPosition && teleportTime < teleportSpeed)
        {
            teleportTime += Time.deltaTime;
            float lerpValue = teleportTime / teleportSpeed;
            headset.position = Vector3.Lerp(headset.position, teleportPosition, lerpValue);
        }
        else
            teleportTime = 0;
    }

    private void LateUpdate()
    {
        hitPoint = UpdateLine();
        currentObject = UpdatePointerStatus();
        onPointerUpdate?.Invoke(hitPoint, currentObject);
    }

    private Vector3 UpdateLine()
    {
        Vector3 lineStart = currentOrigin.position;
        Vector3 lineEnd = currentOrigin.position + (currentOrigin.forward * maxLineLength / 5);

        RaycastHit hitForward = CreateForwardRaycast(everythingMask);

        reticulePosition = currentOrigin.position + (currentOrigin.forward * maxLineLength);

        if (attemptTeleport && hitForward.collider == null)
        {
            RaycastHit hitFloor;
            Physics.Raycast(reticulePosition, Vector3.down, out hitFloor);
            reticulePosition = hitFloor.point;
        }
        else if (hitForward.collider != null)
            reticulePosition = hitForward.point;

        if (grabbedObject != null)
            lineEnd = reticulePosition;

        if (attemptTeleport)
            drawTeleportingLine(reticulePosition);
        else
            lineRenderer.enabled = false;

        return reticulePosition;
    }

    private void drawTeleportingLine(Vector3 reticulePoint)
    {
        Vector3 lineEnd = reticulePoint;
        Vector3 lineStart = currentOrigin.position;

        var curvePointList = new List<Vector3>();
        Vector3 curveCenter = Vector3.Lerp(lineStart, lineEnd, 0.5f);
        curveCenter.y = teleportCurveHeight;
        for (float ratio = 0; ratio <= 1; ratio += 1.0f / teleportCurveDetail)
        {
            Vector3 tangentLineVertex1 = Vector3.Lerp(lineStart, curveCenter, ratio);
            Vector3 tangentLineVertex2 = Vector3.Lerp(curveCenter, lineEnd, ratio);
            Vector3 bezierPoint = Vector3.Lerp(tangentLineVertex1, tangentLineVertex2, ratio);
            curvePointList.Add(bezierPoint);
        }

        lineRenderer.enabled = true;
        lineRenderer.SetPositions(curvePointList.ToArray());
    }

    private void UpdateOrigin(OVRInput.Controller controller, GameObject controllerObject)
    {
        currentOrigin = controllerObject.transform;
    }

    private GameObject UpdatePointerStatus()
    {
        RaycastHit hit = CreateForwardRaycast(interactableMask);

        if (hit.collider)
            return hit.collider.gameObject;

        return null;
    }

    private RaycastHit CreateForwardRaycast(int layer)
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
        if (grabbedObject == null)
            attemptTeleport = true;
    }

    private void ProcessTriggerUp()
    {
        if (grabbedObject == null)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(hitPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                teleportPosition = new Vector3(hit.position.x, headset.position.y, hit.position.z);
            }

            attemptTeleport = false;
        }
    }

    private void ProcessTriggerDoublePressed()
    {
        headset.eulerAngles += 180f * Vector3.up;
    }

    private void ProcessTouchpadTouched(Vector2 touchpadPosition)
    {
        if (grabbedObject != null)
            moveGrabbed(touchpadPosition);
    }

    private void moveGrabbed(Vector2 touchpadPosition)
    {
        float touchpadDistance = touchpadPosition.y / 10.0f;

        if (touchpadPosition.y > 0.5 || touchpadPosition.y < -0.5)
        {
            Vector3 relativePosition = grabbedObject.transform.localPosition;
            if (relativePosition.z + touchpadDistance <= maxLineLength && relativePosition.z + touchpadDistance >= 1.0f)
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
