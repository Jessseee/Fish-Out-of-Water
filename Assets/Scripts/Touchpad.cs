using UnityEngine;

public class Touchpad : MonoBehaviour
{
    public Animator m_Animator = null;

    private void Awake()
    {
        PlayerEvents.onTouchpadDown += ProcessTouchPadDown;
        PlayerEvents.onTouchpadUp += ProcessTouchPadUp;
    }

    private void OnDestroy()
    {
        PlayerEvents.onTouchpadDown -= ProcessTouchPadDown;
        PlayerEvents.onTouchpadUp -= ProcessTouchPadUp;
    }

    private void ProcessTouchPadDown()
    {
        if (m_Animator != null)
            m_Animator.SetBool("Pressed", true);
    }

    private void ProcessTouchPadUp()
    {
        if (m_Animator != null)
            m_Animator.SetBool("Pressed", false);
    }
}
