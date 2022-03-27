using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;
public class ChartEditor : MonoBehaviour
{
    public Koreography Koreo;
    public KoreographyTrack Chart;
    public KoreographyTrack FormatPool;
    public SimpleMusicPlayer MusicPlayer;
    public AudioSource AudioCom;
    List<KoreographyEvent> EventList = new List<KoreographyEvent>();
    public List<KoreographyEvent> CurEventList = new List<KoreographyEvent>();
    internal string ClipName;
    public int SampleTime;
    KoreographyEvent evt;

    // Start is called before the first frame update
    void Start()
    {
        ClipName = MusicPlayer.GetCurrentClipName();
        EventList = FormatPool.GetAllEvents();
        //MusicPlayer.LoadSong(Koreo, 0, true);
        AudioCom.Play();
    }

    private void Update()
    {
        SampleTime = MusicPlayer.GetSampleTimeForClip(ClipName);
        CurEventList = Chart.GetAllEvents();
        if (Input.GetKeyDown(KeyCode.D))
        {
            AddEvents(1);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            AddEvents(2);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            AddEvents(3);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            AddEvents(4);
        }
    }

    public void AddEvents(int i)
    {
        i = i-1;
        evt = EventList[i];
        evt.StartSample = SampleTime;
        evt.EndSample = SampleTime;
        Chart.AddEvent(evt);
        //Chart.SetDirty();
        if (Chart.AddEvent(evt))
        {
            Debug.Log("Added in " + i);
        }
        else
        {
            Debug.Log("Failed to add in " + i);
        }
    }
}
