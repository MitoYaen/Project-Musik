using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;
using Bolt;
using UnityEngine.SceneManagement;

public class RythmGameManager : MonoBehaviour
{
    [EventID]
    public string EventID;

    public float InGameNoteSpeed;
    
    public float NoteSpeed;

    [Range(8f,200f)]
    public float HitWindowSize_ms;

    public int FullScore;

    public float PerScore;

    public float CurScore;

    public int TotalNotes;

    public GameObject BoltLinkObject;

    public float HitWindowSize_unit
    { 
        get
        {
            return NoteSpeed * (HitWindowSize_ms * 0.01f);
        }
    }
    
    int HitWindowSize_sample;

    public int HitWindowSampleWidth
    {
        get 
        {
            return HitWindowSize_sample;
        }
    }

    public int SampleRate
    {
        get
        {
            return PlayingKoreo.SampleRate;
        }
    }

    Stack<Note> noteObjectPool = new Stack<Note>();

    //Quote

    Koreography PlayingKoreo;

    public List<LaneController> noteLanes = new List<LaneController>();

    public AudioSource AudioCom;

    //Prefab

    public Note noteObject;

    public GameObject HitEffectNote;

    public GameObject HitEffectLongNote;



    //Time Related
    [Tooltip("开始播放音频之前提供的时间量，以秒为单位")]
    public float LeadInTime;

    float LeadInTimeLeft;
    //Countdown Before the music actually plays(Which means the events will be called first)
    float TimeLeftToPlay;

    //Previous SampleTime, including any necessary delay (To koreo)
    public int DelayedSampleTime
    {
        get
        {
            return PlayingKoreo.GetLatestSampleTime() - (int)(SampleRate * LeadInTimeLeft);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

        //Multiply the note speed

        NoteSpeed = InGameNoteSpeed * 5;

        InitializeLeadIn();
        for (int i = 0; i < noteLanes.Count; ++i)
        {
            noteLanes[i].Initialize(this);
        }
        PlayingKoreo = Koreographer.Instance.GetKoreographyAtIndex(0);
        KoreographyTrackBase rythmTrack = PlayingKoreo.GetTrackByID(EventID);
        //Get Event

        List<KoreographyEvent> rawEvents = rythmTrack.GetAllEvents();

        TotalNotes = rawEvents.Count;
        PerScore = FullScore / TotalNotes ;
        Variables.ActiveScene.Set("PerScore",PerScore);
        for (int i = 0; i < rawEvents.Count; ++i)
        {
            KoreographyEvent evt = rawEvents[i];
            int noteID = evt.GetIntValue();
            for (int j = 0; j < noteLanes.Count; ++j)
            {
                LaneController lane = noteLanes[j];
                if (noteID > 4)
                {
                    noteID = noteID - 4;
                    if (noteID > 4)
                    {
                        noteID = noteID - 4;
                    }
                }
                if (lane.DoesMatch(noteID))
                {
                    lane.AddEventToLane(evt);
                    break;
                }
            }
        }
        HitWindowSize_sample = (int)(0.001f * HitWindowSize_ms * SampleRate );

        
    }

    // Update is called once per frame
    void Update() {
        if (TimeLeftToPlay > 0)
        {
            TimeLeftToPlay -= Time.unscaledDeltaTime;

            if (TimeLeftToPlay <= 0)
            {
                AudioCom.Play();
                TimeLeftToPlay = 0;
            }
        }
        //
        if (LeadInTimeLeft>0)
        {
            LeadInTimeLeft = Mathf.Max(LeadInTimeLeft - Time.unscaledDeltaTime, 0);
        }
        
        //Update the scores to Bolt
        Variables.Object(BoltLinkObject).Set("Score_Ori", CurScore);

        //Cheats to debug
        if (Input.GetKeyDown(KeyCode.K))
        {
            CurScore += 3000;
            Debug.Log("The score adds 3000, now is " + CurScore);
        }

    }
    /// <summary>
    /// InitializeLeadTime  
    /// </summary>
    void InitializeLeadIn ()
    {
        if (LeadInTime>0)
        {
            LeadInTimeLeft = LeadInTime;
            TimeLeftToPlay = LeadInTime;
        }
        else
        {
            AudioCom.Play();
        }
    }


    //pick up the object from the pool
    public Note GetFreshNoteObject()
    {
        Note retObj;

        if (noteObjectPool.Count>0)
        {
            retObj = noteObjectPool.Pop();
        }
        else
        {
            //Origin of the resource
            retObj = Instantiate(noteObject);
        }

        retObj.gameObject.SetActive(true);
        retObj.enabled = true;

        return retObj;
    }


    //Put back the object to the pool

    public void ReturnNoteObjectToPool(Note obj)
    {
        if (obj!=null)
        {
            obj.enabled = false;
            obj.gameObject.SetActive(false);
            noteObjectPool.Push(obj);
        }
    }

    //Score Update (Pure)
    public void PureScoreUpdate()
    {
        CurScore += PerScore;
    }

    //Score Update (Far)
    public void FarScoreUpdate()
    {
        CurScore += PerScore/2;
    }

    //Reload Scene
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
