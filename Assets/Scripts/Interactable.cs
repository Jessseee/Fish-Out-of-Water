using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour
{
    public bool grabbable;

    #region Events
    [Header("Interaction Events")]
    public UnityEvent onTouchpadDown = new UnityEvent();
    public UnityEvent onTouchpadUp = new UnityEvent();
    #endregion

    private void Awake()
    {
        if (grabbable && GetComponent<Rigidbody>() == null)
            gameObject.AddComponent<Rigidbody>();
    }

    public void TouchpadDown()
    {
        onTouchpadDown?.Invoke();
    }

    public void TouchpadUp()
    {
        onTouchpadUp?.Invoke();
    }
}
