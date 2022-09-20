using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;
using System;

public class ChartEditor : MonoBehaviour
{
    [Header("Basics")]
    [Header("")]
    [Header(" - Normal Keys : W,E,U,I")]
    [Header(" - Side Keys : Q,O")]
    [Header("HotKeys:")]
    [Header("Put your koreo and Chart before start editing!")]
    public Koreography Koreo;
    public KoreographyTrack Chart;
    [Range(0.1f,1f)]
    [SerializeField] float PlaybackSpd;
    public int subdivs = 2;
    public Boolean HoldMode = false;
    [Header("Scene")]
    public List<SpriteRenderer> PressedVisuals;
    public GameObject Target;
    public SimpleMusicPlayer MusicPlayer;
    public AudioSource AudioCom;
    internal string ClipName;
    [Header("Debug")]
    public List<KoreographyEvent> CurEventList = new List<KoreographyEvent>();
    public int TouchInSameTime;
    public int sampleTime;
    public int CurSample;
    bool pause;

    internal bool addTap1 = true, addTap2 = true, addTap3 = true, addTap4 = true, addBigTap1 = true, addBigTap2 = true,addHoldL = true, addHoldR = true;

    // Start is called before the first frame update
    void Start()
    {
        Target.gameObject.SetActive(false);
        ClipName = MusicPlayer.GetCurrentClipName();
        //MusicPlayer.LoadSong(Koreo, 0, false);
        AudioCom.pitch = PlaybackSpd;
        AudioCom.Play();
    }

    private void Update()
    {
        //Debug.Log("dd");
        CurEventList = Chart.GetAllEvents();
        if (Input.GetKeyDown(KeyCode.W))
        {
            KeyDownEvent(1);
            PressedVisuals[0].GetComponent<SpriteRenderer>().enabled = true;
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            KeyUpEvent(1);
            PressedVisuals[0].GetComponent<SpriteRenderer>().enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            KeyDownEvent(2);
            PressedVisuals[1].GetComponent<SpriteRenderer>().enabled = true;
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            KeyUpEvent(2);
            PressedVisuals[1].GetComponent<SpriteRenderer>().enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            KeyDownEvent(3);
            PressedVisuals[2].GetComponent<SpriteRenderer>().enabled = true;
        }
        if (Input.GetKeyUp(KeyCode.U))
        {
            KeyUpEvent(3);
            PressedVisuals[2].GetComponent<SpriteRenderer>().enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            KeyDownEvent(4);
            PressedVisuals[3].GetComponent<SpriteRenderer>().enabled = true;
        }
        if (Input.GetKeyUp(KeyCode.I))
        {
            KeyUpEvent(4);
            PressedVisuals[3].GetComponent<SpriteRenderer>().enabled = false;
        }
        if (HoldMode)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                KeyDownEvent(17);
                PressedVisuals[4].gameObject.SetActive(true);
            }
            if (Input.GetKeyUp(KeyCode.Q))
            {
                KeyUpEvent(19);
                PressedVisuals[4].gameObject.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                KeyDownEvent(18);
                PressedVisuals[5].gameObject.SetActive(true);
            }
            if (Input.GetKeyUp(KeyCode.O))
            {
                KeyUpEvent(20);
                PressedVisuals[5].gameObject.SetActive(false);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                KeyDownEvent(21);
                PressedVisuals[4].gameObject.SetActive(true);
            }
            if (Input.GetKeyUp(KeyCode.Q))
            {
                KeyUpEvent(21);
                PressedVisuals[4].gameObject.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                KeyDownEvent(22);
                PressedVisuals[5].gameObject.SetActive(true);
            }
            if (Input.GetKeyUp(KeyCode.O))
            {
                KeyUpEvent(22);
                PressedVisuals[5].gameObject.SetActive(false);
            }
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
            case 17:
                StartCoroutine(AddEvents(17, TouchInSameTime));
                if (addHoldL)
                {
                    TouchInSameTime++;
                    addHoldL = false;
                }
                break;
            case 18:
                StartCoroutine(AddEvents(18, TouchInSameTime));
                if (addHoldR)
                {
                    TouchInSameTime++;
                    addHoldR = false;
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
            case 19:
                if (!addHoldL)
                {
                    StartCoroutine(AddEvents(19, TouchInSameTime));
                    TouchInSameTime--;
                    addHoldL = true;
                }
                break;
            case 20:
                if (!addHoldR)
                {
                    StartCoroutine(AddEvents(20, TouchInSameTime));
                    TouchInSameTime--;
                    addHoldR = true;
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
