using UnityEngine;

public class JSONReader2 : MonoBehaviour
{
    public TextAsset jsonFile;

    void Start()
    {
        Vehicles vehiclesInJson = JsonUtility.FromJson<Vehicles>(jsonFile.text);
        Vehicles vehicles2InJson = JsonUtility.FromJson<Vehicles>(jsonFile.text);

        foreach (Vehicle vehicle in vehiclesInJson.vehicles)
        {
            Debug.Log("Found employee: " + vehicle.firstName + " " + vehicle.lastName);
        }

        foreach (Vehicle vehicle2 in vehicles2InJson.vehicles2)
        {
            Debug.Log("Found employee: " + vehicle2.firstName + " " + vehicle2.lastName);
            
        }
    }
}