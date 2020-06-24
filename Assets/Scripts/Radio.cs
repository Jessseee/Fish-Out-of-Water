using UnityEngine;

public class Radio : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string RadioStaticSoundEvent = "";

    private FMOD.Studio.EventInstance newsReportSound;
    private FMOD.Studio.EventInstance radioStaticSound;

    private void Awake()
    {
        InfoBoard.onDateUpdate += PlayNewsReport;

        radioStaticSound = FMODUnity.RuntimeManager.CreateInstance(RadioStaticSoundEvent);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(radioStaticSound, GetComponent<Transform>(), GetComponent<Rigidbody>());
    }

    private void PlayNewsReport(string year)
    {
        newsReportSound.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE playbackState);
        if (playbackState == FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            newsReportSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            radioStaticSound.start();
            radioStaticSound.release();
        }

        newsReportSound = FMODUnity.RuntimeManager.CreateInstance("event:/" + year);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(newsReportSound, GetComponent<Transform>(), GetComponent<Rigidbody>());
        newsReportSound.start();
        radioStaticSound.release();
    }
}
