using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;
public class ChartEditor : MonoBehaviour
{
    public Koreography Koreo;
    public KoreographyTrack Chart;
    public SimpleMusicPlayer MusicPlayer;
    public AudioSource AudioCom;
    public List<KoreographyEvent> CurEventList = new List<KoreographyEvent>();
    internal string ClipName;
    public int sampleTime;
    public int subdivs = 2;
    public int CurSample;
    bool pause;

    // Start is called before the first frame update
    void Start()
    {
        ClipName = MusicPlayer.GetCurrentClipName();
        //MusicPlayer.LoadSong(Koreo, 0, false);
        AudioCom.Play();
    }

    private void Update()
    {
        //Debug.Log("dd");
        CurEventList = Chart.GetAllEvents();
        if (Input.GetKeyDown("w"))
        {
            AddTapEvents(1);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            AddTapEvents(2);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            AddTapEvents(3);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            AddTapEvents(4);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AddTapEvents(21);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            AddTapEvents(22);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangePauseState();
        }
    }

    public void AddTapEvents(int i)
    {
        CurSample = MusicPlayer.GetSampleTimeForClip(ClipName);
        sampleTime = Koreo.GetSampleOfNearestBeat(CurSample, subdivs);
        //SampleTime = MusicPlayer.GetSampleTimeForClip(ClipName);
        KoreographyEvent evt = new KoreographyEvent();
        IntPayload payload = new IntPayload();
        payload.IntVal = i;
        evt.Payload = payload;
        evt.StartSample = sampleTime;
        evt.EndSample = sampleTime;
        Chart.AddEvent(evt);
        if (Chart.AddEvent(evt))
        {
            Debug.Log("Added in " + i);
        }
        else
        {
            Debug.Log("Failed to add in " + i);
        }
    }

    public void ChangePauseState()
    {
        pause = !pause;
        if (pause)
        {
            AudioCom.Play();
        }
        else
        {
            AudioCom.Pause();
        }
    }
}
