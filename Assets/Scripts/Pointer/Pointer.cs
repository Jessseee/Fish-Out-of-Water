using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(LineRenderer))]
public class Pointer : MonoBehaviour
{
    #region Singleton
    public static Pointer _instance;
    public static Pointer instance { get { return _instance; } }
    #endregion

    public float distance = 10.0f;
    public LayerMask everythingMask = 0;
    public LayerMask interactableMask = 0;
    public Color standardColor = Color.white;
    public Color pressedColor = Color.blue;
    public UnityAction<Vector3, GameObject> onPointerUpdate;
    [NonSerialized] public Vector3 endPosition;

    private LineRenderer lineRenderer;
    private Transform currentOrigin;
    private GameObject currentObject;
    private Interactable currentInteractable;

    private void Awake()
    {
        #region Singleton
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("There was another pointer present in the scene");
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        #endregion

        lineRenderer = GetComponent<LineRenderer>();

        VRInput.onControllerSource += UpdateOrigin;
        VRInput.onTouchpadDown += ProcessTouchPadDown;
        VRInput.onTouchpadUp += ProcessTouchPadUp;
        VRInput.onTriggerDown += ProcessTriggerDown;
        VRInput.onTriggerUp += ProcessTriggerUp;
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
    }

    private void LateUpdate()
    {
        Vector3 hitPoint = UpdateLine();

        currentObject = UpdatePointerStatus();

        onPointerUpdate?.Invoke(hitPoint, currentObject);
    }

    private Vector3 UpdateLine()
    {
        RaycastHit hit = CreateRaycast(everythingMask);

        endPosition = currentOrigin.position + (currentOrigin.forward * distance);
        if (hit.collider != null)
            endPosition = hit.point;

        lineRenderer.SetPosition(0, currentOrigin.position);
        lineRenderer.SetPosition(1, currentOrigin.position + (currentOrigin.forward * 0.5f));

        return endPosition;
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
        Physics.Raycast(ray, out hit, distance, layer);

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

        if (!currentObject)
            return;

        currentInteractable = currentObject.GetComponent<Interactable>();
        currentInteractable.TouchpadDown();
    }

    private void ProcessTouchPadUp()
    {
        SetLineColor(standardColor);

        if (!currentObject)
            return;

        currentInteractable.TouchpadUp();
    }

    private void ProcessTriggerDown()
    {
        if (!currentObject)
            return;

        currentInteractable = currentObject.GetComponent<Interactable>();
        currentInteractable.TriggerDown();
    }

    private void ProcessTriggerUp()
    {
        if (!currentObject)
            return;

        currentInteractable.TriggerUp();
    }

    public Rigidbody GetCurrentAnchorRb()
    {
        return currentOrigin.GetComponent<Rigidbody>();
    }
}
