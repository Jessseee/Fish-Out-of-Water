using UnityEngine;

public class Bobbing : MonoBehaviour
{
    float posY;
    float rotY;
    float originalPosY;
    float originalRotY;
    float rndOffset;

    public float floatStrength = 0.1f;
    public float rotateStrength = 0.05f;

    void Start()
    {
        rndOffset = Random.Range(0.5f, 1.5f);
        originalPosY = transform.position.y;
        originalRotY = transform.rotation.y;
    }

    void Update()
    {
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;
        posY = originalPosY + (Mathf.Sin(Time.time * rndOffset) * floatStrength);
        rotY = originalRotY + (Mathf.Sin(Time.time * rndOffset) * rotateStrength);
        transform.position = new Vector3(pos.x, posY, pos.z);
        transform.rotation = new Quaternion(rot.x, rotY, rot.z, rot.w);
    }
}
