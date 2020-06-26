using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Events;

public class ReadData : MonoBehaviour
{
    public TextAsset jsonFilePoll;
    public TextAsset jsonFileOil;
    public TextAsset jsonFilePlas;

    #region Event
    public static UnityAction<string, float> onDataUpdate;
    #endregion

    private string year, country, type;
    private float pollution;
    private bool oilSpill, plasticSoup, waterPollution;

    private Dictionary<string, Dictionary<string, Dictionary<string, float>>> data;
    private Dictionary<string, Dictionary<string, float>> oil;
    private Dictionary<string, Dictionary<string, float>> plastic;
    private Dictionary<string, PollutionSpawner> pollutionSpawners;

    private void Awake()
    {
        year = "2007";
        country = null;
        type = null;
        oilSpill = true;
        plasticSoup = true;
        waterPollution = true;

        InfoBoard.onDateUpdate += CalendarFilter;
        InfoBoard.onFilterUpdate += BoardFilter;
    }

    private void Start()
    {
        pollutionSpawners = new Dictionary<string, PollutionSpawner>();
        foreach (PollutionSpawner spawner in FindObjectsOfType<PollutionSpawner>())
        {
            pollutionSpawners.Add(spawner.GetTypeString(), spawner);
        }

        data = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, float>>>>(jsonFilePoll.text);
        oil = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, float>>>(jsonFileOil.text);
        plastic = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, float>>>(jsonFilePlas.text);
    }

    private void Update()
    {
        // press mouse button to spawn pollution objects for debugging
        if(Input.GetMouseButtonDown(0))
        {
            SetFilteredData();
        }
    }

    private void OnDestroy()
    {
        InfoBoard.onDateUpdate -= CalendarFilter;
        InfoBoard.onFilterUpdate -= BoardFilter;
    }

    void SetFilteredData()
    {
        Debug.Log(data);
        {
            if (waterPollution)
            {
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
                    }
                }
            }
            else
            {
                foreach (string srt in data["2008"]["Norway"].Keys)
                {
                    pollutionSpawners[srt].SetPollution(0);
                    onDataUpdate?.Invoke(srt, 0);
                }
            }

            if (oilSpill)
            {
                pollutionSpawners["oil"].SetPollution(oil[year]["quantitySpilled"]);
                onDataUpdate?.Invoke("oil", oil[year]["quantitySpilled"]);
            }
            else
            {
                pollutionSpawners["oil"].SetPollution(0);
                onDataUpdate?.Invoke("oil", 0);
            }

            if (plasticSoup && int.Parse(year) <= 2013)
            {
                pollutionSpawners["plastic"].SetPollution(plastic[year]["Total g plastic"]);
                onDataUpdate?.Invoke("plastic", plastic[year]["Total g plastic"]);
            }
            else
            {
                pollutionSpawners["plastic"].SetPollution(0);
                onDataUpdate?.Invoke("plastic", 0);
            }
        }
    }

    void CalendarFilter(string yr)
    {
        year = yr;
        SetFilteredData();
    }

    void BoardFilter(bool oil, bool plastic, bool pollution)
    {
        oilSpill = oil;
        plasticSoup = plastic;
        waterPollution = pollution;

        SetFilteredData();
    }

}
