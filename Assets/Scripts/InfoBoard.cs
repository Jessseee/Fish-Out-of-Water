using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class InfoBoard : MonoBehaviour
{
    public static UnityAction<string> onDateUpdate;

    public int calendarCurrentDate = 2007;
    public int calendarMinDate = 2007;
    public int calendarMaxDate = 2017;
    public TextMeshPro calendarDateText;
    [FMODUnity.EventRef]
    public string calendarFlipSoundEvent = "";

    public void CalendarDateDown()
    {
        if (calendarCurrentDate - 1 >= calendarMinDate)
            calendarCurrentDate -= 1;
        else
            return;

        // filter data

        ChangeDate();
    }

    public void CalendarDateUp()
    {
        if (calendarCurrentDate + 1 <= calendarMaxDate)
            calendarCurrentDate += 1;
        else
            return;

        // filter data

        ChangeDate();
    }

    private void ChangeDate()
    {
        onDateUpdate?.Invoke(calendarCurrentDate.ToString());
        FMODUnity.RuntimeManager.PlayOneShot(calendarFlipSoundEvent);
        calendarDateText.text = calendarCurrentDate.ToString();
    }
}
