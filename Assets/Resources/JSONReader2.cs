using UnityEngine;

public class JSONReader2 : MonoBehaviour
{
    public TextAsset jsonFile;

    void Start()
    {
        Vehicles vehiclesInJson = JsonUtility.FromJson<Vehicles>(jsonFile.text);

        foreach (Vehicle vehicle in vehiclesInJson.vehicles)
        {
            Debug.Log("Found employee: " + vehicle.firstName + " " + vehicle.lastName);
        }
    }
}