using OVRSimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public class ReadData : MonoBehaviour
{
    public TextAsset jsonFilePoll;
    public TextAsset jsonFileOil;
    public TextAsset jsonFilePlas;
    public string year, country, sort;
    public float pollution, plastics, oils, spills;

    Dictionary<string, Dictionary<string, Dictionary<string, float>>> data;
    Dictionary<string, Dictionary<string, float>> oil;
    Dictionary<string, Dictionary<string, float>> plastic;
    // Start is called before the first frame update
    void Start()
    {
        data = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, float>>>>(jsonFilePoll.text);
        oil = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, float>>>(jsonFileOil.text);
        plastic = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, float>>>(jsonFilePlas.text);

        year = "2007";
        country = null;
        sort = null;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(data["2007"]["Austria"]["Heavy metals"]);
        //foreach (string key in data["2007"].Keys)
        //{
        //    Debug.Log(key);
        //}


        if (year != null && country != null && sort != null && year != "" && country != "" && sort != "")
        {
            print(year);
            print(country);
            print(sort);
            if (data.ContainsKey(year) && data[year].ContainsKey(country) && data[year][country].ContainsKey(sort))
            {
                pollution = data[year][country][sort];
            }

            // FloatStuff(sort, pollution);

        }
        else if (year != null && country != null && year != "" && country != "" )
        {
            pollution = 0;
            foreach (string srt in data[year][country].Keys)
            {   if (data[year][country].ContainsKey(srt))
                {
                    print(srt + ": " + data[year][country][srt] + " KG");
                    pollution = pollution + data[year][country][srt];
                }
                
            }
            print("Total pollution in " + year + " in " + country + ": " + pollution + " in KG");
            // FloatStuff(sort, pollution);

        }
        else if (year != null && year != "")
        {
            pollution = 0;
            foreach (string cntry in data[year].Keys)
            {
                foreach (string srt in data[year][cntry].Keys)
                {
                    if (cntry != null && srt != null && data[year][cntry].ContainsKey(srt))
                    {
                        pollution = pollution + data[year][cntry][srt];
                    }
                }
                // FloatStuffTotal(pollution);

            }
            //foreach (string key in data[year].Keys)
            //{
                foreach (string srt in data["2007"]["Austria"].Keys)
                {
                    pollution = 0;
                    foreach (string cntry in data[year].Keys)
                    {
                        if (cntry != null && srt != null && data[year][cntry].ContainsKey(srt))
                        {
                            pollution = pollution + data[year][cntry][srt];
                        }
                    }
                    //FloatStuff(srt, pollution);
                    print(srt + " in " + year + ": " + pollution);
                }


            //}

            
            foreach(string srt in oil[year].Keys)
            {
                //FloatStuff(srt, oil[year][srt]);
                print(srt + " in " + year + ": " + oil[year][srt]);
            }

            foreach (string srt in plastic[year].Keys)
            {
                //FloatStuff(srt, plastic[year][srt]);
                print(srt + " in " + year + ": " + plastic[year][srt]);
            }
        }


    }
    void Filter(string yr, string cntry, string srt)
    {
        year = yr;
        country = cntry;
        sort = srt;


    }

}
