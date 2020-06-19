using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Reticule : MonoBehaviour
{
    public Sprite openSprite;
    public Sprite closeSprite;
    public Sprite grabSprite;

    private Pointer pointer;
    private SpriteRenderer reticuleRenderer;
    private Camera lookAtCamera;

    private void Awake()
    {
        pointer = Pointer.instance;
        lookAtCamera = Camera.main;
        reticuleRenderer = GetComponent<SpriteRenderer>();

        pointer.onPointerUpdate += UpdateSprite;
        VRInput.onTouchpadDown += ProcessTouchPadDown;
        VRInput.onTouchpadUp += ProcessTouchPadUp;

        reticuleRenderer.color = pointer.standardColor;
    }

    private void Update()
    {
        transform.LookAt(lookAtCamera.gameObject.transform);
    }

    private void OnDestroy()
    {
        pointer.onPointerUpdate -= UpdateSprite;
        VRInput.onTouchpadDown -= ProcessTouchPadDown;
        VRInput.onTouchpadUp -= ProcessTouchPadUp;
    }

    private void UpdateSprite(Vector3 point, GameObject hitObject)
    {
        transform.position = point;
        if (hitObject)
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
        reticuleRenderer.color = pointer.pressedColor;
    }

    private void ProcessTouchPadUp()
    {
        reticuleRenderer.color = pointer.standardColor;
    }
}
