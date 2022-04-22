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
    public int TouchInSameTime;
    public int sampleTime;
    public int subdivs = 2;
    public int CurSample;
    bool pause;

    internal bool addTap1 = true, addTap2 = true, addTap3 = true, addTap4 = true, addBigTap1 = true, addBigTap2 = true;

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
        if (Input.GetKeyDown(KeyCode.W))
        {
            KeyDownEvent(1);
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            KeyUpEvent(1);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            KeyDownEvent(2);
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            KeyUpEvent(2);
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            KeyDownEvent(3);
        }
        if (Input.GetKeyUp(KeyCode.U))
        {
            KeyUpEvent(3);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            KeyDownEvent(4);
        }
        if (Input.GetKeyUp(KeyCode.I))
        {
            KeyUpEvent(4);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            KeyDownEvent(21);
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            KeyUpEvent(21);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            KeyDownEvent(22);
        }
        if (Input.GetKeyUp(KeyCode.O))
        {
            KeyUpEvent(22);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangePauseState();
        }
    }

    public void AddTapEvents(int i)
    {
        Debug.Log("Hi in " + i);
        /*CurSample = MusicPlayer.GetSampleTimeForClip(ClipName);
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
        }*/
    }

    IEnumerator AddEvents(int i,int Seq)
    {
        CurSample = MusicPlayer.GetSampleTimeForClip(ClipName);
        sampleTime = Koreo.GetSampleOfNearestBeat(CurSample, subdivs);
        KoreographyEvent evt = new KoreographyEvent();
        IntPayload payload = new IntPayload();
        payload.IntVal = i;
        evt.Payload = payload;
        evt.StartSample = sampleTime + Seq;
        evt.EndSample = sampleTime + Seq;
        Debug.Log("Event added in Lane" + i + ", Sample offset is " + (sampleTime - evt.StartSample));
        Chart.AddEvent(evt);
        yield return null;
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

    public void KeyDownEvent(int Num)
    {
        switch (Num)
        {
            case 1:
                StartCoroutine(AddEvents(1, TouchInSameTime));
                //AddTapEvents(1);
                if (addTap1)
                {
                    TouchInSameTime++;
                    addTap1 = false;
                }
                break;
            case 2:
                StartCoroutine(AddEvents(2, TouchInSameTime));
                //AddTapEvents(2);
                if (addTap2)
                {
                    TouchInSameTime++;
                    addTap2 = false;
                }
                break;
            case 3:
                StartCoroutine(AddEvents(3, TouchInSameTime));
                //AddTapEvents(3);
                if (addTap3)
                {
                    TouchInSameTime++;
                    addTap3 = false;
                }
                break;
            case 4:
                StartCoroutine(AddEvents(4, TouchInSameTime));
                //AddTapEvents(4);
                if (addTap4)
                {
                    TouchInSameTime++;
                    addTap4 = false;
                }
                break;
            case 21:
                StartCoroutine(AddEvents(21, TouchInSameTime));
                //AddTapEvents(21);
                if (addBigTap1)
                {
                    TouchInSameTime++;
                    addBigTap1 = false;
                }
                break;
            case 22:
                StartCoroutine(AddEvents(22, TouchInSameTime));
                //AddTapEvents(22);
                if (addBigTap2)
                {
                    TouchInSameTime++;
                    addBigTap2 = false;
                }
                break;
            default:
                break;
        }
    }

    public void KeyUpEvent(int Num)
    {
        switch (Num)
        {
            case 1:
                if (!addTap1)
                {
                    TouchInSameTime--;
                    addTap1 = true;
                }
                break;
            case 2:
                if (!addTap2)
                {
                    TouchInSameTime--;
                    addTap2 = true;
                }
                break;
            case 3:
                if (!addTap3)
                {
                    TouchInSameTime--;
                    addTap3 = true;
                }
                break;
            case 4:
                if (!addTap4)
                {
                    TouchInSameTime--;
                    addTap4 = true;
                }
                break;
            case 21:
                if (!addBigTap1)
                {
                    TouchInSameTime--;
                    addBigTap1 = true;
                }
                break;
            case 22:
                if (!addBigTap2)
                {
                    TouchInSameTime--;
                    addBigTap2 = true;
                }
                break;
            default:
                break;
        }
    }
}
