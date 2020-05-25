using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class Touchpad : MonoBehaviour
{
    private Animator animator;
    private Image touchpadSprite;
    private Pointer pointer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        touchpadSprite = GetComponentInChildren<Image>();
        pointer = Pointer.instance;

        touchpadSprite.color = pointer.pressedColor;

        VRInput.onTouchpadDown += ProcessTouchPadDown;
        VRInput.onTouchpadUp += ProcessTouchPadUp;
    }

    private void OnDestroy()
    {
        VRInput.onTouchpadDown -= ProcessTouchPadDown;
        VRInput.onTouchpadUp -= ProcessTouchPadUp;
    }

    private void ProcessTouchPadDown()
    {
        if (animator != null)
            animator.SetBool("Pressed", true);
    }

    private void ProcessTouchPadUp()
    {
        if (animator != null)
            animator.SetBool("Pressed", false);
    }
}
