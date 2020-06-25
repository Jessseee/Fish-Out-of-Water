using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class InfoBoard : MonoBehaviour
{
    public static UnityAction<string> onDateUpdate;

    public int calendarCurrentDate = 2007;
    public int calendarMinDate = 2007;
    public int calendarMaxDate = 2017;

    public TextMeshPro plasticPollutionText;
    public TextMeshPro oilPollutionText;
    public TextMeshPro inorganicPollutionText;
    public TextMeshPro calendarDateText;

    [FMODUnity.EventRef]
    public string calendarFlipSoundEvent = "";

    private void Awake()
    {
        ReadData.onDataUpdate += UpdatePollutionText;
    }

    private void OnDestroy()
    {
        ReadData.onDataUpdate -= UpdatePollutionText;
    }

    public void CalendarDateDecrease()
    {
        if (calendarCurrentDate - 1 >= calendarMinDate)
            calendarCurrentDate -= 1;
        else
            return;

        UpdateDate();
    }

    public void CalendarDateIncrease()
    {
        if (calendarCurrentDate + 1 <= calendarMaxDate)
            calendarCurrentDate += 1;
        else
            return;

        UpdateDate();
    }

    private void UpdateDate()
    {
        onDateUpdate?.Invoke(calendarCurrentDate.ToString());
        FMODUnity.RuntimeManager.PlayOneShot(calendarFlipSoundEvent);
        calendarDateText.text = calendarCurrentDate.ToString();
    }

    private void UpdatePollutionText(string type, float amount)
    {
        if (type == "plastic")
        {
            if (calendarCurrentDate > 2013)
                plasticPollutionText.text = "N/A" + Environment.NewLine + " g/km2";
            else
                plasticPollutionText.text = amount.ToString("n0") + Environment.NewLine + " g/km2";
        }

        if (type == "oil")
            oilPollutionText.text = amount.ToString("n0") + Environment.NewLine + "tonnes";

        if (type == "Inorganic substances")
        {
            amount /= 1000;
            inorganicPollutionText.text = amount.ToString("n0") + "tonnes";
        }
    }
}
