using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject objectPrefab;

    private bool spawning;

    private void Awake()
    {
        VRInput.onTriggerDown += ToggleSpawn;

    }

    private void Update()
    {
        if (spawning)
            SpawnObject();
    }

    private void SpawnObject()
    {
        Instantiate(objectPrefab, Pointer.instance.endPosition + new Vector3(0, 2, 0), Quaternion.identity);
    }

    private void ToggleSpawn()
    {
        spawning = !spawning;
    }
}
