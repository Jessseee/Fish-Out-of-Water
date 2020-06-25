using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PollutionSpawner : MonoBehaviour
{
    public enum PollutionType
    {
        Plastic,
        Oil,
        Organic,
        Inorganic,
        Chlorinated,
        Pesticides,
        Metal,
        Gas,
    }

    // Public vairables
    public PollutionType type;
    public int depth = 20;
    public int spawnPlaneRadius = 20;
    public GameObject pollutionPrefab;
    public long divisionFactor = 100;
    public int maxPollution = 100;

    // Private variables
    private List<GameObject> usedPollutions;
    private List<GameObject> availablePollutions;
    private Vector3 zeroLoc;


    public string GetTypeString()
    {
        switch (type)
        {
            case PollutionType.Plastic:
                return "plastic";
            case PollutionType.Oil:
                return "oil";
            case PollutionType.Organic:
                return "Other organic substances";
            case PollutionType.Inorganic:
                return "Inorganic substances";
            case PollutionType.Chlorinated:
                return "Chlorinated organic substances";
            case PollutionType.Pesticides:
                return "Pesticides";
            case PollutionType.Metal:
                return "Heavy metals";
            case PollutionType.Gas:
                return "Other gases";
            default:
                return "";
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        zeroLoc = transform.position + new Vector3(0, -depth, 0);
        usedPollutions = new List<GameObject>();
        availablePollutions = new List<GameObject>();
        for (int i = 0; i < maxPollution; i++)
        {
            GameObject newPollution = Instantiate(pollutionPrefab, zeroLoc, Random.rotation);
            newPollution.SetActive(false);
            availablePollutions.Add(newPollution);
        }
    }

    private void Update()
    {
        foreach(GameObject pollution in usedPollutions)
        {
            if (pollution.GetComponent<FloatingObject>().IsRemoved())
            {
                usedPollutions.Remove(pollution);
                pollution.SetActive(false);
                availablePollutions.Add(pollution);
            }
        }
    }

    public void SetPollution(float pollution)
    {
        int numberOfObjects = (int) Math.Round(pollution / divisionFactor * maxPollution);
        int difference = numberOfObjects - usedPollutions.Count;
        if (difference > 0)
        {
            AddPollutions(difference);
        }
        else if (difference < 0)
        {
            RemovePollutions(-difference);
        }
    }

    /// <summary>
    /// Add a specified amount of pollutions
    /// </summary>
    /// <param name="amount">The amount of pollutions to be added</param>
    public void AddPollutions(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            AddPollution(availablePollutions[0]);
        }
    }

    /// <summary>
    /// Add a specific pollution object
    /// </summary>
    /// <param name="pollution">The pollution object to be added</param>
    public void AddPollution(GameObject pollution)
    {
        // Find an available spawn location
        bool spawned = false;
        int tries = 0;
        while (!spawned && tries < 1000)
        {
            float angle = Random.value * 360;
            float radius = Random.value * spawnPlaneRadius;

            Vector3 spawnLocation = new Vector3(
                Mathf.Cos(angle) * radius + transform.position.x,
                transform.position.y,
                Mathf.Sin(angle) * radius + transform.position.z
                );
            Vector3 size = pollution.GetComponent<MeshRenderer>().bounds.size;
            float checkRadius = Mathf.Max(size.x, Mathf.Max(size.y, size.z));
            if (Physics.OverlapSphere(spawnLocation, checkRadius).Length == 0)
            {
                availablePollutions.Remove(pollution);
                pollution.SetActive(true);
                FloatingObject floatingObject = pollution.GetComponent<FloatingObject>();
                floatingObject.SetTarget(spawnLocation, depth);
                spawned = true;
                usedPollutions.Add(pollution);
            }
            tries++;
        }
    }

    /// <summary>
    /// Remove a specified amount of pollutions
    /// </summary>
    /// <param name="amount">The amount of pollutions to be removed</param>
    public void RemovePollutions(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            RemovePollution(usedPollutions[Random.Range(0, usedPollutions.Count - 1)]);
        }
    }

    /// <summary>
    /// Remove a specific pollution object
    /// </summary>
    /// <param name="pollution">The pollution object to be removed</param>
    public void RemovePollution(GameObject pollution)
    {
        usedPollutions.Remove(pollution);
        //pollution.SetActive(false);
        FloatingObject floatingObject = pollution.GetComponent<FloatingObject>();
        floatingObject.Remove(depth);
        availablePollutions.Add(pollution);
    }
}
