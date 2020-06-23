using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    Vector3 bottom = new Vector3(0, -100, 0);
    float speed = 0.5f;

    bool moving = false;
    bool removing = true;
    Vector3 target;
    Vector3 secondTarget;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(moving)
        {
            transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * speed);
            if(Vector3.Distance(transform.position, target) < 0.2)
            {
                if(secondTarget != Vector3.zero)
                {
                    target = secondTarget;
                    secondTarget = Vector3.zero;
                } else
                {
                    moving = false;
                }
            }
        } else if (!removing)
        {
            Vector3 sinePos = target + new Vector3(0, 3*Mathf.Sin(Time.time), 0);
            transform.position = sinePos;
        }

        SetColliderPosition();
    }

    /// <summary>
    /// Set the target of the floating object
    /// </summary>
    /// <param name="position">The target position</param>
    public void SetTarget(Vector3 position)
    {
        removing = false;
        if(!moving)
        {
            target = position;
            transform.position = position + bottom;
            moving = true;
            SetColliderPosition();
        } else
        {
            secondTarget = position;
        }
    }

    /// <summary>
    /// Remove the floating object
    /// </summary>
    public void Remove()
    {
        target += bottom;
        moving = true;
        removing = true;
    }

    /// <summary>
    /// Check if the object is removed from view.
    /// </summary>
    /// <returns></returns>
    public bool IsRemoved()
    {
        return removing && !moving;
    }

    /// <summary>
    /// Place the collider on the target position.
    /// </summary>
    private void SetColliderPosition()
    {
        BoxCollider collider = GetComponent<BoxCollider>();
        collider.center = transform.InverseTransformPoint(target);
    }

}
