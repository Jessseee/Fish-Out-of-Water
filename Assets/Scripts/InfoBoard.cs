using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InfoBoard : MonoBehaviour
{
    public static UnityAction<string> onDateUpdate;
    public static UnityAction<bool, bool, bool> onFilterUpdate;

    public int calendarCurrentDate = 2007;
    public int calendarMinDate = 2007;
    public int calendarMaxDate = 2017;

    public TextMeshPro plasticPollutionText;
    public TextMeshPro oilPollutionText;
    public TextMeshPro inorganicPollutionText;
    public TextMeshPro calendarDateText;

    public Image plasticCheckbox;
    public Image oilCheckbox;
    public Image inorganicCheckbox;

    public Sprite checkedBox;
    public Sprite uncheckedBox;

    private bool filterOil = true, filterPlastic = true, filterInorganic = true;

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

    public void TogglePlastic()
    {
        filterPlastic = !filterPlastic;
        onFilterUpdate?.Invoke(filterOil, filterPlastic, filterInorganic);
        if(filterPlastic)
            plasticCheckbox.sprite = checkedBox;
        else
            plasticCheckbox.sprite = uncheckedBox;
    }

    public void ToggleOil()
    {
        filterOil = !filterOil;
        onFilterUpdate?.Invoke(filterOil, filterPlastic, filterInorganic);
        if (filterOil)
            oilCheckbox.sprite = checkedBox;
        else
            oilCheckbox.sprite = uncheckedBox;
    }

    public void ToggleInorganic()
    {
        filterInorganic = !filterInorganic;
        onFilterUpdate?.Invoke(filterOil, filterPlastic, filterInorganic);
        if(filterInorganic)
            inorganicCheckbox.sprite = checkedBox;
        else
            inorganicCheckbox.sprite = uncheckedBox;
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
            if (calendarCurrentDate > 2013 || amount == 0)
                plasticPollutionText.text = "N/A" + Environment.NewLine + " g/km2";
            else
                plasticPollutionText.text = amount.ToString("n0") + Environment.NewLine + " g/km2";
        }

        if (type == "oil")
        {
            if (amount == 0)
                oilPollutionText.text = "N/A" + Environment.NewLine + "tonnes";
            else
                oilPollutionText.text = amount.ToString("n0") + Environment.NewLine + "tonnes";
        }
            

        if (type == "Inorganic substances")
        {
            if (amount == 0)
                inorganicPollutionText.text = "N/A" + Environment.NewLine + "tonnes";
            else
            {
                amount /= 1000;
                inorganicPollutionText.text = amount.ToString("n0") + "tonnes";
            }
        }
    }
}
