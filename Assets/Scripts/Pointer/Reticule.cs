using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Reticule : MonoBehaviour
{
    public Sprite openSprite;
    public Sprite closeSprite;
    public Sprite grabSprite;
    public Sprite teleportSprite;

    private bool isTeleportReticule;
    private SpriteRenderer reticuleRenderer;
    private Camera lookAtCamera;

    private void Awake()
    {
        lookAtCamera = Camera.main;
        reticuleRenderer = GetComponent<SpriteRenderer>();

        Pointer.instance.onPointerUpdate += UpdateSprite;
        VRInput.onTouchpadDown += ProcessTouchPadDown;
        VRInput.onTouchpadUp += ProcessTouchPadUp;
        VRInput.onTriggerDown += ProcessTriggerDown;
        VRInput.onTriggerUp += ProcessTriggerUp;

        reticuleRenderer.color = Pointer.instance.standardColor;
    }

    private void Update()
    {
        if (!isTeleportReticule)
            transform.LookAt(lookAtCamera.gameObject.transform);
    }

    private void OnDestroy()
    {
        Pointer.instance.onPointerUpdate -= UpdateSprite;
        VRInput.onTouchpadDown -= ProcessTouchPadDown;
        VRInput.onTouchpadUp -= ProcessTouchPadUp;
        VRInput.onTriggerDown -= ProcessTriggerDown;
        VRInput.onTriggerUp -= ProcessTriggerUp;
    }

    private void UpdateSprite(Vector3 point, GameObject hitObject)
    {
        transform.position = point;

        // Set the type of reticule depending on the object pointed at or trigger pressed
        if (isTeleportReticule)
        {
            reticuleRenderer.sprite = teleportSprite;
            transform.eulerAngles = new Vector3(90, 0, 0);
        } else if (hitObject)
            {
                if (hitObject.GetComponent<Interactable>().grabbable)
                    reticuleRenderer.sprite = grabSprite;
                else
                    reticuleRenderer.sprite = closeSprite;
        } else
        {
            reticuleRenderer.sprite = openSprite;
        }
    }   

    private void ProcessTouchPadDown()
    {
        reticuleRenderer.color = Pointer.instance.pressedColor;
    }

    private void ProcessTouchPadUp()
    {
        reticuleRenderer.color = Pointer.instance.standardColor;
    }

    private void ProcessTriggerDown()
    {
        isTeleportReticule = true;
    }

    private void ProcessTriggerUp()
    {
        isTeleportReticule = false;
    }
}
