using OVRSimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine.Events;

public class ReadData : MonoBehaviour
{
    public TextAsset jsonFilePoll;
    public TextAsset jsonFileOil;
    public TextAsset jsonFilePlas;
    public string year, country, sort;
    public float pollution;
    public Boolean oilSpill, plasticSoup, waterPollution;
    public Boolean request;

    #region Events
    public UnityAction<string, float> onDataUpdate;
    #endregion

    Dictionary<string, Dictionary<string, Dictionary<string, float>>> data;
    Dictionary<string, Dictionary<string, float>> oil;
    Dictionary<string, Dictionary<string, float>> plastic;

    Dictionary<string, PollutionSpawner> pollutionSpawners;
    // Start is called before the first frame update
    void Start()
    {
        pollutionSpawners = new Dictionary<string, PollutionSpawner>();
        foreach (PollutionSpawner spawner in FindObjectsOfType<PollutionSpawner>())
        {
            pollutionSpawners.Add(spawner.GetTypeString(), spawner);
        }

        data = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, float>>>>(jsonFilePoll.text);
        oil = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, float>>>(jsonFileOil.text);
        plastic = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, float>>>(jsonFilePlas.text);

        year = "2007";
        country = null;
        sort = null;
        oilSpill = true;
        plasticSoup = true;
        waterPollution = true;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            SetFilteredData();
        }
    }

    // Update is called once per frame
    void SetFilteredData()
    {
        {
            Debug.Log("Handling request");
            if (waterPollution)
            {
                //if (year != null && country != null && sort != null && year != "" && country != "" && sort != "")
                //{
                //    print(year);
                //    print(country);
                //    print(sort);
                //    if (data.ContainsKey(year) && data[year].ContainsKey(country) && data[year][country].ContainsKey(sort))
                //    {
                //        pollution = data[year][country][sort];
                //    }

                //    // FloatStuff(sort, pollution);

                //}
                //else if (year != null && country != null && year != "" && country != "")
                //{
                //    pollution = 0;
                //    foreach (string srt in data[year][country].Keys)
                //    {
                //        if (data[year][country].ContainsKey(srt))
                //        {
                //            print(srt + ": " + data[year][country][srt] + " KG");
                //            pollution = pollution + data[year][country][srt];
                //        }

                //    }
                //    print("Total pollution in " + year + " in " + country + ": " + pollution + " in KG");
                //    // FloatStuff(sort, pollution);

                //}
                //else 
                if (year != null && year != "")
                {
                    //looping through the standard set of keys
                    foreach (string srt in data["2008"]["Norway"].Keys)
                    {
                        pollution = 0;
                        foreach (string cntry in data[year].Keys)
                        {
                            if (cntry != null && srt != null && data[year][cntry].ContainsKey(srt))
                            {
                                pollution = pollution + data[year][cntry][srt];
                            }
                        }
                        pollutionSpawners[srt].SetPollution(pollution);
                        onDataUpdate?.Invoke(srt, pollution);
                        //print(srt + " in " + year + ": " + pollution);
                    }
                }
            }
            else
            {
                foreach (string srt in data["2008"]["Norway"].Keys)
                {
                    pollutionSpawners[srt].SetPollution(0);
                    onDataUpdate?.Invoke(srt, 0);
                    //print(srt + " in " + year + ": " + 0);
                }
            }

            if (oilSpill)
            {
                pollutionSpawners["oil"].SetPollution(oil[year]["quantitySpilled"]);
                onDataUpdate?.Invoke("oil", oil[year]["quantitySpilled"]);
                //print("quantitySpilled" + " in " + year + ": " + oil[year]["quantitySpilled"]);
            }
            else
            {
                pollutionSpawners["oil"].SetPollution(0);
                onDataUpdate?.Invoke("oil", 0);
                //print("quantitySpilled" + " in " + year + ": " + 0);
            }

            if (plasticSoup)
            {
                pollutionSpawners["plastic"].SetPollution(plastic[year]["Total g plastic"]);
                onDataUpdate?.Invoke("plastic", plastic[year]["Total g plastic"]);
                //print("Total g plastic" + " in " + year + ": " + plastic[year]["Total g plastic"]);
            }
            else
            {
                pollutionSpawners["plastic"].SetPollution(0);
                onDataUpdate?.Invoke("plastic", 0);
                //print("Total g plastic" + " in " + year + ": " + 0);
            }

            request = false;
        }
    }

    void CalendarFilter(string yr)
    {
        year = yr;
        SetFilteredData();
    }

    void BoardFilter(Boolean oil, Boolean plastic, Boolean pollution)
    {
        oilSpill = oil;
        plasticSoup = plastic;
        waterPollution = pollution;
        SetFilteredData();
    }

}
