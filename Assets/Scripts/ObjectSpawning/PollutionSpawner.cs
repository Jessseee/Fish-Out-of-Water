using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

using TriangleNet;
using TriangleNet.Geometry;
using TriangleNet.Tools;
using TriangleNet.Topology;

public class PollutionSpawner : MonoBehaviour
{
    // Public vairables
    public Vector3 spawnPlanePosition = new Vector3(0, 0, 0);
    public int spawnPlaneRadius = 20;
    public GameObject plasticPrefab;
    public int maxPlastics = 100;
    public Vector3 zeroLoc = new Vector3(0, -100, 0);

    // Private variables
    List<GameObject> usedPlastics;
    List<GameObject> availablePlastics;


    // Start is called before the first frame update
    void Start()
    {
        usedPlastics = new List<GameObject>();
        availablePlastics = new List<GameObject>();
        for (int i = 0; i < maxPlastics; i++)
        {
            GameObject newPlastic = Instantiate(plasticPrefab, zeroLoc, Random.rotation);
            newPlastic.SetActive(false);
            availablePlastics.Add(newPlastic);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            int randomAmount = Random.Range(0, maxPlastics);
            Debug.Log(randomAmount);
            SetPlastics(randomAmount);
        }
        foreach(GameObject plastic in usedPlastics)
        {
            if (plastic.GetComponent<FloatingObject>().IsRemoved())
            {
                usedPlastics.Remove(plastic);
                availablePlastics.Add(plastic);
            }
        }
    }

    public void SetPlastics(int plastics)
    {
        int difference = plastics - usedPlastics.Count;
        if (difference > 0)
        {
            AddPlastics(difference);
        }
        else if (difference < 0)
        {
            RemovePlastics(-difference);
        }
    }

    /// <summary>
    /// Add a specified amount of plastics
    /// </summary>
    /// <param name="amount">The amount of plastics to be added</param>
    public void AddPlastics(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            AddPlastic(availablePlastics[0]);
        }
    }

    /// <summary>
    /// Add a specific plastic object
    /// </summary>
    /// <param name="plastic">The plastic object to be added</param>
    public void AddPlastic(GameObject plastic)
    {
        // Find an available spawn location
        bool spawned = false;
        int tries = 0;
        while (!spawned && tries < Mathf.Infinity)
        {
            float angle = Random.value * 360;
            float radius = Random.value * spawnPlaneRadius;


            Vector3 spawnLocation = new Vector3(
                Mathf.Cos(angle) * radius + spawnPlanePosition.x,
                spawnPlanePosition.y,
                Mathf.Sin(angle) * radius + spawnPlanePosition.z
                );
            Vector3 size = plastic.GetComponent<MeshRenderer>().bounds.size;
            float checkRadius = Mathf.Max(size.x, Mathf.Max(size.y, size.z));
            if (Physics.OverlapSphere(spawnLocation, checkRadius).Length == 0)
            {
                availablePlastics.Remove(plastic);
                plastic.SetActive(true);
                FloatingObject floatingObject = plastic.GetComponent<FloatingObject>();
                floatingObject.SetTarget(spawnLocation);
                spawned = true;
                usedPlastics.Add(plastic);
            }
            tries++;
        }
    }

    /// <summary>
    /// Remove a specified amount of plastics
    /// </summary>
    /// <param name="amount">The amount of plastics to be removed</param>
    public void RemovePlastics(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            RemovePlastic(usedPlastics[Random.Range(0, usedPlastics.Count - 1)]);
        }
    }

    /// <summary>
    /// Remove a specific plastic object
    /// </summary>
    /// <param name="plastic">The plastic object to be removed</param>
    public void RemovePlastic(GameObject plastic)
    {
        usedPlastics.Remove(plastic);
        //plastic.SetActive(false);
        FloatingObject floatingObject = plastic.GetComponent<FloatingObject>();
        floatingObject.Remove();
        availablePlastics.Add(plastic);
    }
}
