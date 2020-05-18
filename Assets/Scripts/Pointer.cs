using UnityEngine;
using UnityEngine.Events;

public class Pointer : MonoBehaviour
{
    public float m_Distance = 10.0f;
    public LineRenderer m_LineRenderer = null;
    public LayerMask m_EverythingMask = 0;
    public LayerMask m_InteractableMask = 0;
    public Color m_PointerStandardColor;
    public Color m_PointerPressedColor;
    public UnityAction<Vector3, GameObject> OnPointerUpdate = null;

    private Transform m_CurrentOrigin = null;
    private GameObject m_CurrentObject = null;

    private void Awake()
    {
        PlayerEvents.onControllerSource += UpdateOrigin;
        PlayerEvents.onTouchpadDown += ProcessTouchPadDown;
        PlayerEvents.onTouchpadUp += ProcessTouchPadUp;
    }

    private void Start()
    {
        setLineColor(m_PointerStandardColor);
    }

    private void OnDestroy()
    {
        PlayerEvents.onControllerSource -= UpdateOrigin;
        PlayerEvents.onTouchpadDown -= ProcessTouchPadDown;
        PlayerEvents.onTouchpadUp -= ProcessTouchPadUp;
    }

    private void Update()
    {
        Vector3 hitPoint = UpdateLine();

        m_CurrentObject = UpdatePointerStatus();

        OnPointerUpdate?.Invoke(hitPoint, m_CurrentObject);
    }

    private Vector3 UpdateLine()
    {
        RaycastHit hit = CreateRaycast(m_EverythingMask);

        Vector3 endPosition = m_CurrentOrigin.position + (m_CurrentOrigin.forward * m_Distance);
        if (hit.collider != null)
            endPosition = hit.point;

        m_LineRenderer.SetPosition(0, m_CurrentOrigin.position);
        m_LineRenderer.SetPosition(1, m_CurrentOrigin.position + (m_CurrentOrigin.forward * 0.5f));

        return endPosition;
    }

    private void UpdateOrigin(OVRInput.Controller controller, GameObject controllerObject)
    {
        m_CurrentOrigin = controllerObject.transform;
    }

    private GameObject UpdatePointerStatus()
    {
        RaycastHit hit = CreateRaycast(m_InteractableMask);

        if (hit.collider)
            return hit.collider.gameObject;

        return null;
    }

    private RaycastHit CreateRaycast(int layer)
    {
        RaycastHit hit;
        Ray ray = new Ray(m_CurrentOrigin.position, m_CurrentOrigin.forward);
        Physics.Raycast(ray, out hit, m_Distance, layer);

        return hit;
    }

    private void setLineColor(Color newColor)
    {
        if (!m_LineRenderer)
        {
            Debug.LogWarning("No LineRenderer selected for pointer!");
            return;
        }

        m_LineRenderer.startColor = newColor;
    }

    private void ProcessTouchPadDown()
    {
        setLineColor(m_PointerPressedColor);

        if (!m_CurrentObject)
            return;

        Interactable interactable = m_CurrentObject.GetComponent<Interactable>();
        interactable.Pressed();
    }

    private void ProcessTouchPadUp()
    {
        setLineColor(m_PointerStandardColor);
    }
}
