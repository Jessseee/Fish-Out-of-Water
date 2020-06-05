using UnityEngine;
using UnityEngine.Events;


public class Interactable : MonoBehaviour
{
    #region Events
    public UnityEvent onTouchpadDown = new UnityEvent();
    public UnityEvent onTouchpadUp = new UnityEvent();
    public UnityEvent onTriggerDown = new UnityEvent();
    public UnityEvent onTriggerUp = new UnityEvent();
    #endregion

    public void TouchpadDown()
    {
        onTouchpadDown?.Invoke();
    }

    public void TouchpadUp()
    {
        onTouchpadUp?.Invoke();
    }

    public void TriggerDown()
    {
        onTriggerDown?.Invoke();
    }

    public void TriggerUp()
    {
        onTriggerUp?.Invoke();
    }
}
