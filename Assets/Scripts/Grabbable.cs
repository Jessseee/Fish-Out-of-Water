using UnityEngine;

public class Grabbable : Interactable
{
    public bool grabbed;

    private FixedJoint joint;


    public void ToggleGrab()
    {
        if (!grabbed)
            TryGrab();
        else
            Release();
    }

    public void TryGrab()
    {
        if (!grabbed)
        {
            grabbed = true;
            joint = gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = Pointer.instance.GetCurrentAnchorRb();
        }
    }

    public void Release()
    {
        grabbed = false;
        Destroy(joint);
        transform.SetParent(null);
    }
}
