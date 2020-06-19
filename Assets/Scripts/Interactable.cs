using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour
{
    [Tooltip("If grabbable is enabled requires a RigidBody component.")]
    public bool grabbable;

    #region Events
    [Header("Interaction Events")]
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
}
